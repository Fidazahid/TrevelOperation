using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

public interface IMatchingService
{
    Task<IEnumerable<TripMatchingSuggestion>> GetAutoMatchingSuggestionsAsync();
    Task<IEnumerable<TripMatchingSuggestion>> GetMatchingSuggestionsForTripAsync(int tripId);
    Task<IEnumerable<Transaction>> GetUnlinkedTransactionsAsync(string email, DateTime startDate, DateTime endDate);
    Task<bool> LinkTransactionToTripAsync(string transactionId, int tripId, string userId);
    Task<bool> UnlinkTransactionFromTripAsync(string transactionId, string userId);
    Task<IEnumerable<Transaction>> GetTransactionsForMatchingAsync(int tripId, int daysTolerance = 5);
    Task<decimal> CalculateMatchingConfidenceAsync(Transaction transaction, Trip trip);
    Task<MatchingStatistics> GetMatchingStatisticsAsync();
}

public class TripMatchingSuggestion
{
    public int TripId { get; set; }
    public string TripName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<TransactionMatch> SuggestedTransactions { get; set; } = new List<TransactionMatch>();
    public decimal OverallConfidence { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalAmount { get; set; }
}

public class TransactionMatch
{
    public string TransactionId { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal AmountUSD { get; set; }
    public decimal Confidence { get; set; }
    public string MatchingReason { get; set; } = string.Empty;
    public bool IsAlreadyLinked { get; set; }
}

public class MatchingStatistics
{
    public int TotalTransactions { get; set; }
    public int LinkedTransactions { get; set; }
    public int UnlinkedTransactions { get; set; }
    public int TotalTrips { get; set; }
    public int TripsWithTransactions { get; set; }
    public int TripsWithoutTransactions { get; set; }
    public decimal LinkingPercentage { get; set; }
    public int PendingSuggestions { get; set; }
}