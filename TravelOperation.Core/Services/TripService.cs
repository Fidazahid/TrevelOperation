using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Services;

public class TripService : ITripService
{
    private readonly TravelDbContext _context;
    private readonly IAuditService _auditService;
    private readonly IAuthenticationService _authService;

    public TripService(
        TravelDbContext context, 
        IAuditService auditService, 
        IAuthenticationService authService)
    {
        _context = context;
        _auditService = auditService;
        _authService = authService;
    }

    public async Task<IEnumerable<Trip>> GetAllTripsAsync()
    {
        var query = _context.Trips
            .Include(t => t.Purpose)
            .Include(t => t.TripType)
            .Include(t => t.Status)
            .Include(t => t.ValidationStatus)
            .Include(t => t.Owner)
            .Include(t => t.Transactions)
            .AsQueryable();

        // Apply role-based filtering
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser != null)
        {
            if (currentUser.Role == "Employee")
            {
                // Employees see only their own trips
                query = query.Where(t => t.Email == currentUser.Email);
            }
            else if (currentUser.Role == "Owner")
            {
                // Owners see trips for their department
                var departmentEmails = await GetDepartmentEmailsAsync(currentUser.Department);
                query = query.Where(t => departmentEmails.Contains(t.Email));
            }
            // Finance users see all trips (no filter)
        }

        return await query
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    private async Task<List<string>> GetDepartmentEmailsAsync(string department)
    {
        // For now, just return empty list to avoid circular dependency
        // In production, this would query a separate users/headcount table
        // TODO: Implement proper department email lookup without circular dependency
        await Task.CompletedTask;
        return new List<string>();
    }

    public async Task<Trip?> GetTripByIdAsync(int tripId)
    {
        return await _context.Trips
            .Include(t => t.Purpose)
            .Include(t => t.TripType)
            .Include(t => t.Status)
            .Include(t => t.ValidationStatus)
            .Include(t => t.Owner)
            .Include(t => t.Transactions)
                .ThenInclude(tr => tr.Category)
            .Include(t => t.Transactions)
                .ThenInclude(tr => tr.Source)
            .Include(t => t.Transactions)
                .ThenInclude(tr => tr.CabinClass)
            .FirstOrDefaultAsync(t => t.TripId == tripId);
    }

    public async Task<IEnumerable<Trip>> GetTripsByEmailAsync(string email)
    {
        return await _context.Trips
            .Include(t => t.Purpose)
            .Include(t => t.TripType)
            .Include(t => t.Status)
            .Include(t => t.ValidationStatus)
            .Include(t => t.Owner)
            .Where(t => t.Email == email)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trip>> GetTripsByOwnerAsync(int ownerId)
    {
        return await _context.Trips
            .Include(t => t.Purpose)
            .Include(t => t.TripType)
            .Include(t => t.Status)
            .Include(t => t.ValidationStatus)
            .Include(t => t.Owner)
            .Where(t => t.OwnerId == ownerId)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trip>> GetTripsReadyForValidationAsync()
    {
        return await _context.Trips
            .Include(t => t.Purpose)
            .Include(t => t.TripType)
            .Include(t => t.Status)
            .Include(t => t.ValidationStatus)
            .Include(t => t.Owner)
            .Include(t => t.Transactions)
            .Where(t => t.ValidationStatus.Name == "Ready to validate")
            .OrderBy(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<Trip> CreateTripAsync(Trip trip)
    {
        trip.Duration = await CalculateTripDurationAsync(trip.StartDate, trip.EndDate);
        trip.CreatedAt = DateTime.UtcNow;
        trip.ModifiedAt = DateTime.UtcNow;
        
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Create", "Trips", trip.TripId.ToString(), null, trip);
        
        return trip;
    }

    public async Task UpdateTripAsync(Trip trip)
    {
        var existing = await GetTripByIdAsync(trip.TripId);
        if (existing == null)
            throw new ArgumentException($"Trip with ID {trip.TripId} not found");

        trip.Duration = await CalculateTripDurationAsync(trip.StartDate, trip.EndDate);
        trip.ModifiedAt = DateTime.UtcNow;
        
        _context.Entry(existing).CurrentValues.SetValues(trip);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Edit", "Trips", trip.TripId.ToString(), existing, trip);
    }

    public async Task DeleteTripAsync(int tripId)
    {
        var trip = await GetTripByIdAsync(tripId);
        if (trip == null)
            throw new ArgumentException($"Trip with ID {tripId} not found");

        // Unlink all transactions from this trip
        var linkedTransactions = await _context.Transactions
            .Where(t => t.TripId == tripId)
            .ToListAsync();

        foreach (var transaction in linkedTransactions)
        {
            transaction.TripId = null;
        }

        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Delete", "Trips", tripId.ToString(), trip, null);
    }

    public async Task<IEnumerable<Trip>> SuggestTripsFromTransactionsAsync()
    {
        // Algorithm: Group unlinked transactions by email and date proximity (±2 days)
        // Look for airfare/lodging to identify trips
        
        var unlinkedTransactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.TripId == null && 
                       (t.Category.Name == "Airfare" || t.Category.Name == "Lodging"))
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();

        var suggestions = new List<Trip>();
        var processedTransactions = new HashSet<string>();

        foreach (var transaction in unlinkedTransactions)
        {
            if (processedTransactions.Contains(transaction.TransactionId))
                continue;

            // Find related transactions within ±2 days for the same email
            var relatedTransactions = unlinkedTransactions
                .Where(t => t.Email == transaction.Email &&
                           Math.Abs((t.TransactionDate - transaction.TransactionDate).TotalDays) <= 2 &&
                           !processedTransactions.Contains(t.TransactionId))
                .ToList();

            if (relatedTransactions.Count >= 2) // At least 2 transactions to suggest a trip
            {
                var startDate = relatedTransactions.Min(t => t.TransactionDate);
                var endDate = relatedTransactions.Max(t => t.TransactionDate);
                
                // Try to determine destination from transaction addresses
                var destination = ExtractDestinationFromTransactions(relatedTransactions);
                
                var suggestedTrip = new Trip
                {
                    TripName = $"Trip to {destination}",
                    Email = transaction.Email,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = await CalculateTripDurationAsync(startDate, endDate),
                    Country1 = destination.Split(',')[0].Trim(),
                    City1 = destination.Split(',').Length > 1 ? destination.Split(',')[1].Trim() : "",
                    PurposeId = 1, // Default to "Business trip"
                    TripTypeId = 1, // Default to "Domestic" - can be updated based on logic
                    StatusId = 2, // "Upcoming" or "Completed" based on dates
                    ValidationStatusId = 1, // "Not ready to validate"
                    IsManual = false,
                    OwnerId = 1 // Default owner - should be determined by email domain logic
                };

                suggestions.Add(suggestedTrip);
                
                // Mark transactions as processed
                foreach (var rt in relatedTransactions)
                {
                    processedTransactions.Add(rt.TransactionId);
                }
            }
        }

        return suggestions;
    }

    private string ExtractDestinationFromTransactions(List<Transaction> transactions)
    {
        // Simple logic to extract destination from addresses
        // In a real implementation, this would be more sophisticated
        var addresses = transactions
            .Where(t => !string.IsNullOrEmpty(t.Address))
            .Select(t => t.Address)
            .ToList();

        if (addresses.Any())
        {
            // Try to find common city/country from addresses
            var mostCommon = addresses
                .GroupBy(a => a)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key;

            if (!string.IsNullOrEmpty(mostCommon))
            {
                // Extract city and country from address (simplified)
                var parts = mostCommon.Split(',');
                if (parts.Length >= 2)
                {
                    return $"{parts[^1].Trim()}, {parts[^2].Trim()}"; // Country, City
                }
                return mostCommon;
            }
        }

        return "Unknown Destination";
    }

    public async Task ValidateTripAsync(int tripId)
    {
        var trip = await GetTripByIdAsync(tripId);
        if (trip == null)
            throw new ArgumentException($"Trip with ID {tripId} not found");

        // Set validation status to "Validated"
        trip.ValidationStatusId = 3; // "Validated"
        trip.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Validate", "Trips", tripId.ToString(), 
            new { ValidationStatusId = trip.ValidationStatusId }, new { ValidationStatusId = 3 });
    }

    public async Task<decimal> CalculateTotalSpendAsync(int tripId)
    {
        var trip = await GetTripByIdAsync(tripId);
        if (trip?.Transactions == null)
            return 0;

        return trip.Transactions
            .Where(t => t.AmountUSD.HasValue)
            .Sum(t => t.AmountUSD.Value);
    }

    public async Task<Dictionary<string, decimal>> CalculateCategorySpendAsync(int tripId)
    {
        var trip = await GetTripByIdAsync(tripId);
        if (trip?.Transactions == null)
            return new Dictionary<string, decimal>();

        return trip.Transactions
            .Where(t => t.AmountUSD.HasValue && t.Category != null)
            .GroupBy(t => t.Category.Name)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.AmountUSD!.Value));
    }

