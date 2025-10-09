using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Services;

public class MatchingService : IMatchingService
{
    private readonly TravelDbContext _context;
    private readonly IAuditService _auditService;

    public MatchingService(TravelDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<IEnumerable<TripMatchingSuggestion>> GetAutoMatchingSuggestionsAsync()
    {
        var suggestions = new List<TripMatchingSuggestion>();
        
        var tripsWithoutTransactions = await _context.Trips
            .Where(t => !_context.Transactions.Any(tr => tr.TripId == t.TripId))
            .ToListAsync();

        foreach (var trip in tripsWithoutTransactions)
        {
            var matchingSuggestion = await GenerateMatchingSuggestionForTripAsync(trip);
            if (matchingSuggestion.SuggestedTransactions.Any())
            {
                suggestions.Add(matchingSuggestion);
            }
        }

        return suggestions.OrderByDescending(s => s.OverallConfidence);
    }

    public async Task<IEnumerable<TripMatchingSuggestion>> GetMatchingSuggestionsForTripAsync(int tripId)
    {
        var trip = await _context.Trips.FindAsync(tripId);
        if (trip == null)
            return Enumerable.Empty<TripMatchingSuggestion>();

        var suggestion = await GenerateMatchingSuggestionForTripAsync(trip);
        return new[] { suggestion };
    }

    public async Task<IEnumerable<Transaction>> GetUnlinkedTransactionsAsync(string email, DateTime startDate, DateTime endDate)
    {
        var toleranceStart = startDate.AddDays(-5);
        var toleranceEnd = endDate.AddDays(5);

        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.Email == email && 
                       t.TripId == null &&
                       t.TransactionDate >= toleranceStart &&
                       t.TransactionDate <= toleranceEnd)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<bool> LinkTransactionToTripAsync(string transactionId, int tripId, string userId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        var trip = await _context.Trips.FindAsync(tripId);

        if (transaction == null || trip == null)
            return false;

        var oldValue = new { TripId = transaction.TripId };
        
        transaction.TripId = tripId;
        transaction.ModifiedAt = DateTime.UtcNow;
        transaction.ModifiedBy = userId;

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(
            userId, 
            "Link", 
            "Transactions", 
            transactionId,
            oldValue,
            new { TripId = tripId },
            $"Linked transaction to trip: {trip.TripName}"
        );

        return true;
    }

    public async Task<bool> UnlinkTransactionFromTripAsync(string transactionId, string userId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction == null)
            return false;

        var oldValue = new { TripId = transaction.TripId };
        
        transaction.TripId = null;
        transaction.ModifiedAt = DateTime.UtcNow;
        transaction.ModifiedBy = userId;

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(
            userId, 
            "Unlink", 
            "Transactions", 
            transactionId,
            oldValue,
            new { TripId = (int?)null },
            "Unlinked transaction from trip"
        );

        return true;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsForMatchingAsync(int tripId, int daysTolerance = 5)
    {
        var trip = await _context.Trips.FindAsync(tripId);
        if (trip == null)
            return Enumerable.Empty<Transaction>();

        var toleranceStart = trip.StartDate.AddDays(-daysTolerance);
        var toleranceEnd = trip.EndDate.AddDays(daysTolerance);

        return await _context.Transactions
            .Where(t => t.Email == trip.Email &&
                       t.TransactionDate >= toleranceStart &&
                       t.TransactionDate <= toleranceEnd)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<decimal> CalculateMatchingConfidenceAsync(Transaction transaction, Trip trip)
    {
        decimal confidence = 0;

        // Email match (essential)
        if (transaction.Email != trip.Email)
            return 0;

        // Date proximity scoring (40 points max)
        var daysDifference = Math.Abs((transaction.TransactionDate - trip.StartDate).Days);
        if (daysDifference <= 1) confidence += 40;
        else if (daysDifference <= 3) confidence += 30;
        else if (daysDifference <= 5) confidence += 20;
        else if (daysDifference <= 7) confidence += 10;

        // External trip ID match (30 points)
        if (!string.IsNullOrEmpty(transaction.SourceTripId) && 
            !string.IsNullOrEmpty(trip.TripName) &&
            trip.TripName.Contains(transaction.SourceTripId, StringComparison.OrdinalIgnoreCase))
        {
            confidence += 30;
        }

        // Category-based scoring (20 points max)
        var category = await _context.Categories.FindAsync(transaction.CategoryId);
        if (category != null)
        {
            switch (category.Name.ToLower())
            {
                case "airfare":
                    confidence += 20; // Strong trip indicator
                    break;
                case "lodging":
                    confidence += 15; // Strong trip indicator
                    break;
                case "transportation":
                case "meals":
                    confidence += 10; // Moderate trip indicator
                    break;
                case "communication":
                case "other":
                    confidence += 5; // Weak trip indicator
                    break;
            }
        }

        // Booking information match (10 points)
        if (transaction.BookingStartDate.HasValue && 
            Math.Abs((transaction.BookingStartDate.Value - trip.StartDate).Days) <= 1)
        {
            confidence += 10;
        }

        return Math.Min(confidence, 100); // Cap at 100%
    }

    public async Task<MatchingStatistics> GetMatchingStatisticsAsync()
    {
        var totalTransactions = await _context.Transactions.CountAsync();
        var linkedTransactions = await _context.Transactions.CountAsync(t => t.TripId != null);
        var unlinkedTransactions = totalTransactions - linkedTransactions;
        
        var totalTrips = await _context.Trips.CountAsync();
        var tripsWithTransactions = await _context.Trips
            .CountAsync(t => _context.Transactions.Any(tr => tr.TripId == t.TripId));
        var tripsWithoutTransactions = totalTrips - tripsWithTransactions;

        var linkingPercentage = totalTransactions > 0 ? (decimal)linkedTransactions / totalTransactions * 100 : 0;

        var pendingSuggestions = (await GetAutoMatchingSuggestionsAsync()).Count();

        return new MatchingStatistics
        {
            TotalTransactions = totalTransactions,
            LinkedTransactions = linkedTransactions,
            UnlinkedTransactions = unlinkedTransactions,
            TotalTrips = totalTrips,
            TripsWithTransactions = tripsWithTransactions,
            TripsWithoutTransactions = tripsWithoutTransactions,
            LinkingPercentage = Math.Round(linkingPercentage, 1),
            PendingSuggestions = pendingSuggestions
        };
    }

    private async Task<TripMatchingSuggestion> GenerateMatchingSuggestionForTripAsync(Trip trip)
    {
        var potentialTransactions = await GetTransactionsForMatchingAsync(trip.TripId);
        var transactionMatches = new List<TransactionMatch>();
        
        foreach (var transaction in potentialTransactions)
        {
            var confidence = await CalculateMatchingConfidenceAsync(transaction, trip);
            
            if (confidence >= 30) // Minimum confidence threshold
            {
                var category = await _context.Categories.FindAsync(transaction.CategoryId);
                
                transactionMatches.Add(new TransactionMatch
                {
                    TransactionId = transaction.TransactionId,
                    TransactionDate = transaction.TransactionDate,
                    Category = category?.Name ?? "Unknown",
                    Vendor = transaction.Vendor ?? string.Empty,
                    Amount = transaction.Amount,
                    Currency = transaction.Currency ?? string.Empty,
                    AmountUSD = transaction.AmountUSD ?? 0,
                    Confidence = confidence,
                    MatchingReason = GenerateMatchingReason(confidence, transaction, trip),
                    IsAlreadyLinked = transaction.TripId != null
                });
            }
        }

        var overallConfidence = transactionMatches.Any() 
            ? (decimal)transactionMatches.Average(tm => tm.Confidence)
            : 0;

        return new TripMatchingSuggestion
        {
            TripId = trip.TripId,
            TripName = trip.TripName,
            Email = trip.Email,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            SuggestedTransactions = transactionMatches.OrderByDescending(tm => tm.Confidence),
            OverallConfidence = Math.Round(overallConfidence, 1),
            TotalTransactions = transactionMatches.Count,
            TotalAmount = transactionMatches.Sum(tm => tm.AmountUSD)
        };
    }

    private string GenerateMatchingReason(decimal confidence, Transaction transaction, Trip trip)
    {
        var reasons = new List<string>();

        var daysDifference = Math.Abs((transaction.TransactionDate - trip.StartDate).Days);
        if (daysDifference <= 1)
            reasons.Add("Date matches trip");
        else if (daysDifference <= 3)
            reasons.Add("Date within trip period");
        else if (daysDifference <= 5)
            reasons.Add("Date close to trip");

        if (!string.IsNullOrEmpty(transaction.SourceTripId))
            reasons.Add("External trip ID available");

        if (transaction.BookingStartDate.HasValue)
            reasons.Add("Booking date matches");

        if (confidence >= 80)
            return "High confidence: " + string.Join(", ", reasons);
        else if (confidence >= 60)
            return "Medium confidence: " + string.Join(", ", reasons);
        else
            return "Low confidence: " + string.Join(", ", reasons);
    }
}