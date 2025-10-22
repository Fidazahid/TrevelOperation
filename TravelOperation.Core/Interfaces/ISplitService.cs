using TravelOperation.Core.Models;
using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Interfaces;

public interface ISplitService
{
    Task<IEnumerable<SplitSuggestion>> GetSplitSuggestionsAsync();
    Task<SplitSuggestion?> GetSplitSuggestionAsync(string transactionId);
    Task<bool> ApplySplitAsync(string transactionId, List<SplitItem> splitItems, string userId);
    Task<bool> RejectSplitSuggestionAsync(string transactionId, string userId);
    Task<IEnumerable<Transaction>> GetSplitTransactionsAsync(string originalTransactionId);
    Task<bool> UndoSplitAsync(string originalTransactionId, string userId);
    Task<SplitStatistics> GetSplitStatisticsAsync();
    Task<bool> CreateManualSplitAsync(string transactionId, List<SplitItem> splitItems, string userId);
}

public class SplitSuggestion
{
    public string TransactionId { get; set; } = string.Empty;
    public Transaction Transaction { get; set; } = null!;
    public List<string> SuggestedParticipants { get; set; } = new();
    public List<SplitItem> SuggestedSplits { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string SuggestedReason { get; set; } = string.Empty;
    public int ConfidenceScore { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SplitItem
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountUSD { get; set; }
    public int CategoryId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
}

public class SplitStatistics
{
    public int TotalSuggestions { get; set; }
    public int AppliedSplits { get; set; }
    public int RejectedSuggestions { get; set; }
    public int ManualSplits { get; set; }
    public decimal TotalSplitAmount { get; set; }
    public decimal AverageSplitAmount { get; set; }
    public int MostCommonParticipantCount { get; set; }
    public List<CategorySplitInfo> CategoryBreakdown { get; set; } = new();
}

public class CategorySplitInfo
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int SplitCount { get; set; }
    public decimal TotalAmount { get; set; }
}