    public async Task<decimal> CalculateTaxExposureAsync(int tripId)
    {
        var trip = await GetTripByIdAsync(tripId);
        if (trip?.Transactions == null)
            return 0;

        // Get tax rules for this trip
        var taxRule = await _context.TaxRules
            .FirstOrDefaultAsync(tr => tr.FiscalYear == trip.StartDate.Year &&
                                      tr.Country == trip.Country1);

        if (taxRule == null)
            return 0;

        var categorySpend = await CalculateCategorySpendAsync(tripId);
        var duration = trip.Duration > 0 ? trip.Duration : 1;
        
        decimal taxExposure = 0;

        // Calculate meals exposure
        if (categorySpend.ContainsKey("Meals") && taxRule.MealsCap.HasValue)
        {
            var mealsPerDay = categorySpend["Meals"] / duration;
            if (mealsPerDay > taxRule.MealsCap.Value)
            {
                taxExposure += duration * (mealsPerDay - taxRule.MealsCap.Value);
            }
        }

        // Calculate lodging exposure
        if (categorySpend.ContainsKey("Lodging") && taxRule.LodgingCap.HasValue)
        {
            var lodgingPerNight = categorySpend["Lodging"] / duration;
            if (lodgingPerNight > taxRule.LodgingCap.Value)
            {
                taxExposure += duration * (lodgingPerNight - taxRule.LodgingCap.Value);
            }
        }

        // Check airfare cabin class
        var airfareTransactions = trip.Transactions
            .Where(t => t.Category?.Name == "Airfare" && t.CabinClass != null)
            .ToList();

        if (airfareTransactions.Any(t => t.CabinClass?.Name is "First" or "Business"))
        {
            // Flag for additional scrutiny - specific tax implications would be defined by business rules
            // For now, we'll add a base exposure for premium cabin classes
            taxExposure += categorySpend.GetValueOrDefault("Airfare", 0) * 0.1m; // 10% additional exposure
        }

        return taxExposure;
    }

    public async Task<int> CalculateTripDurationAsync(DateTime startDate, DateTime endDate)
    {
        // Calculate inclusive days (start date to end date inclusive)
        var duration = (endDate.Date - startDate.Date).Days + 1;
        return Math.Max(1, duration); // Minimum 1 day
    }
}