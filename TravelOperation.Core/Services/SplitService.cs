using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Interfaces;
using TravelOperation.Core.Models;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;
using System.Text.RegularExpressions;

namespace TravelOperation.Core.Services;

public class SplitService : ISplitService
{
    private readonly TravelDbContext _context;
    private readonly IAuditService _auditService;

    public SplitService(TravelDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<IEnumerable<SplitSuggestion>> GetSplitSuggestionsAsync()
    {
        var suggestions = new List<SplitSuggestion>();

        // Find transactions that might need splitting
        var candidateTransactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => !t.IsSplit && // Not already split
                       string.IsNullOrEmpty(t.OriginalTransactionId) && // Not a split transaction
                       (t.AmountUSD ?? 0) > 50 && // Significant amount
                       (!string.IsNullOrEmpty(t.Participants) || // Has participants
                        t.Category!.Name == "Client entertainment" || // Client entertainment category
                        t.Category!.Name == "Meals")) // Meals category
            .OrderByDescending(t => t.AmountUSD)
            .Take(50) // Limit for performance
            .ToListAsync();

        foreach (var transaction in candidateTransactions)
        {
            var suggestion = await GenerateSplitSuggestionAsync(transaction);
            if (suggestion != null)
            {
                suggestions.Add(suggestion);
            }
        }

        return suggestions.OrderByDescending(s => s.ConfidenceScore);
    }

    public async Task<SplitSuggestion?> GetSplitSuggestionAsync(string transactionId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

        if (transaction == null)
            return null;

        return await GenerateSplitSuggestionAsync(transaction);
    }

    public async Task<bool> ApplySplitAsync(string transactionId, List<SplitItem> splitItems, string userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var originalTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (originalTransaction == null)
                return false;

            // Validate split amounts
            var totalSplitAmount = splitItems.Sum(s => s.AmountUSD);
            if (Math.Abs(totalSplitAmount - (originalTransaction.AmountUSD ?? 0)) > 0.01m)
                return false; // Amounts don't match

            // Mark original transaction as split
            var oldTransaction = originalTransaction.Clone();
            originalTransaction.IsSplit = true;
            originalTransaction.ModifiedAt = DateTime.UtcNow;
            originalTransaction.ModifiedBy = userId;

            // Create split transactions
            var splitTransactions = new List<Transaction>();
            for (int i = 0; i < splitItems.Count; i++)
            {
                var splitItem = splitItems[i];
                var splitTransaction = new Transaction
                {
                    TransactionId = $"{transactionId}_SPLIT_{i + 1}",
                    OriginalTransactionId = transactionId,
                    Source = originalTransaction.Source,
                    Email = splitItem.Email,
                    TransactionDate = originalTransaction.TransactionDate,
                    AuthorizationDate = originalTransaction.AuthorizationDate,
                    TransactionType = originalTransaction.TransactionType,
                    CategoryId = splitItem.CategoryId,
                    Vendor = originalTransaction.Vendor,
                    MerchantCategory = originalTransaction.MerchantCategory,
                    Address = originalTransaction.Address,
                    Currency = originalTransaction.Currency,
                    Amount = splitItem.Amount,
                    AmountUSD = splitItem.AmountUSD,
                    ExchangeRate = originalTransaction.ExchangeRate,
                    Participants = splitItem.Email,
                    DocumentUrl = originalTransaction.DocumentUrl,
                    Notes = $"Split from {transactionId}. {splitItem.Notes}",
                    TripId = originalTransaction.TripId,
                    IsSplit = false,
                    DataValidation = true,
                    ParticipantsValidated = !splitItem.IsExternal,
                    IsValid = !splitItem.IsExternal,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedBy = userId
                };

                splitTransactions.Add(splitTransaction);
            }

            _context.Transactions.AddRange(splitTransactions);
            await _context.SaveChangesAsync();

            // Log audit entries
            await _auditService.LogActionAsync(userId, "Split", "Transactions", transactionId, 
                oldTransaction, originalTransaction, $"Split into {splitItems.Count} transactions");

            foreach (var splitTxn in splitTransactions)
            {
                await _auditService.LogActionAsync(userId, "Create", "Transactions", splitTxn.TransactionId,
                    null, splitTxn, $"Created from split of {transactionId}");
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> RejectSplitSuggestionAsync(string transactionId, string userId)
    {
        // For now, we'll just log this action
        // In a full implementation, you might want to store rejected suggestions
        await _auditService.LogActionAsync(userId, "Reject", "SplitSuggestions", transactionId,
            null, null, "Rejected split suggestion");
        return true;
    }

    public async Task<IEnumerable<Transaction>> GetSplitTransactionsAsync(string originalTransactionId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.OriginalTransactionId == originalTransactionId)
            .OrderBy(t => t.TransactionId)
            .ToListAsync();
    }

    public async Task<bool> UndoSplitAsync(string originalTransactionId, string userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var originalTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == originalTransactionId);

            if (originalTransaction == null || !originalTransaction.IsSplit)
                return false;

            var splitTransactions = await _context.Transactions
                .Where(t => t.OriginalTransactionId == originalTransactionId)
                .ToListAsync();

            // Remove split transactions
            _context.Transactions.RemoveRange(splitTransactions);

            // Restore original transaction
            var oldTransaction = originalTransaction.Clone();
            originalTransaction.IsSplit = false;
            originalTransaction.ModifiedAt = DateTime.UtcNow;
            originalTransaction.ModifiedBy = userId;

            await _context.SaveChangesAsync();

            // Log audit entries
            await _auditService.LogActionAsync(userId, "Undo Split", "Transactions", originalTransactionId,
                oldTransaction, originalTransaction, $"Undid split, removed {splitTransactions.Count} split transactions");

            foreach (var splitTxn in splitTransactions)
            {
                await _auditService.LogActionAsync(userId, "Delete", "Transactions", splitTxn.TransactionId,
                    splitTxn, null, $"Deleted as part of undo split for {originalTransactionId}");
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<SplitStatistics> GetSplitStatisticsAsync()
    {
        var splitTransactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => !string.IsNullOrEmpty(t.OriginalTransactionId))
            .ToListAsync();

        var originalTransactions = await _context.Transactions
            .Where(t => t.IsSplit)
            .ToListAsync();

        var auditLogs = await _context.AuditLogs
            .Where(a => a.Action == "Reject" && a.LinkedTable == "SplitSuggestions")
            .ToListAsync();

        var manualSplitLogs = await _context.AuditLogs
            .Where(a => a.Action == "Split" && a.LinkedTable == "Transactions")
            .ToListAsync();

        var categoryBreakdown = splitTransactions
            .GroupBy(t => new { t.CategoryId, t.Category!.Name })
            .Select(g => new CategorySplitInfo
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                SplitCount = g.Count(),
                TotalAmount = g.Sum(t => t.AmountUSD ?? 0)
            })
            .ToList();

        // Calculate participant count mode
        var participantCounts = originalTransactions
            .Where(t => !string.IsNullOrEmpty(t.Participants))
            .Select(t => t.Participants!.Split(',').Length)
            .ToList();

        var mostCommonParticipantCount = participantCounts.Any() 
            ? participantCounts.GroupBy(x => x).OrderByDescending(g => g.Count()).First().Key 
            : 0;

        return new SplitStatistics
        {
            TotalSuggestions = await CountPotentialSplitSuggestionsAsync(),
            AppliedSplits = originalTransactions.Count,
            RejectedSuggestions = auditLogs.Count,
            ManualSplits = manualSplitLogs.Count,
            TotalSplitAmount = splitTransactions.Sum(t => t.AmountUSD ?? 0),
            AverageSplitAmount = splitTransactions.Any() ? splitTransactions.Average(t => t.AmountUSD ?? 0) : 0,
            MostCommonParticipantCount = mostCommonParticipantCount,
            CategoryBreakdown = categoryBreakdown
        };
    }

    public async Task<bool> CreateManualSplitAsync(string transactionId, List<SplitItem> splitItems, string userId)
    {
        return await ApplySplitAsync(transactionId, splitItems, userId);
    }

    private async Task<SplitSuggestion?> GenerateSplitSuggestionAsync(Transaction transaction)
    {
        var suggestion = new SplitSuggestion
        {
            TransactionId = transaction.TransactionId,
            Transaction = transaction,
            TotalAmount = transaction.AmountUSD ?? 0,
            CreatedAt = DateTime.UtcNow
        };

        var participants = new List<string>();
        var reason = "";
        var confidenceScore = 0;

        // Parse participants from transaction
        if (!string.IsNullOrEmpty(transaction.Participants))
        {
            participants = transaction.Participants
                .Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();
        }

        // Analysis for different scenarios
        if (transaction.Category?.Name == "Client entertainment")
        {
            if (participants.Count > 1)
            {
                reason = "Client entertainment with multiple participants detected";
                confidenceScore = 85;
            }
            else if (transaction.AmountUSD > 100)
            {
                reason = "High-value client entertainment transaction";
                confidenceScore = 60;
                // Suggest asking for participants
                participants.Add(transaction.Email);
                participants.Add("external@customer.com");
            }
        }
        else if (transaction.Category?.Name == "Meals")
        {
            if (participants.Count > 1)
            {
                reason = "Shared meal with multiple participants";
                confidenceScore = 75;
            }
            else if (transaction.AmountUSD > 80)
            {
                reason = "High-value meal transaction likely shared";
                confidenceScore = 65;
                participants.Add(transaction.Email);
                participants.Add("colleague@company.com");
            }
        }
        else if (participants.Count > 1)
        {
            reason = "Multiple participants detected in transaction";
            confidenceScore = 70;
        }

        // Only suggest if we have confidence > 50
        if (confidenceScore < 50)
            return null;

        suggestion.SuggestedParticipants = participants;
        suggestion.SuggestedReason = reason;
        suggestion.ConfidenceScore = confidenceScore;

        // Generate suggested splits
        if (participants.Count > 1)
        {
            var splitAmount = suggestion.TotalAmount / participants.Count;
            var splitAmountOriginal = transaction.Amount / participants.Count;

            suggestion.SuggestedSplits = participants.Select((email, index) => new SplitItem
            {
                Email = email,
                Name = GetDisplayNameForEmail(email),
                Amount = splitAmountOriginal,
                AmountUSD = splitAmount,
                CategoryId = transaction.CategoryId,
                IsExternal = !IsInternalEmail(email)
            }).ToList();
        }

        return suggestion;
    }

    private async Task<int> CountPotentialSplitSuggestionsAsync()
    {
        return await _context.Transactions
            .Where(t => !t.IsSplit &&
                       string.IsNullOrEmpty(t.OriginalTransactionId) &&
                       (t.AmountUSD ?? 0) > 50 &&
                       (!string.IsNullOrEmpty(t.Participants) ||
                        t.Category!.Name == "Client entertainment" ||
                        t.Category!.Name == "Meals"))
            .CountAsync();
    }

    private string GetDisplayNameForEmail(string email)
    {
        if (email.Contains("@"))
        {
            return email.Split('@')[0].Replace(".", " ").Replace("_", " ");
        }
        return email;
    }

    private bool IsInternalEmail(string email)
    {
        // Check if it's an internal email based on domain or employee list
        var internalDomains = new[] { "company.com", "wsc.com", "yourcompany.com" };
        return internalDomains.Any(domain => email.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
    }
}

// Extension method for cloning transactions
public static class TransactionExtensions
{
    public static Transaction Clone(this Transaction transaction)
    {
        return new Transaction
        {
            TransactionId = transaction.TransactionId,
            OriginalTransactionId = transaction.OriginalTransactionId,
            Source = transaction.Source,
            Email = transaction.Email,
            TransactionDate = transaction.TransactionDate,
            AuthorizationDate = transaction.AuthorizationDate,
            TransactionType = transaction.TransactionType,
            CategoryId = transaction.CategoryId,
            Vendor = transaction.Vendor,
            MerchantCategory = transaction.MerchantCategory,
            Address = transaction.Address,
            SourceTripId = transaction.SourceTripId,
            BookingId = transaction.BookingId,
            BookingStatusId = transaction.BookingStatusId,
            BookingStartDate = transaction.BookingStartDate,
            BookingEndDate = transaction.BookingEndDate,
            BookingTypeId = transaction.BookingTypeId,
            Policy = transaction.Policy,
            Currency = transaction.Currency,
            Amount = transaction.Amount,
            AmountUSD = transaction.AmountUSD,
            ExchangeRate = transaction.ExchangeRate,
            Participants = transaction.Participants,
            DocumentUrl = transaction.DocumentUrl,
            Notes = transaction.Notes,
            TripId = transaction.TripId,
            IsSplit = transaction.IsSplit,
            DataValidation = transaction.DataValidation,
            ParticipantsValidated = transaction.ParticipantsValidated,
            IsValid = transaction.IsValid,
            CreatedAt = transaction.CreatedAt,
            ModifiedAt = transaction.ModifiedAt,
            ModifiedBy = transaction.ModifiedBy
        };
    }
}