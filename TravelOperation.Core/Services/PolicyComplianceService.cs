using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Services;

public class PolicyComplianceService : IPolicyComplianceService
{
    private readonly TravelDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private PolicyRules _cachedRules;
    private DateTime _cacheExpiry;

    public PolicyComplianceService(TravelDbContext context, IAuditService auditService, INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
        _cachedRules = new PolicyRules();
        _cacheExpiry = DateTime.MinValue;
    }

    public async Task<PolicyComplianceResult> CheckComplianceAsync(Transaction transaction)
    {
        var result = new PolicyComplianceResult
        {
            TransactionId = transaction.TransactionId,
            IsCompliant = true,
            CheckedAt = DateTime.UtcNow
        };

        var rules = await GetPolicyRulesAsync();

        // Check all policy rules
        await CheckMealsPolicyAsync(transaction, rules, result);
        await CheckLodgingPolicyAsync(transaction, rules, result);
        await CheckAirfarePolicyAsync(transaction, rules, result);
        await CheckClientEntertainmentPolicyAsync(transaction, rules, result);
        await CheckDocumentationPolicyAsync(transaction, rules, result);
        await CheckCategorizationPolicyAsync(transaction, rules, result);
        await CheckCurrencyPolicyAsync(transaction, rules, result);

        result.IsCompliant = !result.Violations.Any();

        // Send notification if there are policy violations
        if (!result.IsCompliant && !string.IsNullOrEmpty(transaction.Email))
        {
            try
            {
                var violationSummary = string.Join(", ", result.Violations.Select(v => v.Description));
                await _notificationService.NotifyPolicyViolationAsync(
                    transaction.Email,
                    transaction.TransactionId,
                    violationSummary,
                    $"/transactions?id={transaction.TransactionId}"
                );
            }
            catch (Exception ex)
            {
                // Log but don't fail compliance check if notification fails
                Console.WriteLine($"Failed to send policy violation notification: {ex.Message}");
            }
        }

        return result;
    }

    public async Task<List<PolicyComplianceResult>> CheckMultipleComplianceAsync(IEnumerable<Transaction> transactions)
    {
        var results = new List<PolicyComplianceResult>();
        
        foreach (var transaction in transactions)
        {
            var result = await CheckComplianceAsync(transaction);
            results.Add(result);
        }

        return results;
    }

    public async Task<IEnumerable<Transaction>> GetNonCompliantTransactionsAsync()
    {
        var allTransactions = await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.CabinClass)
            .ToListAsync();

        var results = await CheckMultipleComplianceAsync(allTransactions);
        var nonCompliantIds = results.Where(r => !r.IsCompliant).Select(r => r.TransactionId).ToList();

        return allTransactions.Where(t => nonCompliantIds.Contains(t.TransactionId));
    }

    public async Task FlagTransactionAsync(string transactionId, string reason, PolicyViolationType violationType)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction == null)
            throw new ArgumentException($"Transaction {transactionId} not found");

        transaction.DataValidation = true;
        transaction.Notes = string.IsNullOrEmpty(transaction.Notes) 
            ? $"Policy Violation: {reason}" 
            : $"{transaction.Notes}\nPolicy Violation: {reason}";
        
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Flag", "Transactions", transactionId, 
            null, new { Reason = reason, ViolationType = violationType.ToString() });
    }

    public async Task ApproveExceptionAsync(string transactionId, string approvedBy, string approvalReason)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction == null)
            throw new ArgumentException($"Transaction {transactionId} not found");

        transaction.DataValidation = false;
        transaction.IsValid = true;
        transaction.Notes = string.IsNullOrEmpty(transaction.Notes)
            ? $"Exception Approved by {approvedBy}: {approvalReason}"
            : $"{transaction.Notes}\nException Approved by {approvedBy}: {approvalReason}";

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(approvedBy, "ApproveException", "Transactions", transactionId,
            null, new { ApprovedBy = approvedBy, Reason = approvalReason });
    }

    public async Task<PolicyRules> GetPolicyRulesAsync()
    {
        // Check cache
        if (_cacheExpiry > DateTime.UtcNow)
        {
            return _cachedRules;
        }

        // In production, load from database or configuration
        // For now, return default rules
        _cachedRules = new PolicyRules();
        _cacheExpiry = DateTime.UtcNow.AddHours(1);

        await Task.CompletedTask;
        return _cachedRules;
    }

    public async Task UpdatePolicyRulesAsync(PolicyRules rules)
    {
        // In production, save to database or configuration
        _cachedRules = rules;
        _cacheExpiry = DateTime.UtcNow.AddHours(1);

        await _auditService.LogActionAsync("System", "UpdatePolicyRules", "Settings", "PolicyRules",
            null, rules);
    }

    #region Private Policy Check Methods

    private async Task CheckMealsPolicyAsync(Transaction transaction, PolicyRules rules, PolicyComplianceResult result)
    {
        await Task.CompletedTask;
        
        if (transaction.Category?.Name != "Meals")
            return;

        var amountUSD = transaction.AmountUSD ?? 0;

        // Check high-value meals
        if (amountUSD >= rules.HighValueMealThreshold)
        {
            result.Violations.Add(new PolicyViolation
            {
                Type = PolicyViolationType.HighValueMeal,
                Description = $"Meal expense exceeds policy threshold of ${rules.HighValueMealThreshold}",
                Rule = "High-value meals require justification",
                ThresholdValue = rules.HighValueMealThreshold,
                ActualValue = amountUSD,
                Severity = amountUSD >= rules.HighValueMealThreshold * 2 ? PolicySeverity.High : PolicySeverity.Medium,
                RequiresApproval = amountUSD >= rules.HighValueMealThreshold * 2
            });
        }

        // Check if participants are provided for meals requiring them
        if (rules.MealsRequireParticipants && amountUSD >= rules.HighValueMealThreshold)
        {
            if (string.IsNullOrWhiteSpace(transaction.Participants) || !transaction.ParticipantsValidated)
            {
                result.Violations.Add(new PolicyViolation
                {
                    Type = PolicyViolationType.MissingParticipants,
                    Description = "High-value meal requires participant information",
                    Rule = "Meals over threshold require participants list",
                    ThresholdValue = rules.HighValueMealThreshold,
                    ActualValue = amountUSD,
                    Severity = PolicySeverity.Medium,
                    RequiresApproval = false
                });
            }
        }
    }

    private async Task CheckLodgingPolicyAsync(Transaction transaction, PolicyRules rules, PolicyComplianceResult result)
    {
        await Task.CompletedTask;
        
        if (transaction.Category?.Name != "Lodging")
            return;

        var amountUSD = transaction.AmountUSD ?? 0;

        // Check unusually low lodging (possible data error or personal expense)
        if (amountUSD > 0 && amountUSD <= rules.LowValueLodgingThreshold)
        {
            result.Violations.Add(new PolicyViolation
            {
                Type = PolicyViolationType.LowValueLodging,
                Description = $"Lodging amount is unusually low (${amountUSD:F2})",
                Rule = "Low-value lodging requires review to ensure accuracy",
                ThresholdValue = rules.LowValueLodgingThreshold,
                ActualValue = amountUSD,
                Severity = PolicySeverity.Low,
                RequiresApproval = false
            });
        }

        // Check if receipt is required
        if (rules.LodgingRequiresReceipt && string.IsNullOrWhiteSpace(transaction.DocumentUrl))
        {
            result.Violations.Add(new PolicyViolation
            {
                Type = PolicyViolationType.MissingDocumentation,
                Description = "Lodging expense requires receipt/documentation",
                Rule = "All lodging expenses must have receipts",
                Severity = PolicySeverity.High,
                RequiresApproval = false
            });
        }
    }

    private async Task CheckAirfarePolicyAsync(Transaction transaction, PolicyRules rules, PolicyComplianceResult result)
    {
        await Task.CompletedTask;
        
        if (transaction.Category?.Name != "Airfare")
            return;

        // Check premium cabin class
        if (transaction.CabinClass != null && rules.ApprovedPremiumCabinClasses.Any(c => 
            transaction.CabinClass.Name.Contains(c, StringComparison.OrdinalIgnoreCase)))
        {
            if (rules.PremiumCabinRequiresApproval)
            {
                result.Violations.Add(new PolicyViolation
                {
                    Type = PolicyViolationType.PremiumCabinClass,
                    Description = $"Premium cabin class ({transaction.CabinClass.Name}) requires prior approval",
                    Rule = "Business/First class travel requires management approval",
                    Severity = PolicySeverity.High,
                    RequiresApproval = true
                });
            }
        }
    }

    private async Task CheckClientEntertainmentPolicyAsync(Transaction transaction, PolicyRules rules, PolicyComplianceResult result)
    {
        await Task.CompletedTask;
        
        if (transaction.Category?.Name != "Client entertainment")
            return;

        var amountUSD = transaction.AmountUSD ?? 0;

        // Check if participants are required
        if (rules.ClientEntertainmentRequiresParticipants)
        {
            if (string.IsNullOrWhiteSpace(transaction.Participants) || !transaction.ParticipantsValidated)
            {
                result.Violations.Add(new PolicyViolation
                {
                    Type = PolicyViolationType.MissingParticipants,
                    Description = "Client entertainment requires participant information for tax compliance",
                    Rule = "All client entertainment must list participants (internal and external)",
                    Severity = PolicySeverity.High,
                    RequiresApproval = false
                });
            }
        }

        // Check threshold
        if (amountUSD >= rules.ClientEntertainmentThreshold * 3)
        {
            result.Violations.Add(new PolicyViolation
            {
                Type = PolicyViolationType.ExcessiveSpending,
                Description = $"Client entertainment amount (${amountUSD:F2}) exceeds typical spending",
                Rule = $"Client entertainment over ${rules.ClientEntertainmentThreshold * 3} requires review",
                ThresholdValue = rules.ClientEntertainmentThreshold * 3,
                ActualValue = amountUSD,
                Severity = PolicySeverity.Medium,
                RequiresApproval = true
            });
        }
    }

    private async Task CheckDocumentationPolicyAsync(Transaction transaction, PolicyRules rules, PolicyComplianceResult result)
    {
        await Task.CompletedTask;
        
        var amountUSD = Math.Abs(transaction.AmountUSD ?? 0);

        // Check if documentation is required based on amount
        if (amountUSD >= rules.DocumentationRequiredThreshold)
        {
            if (string.IsNullOrWhiteSpace(transaction.DocumentUrl))
            {
                var daysSinceTransaction = (DateTime.UtcNow - transaction.TransactionDate).Days;
                var isOverdue = daysSinceTransaction > rules.DocumentationGracePeriodDays;

                result.Violations.Add(new PolicyViolation
                {
                    Type = PolicyViolationType.MissingDocumentation,
                    Description = isOverdue 
                        ? $"Receipt missing for {daysSinceTransaction} days (grace period: {rules.DocumentationGracePeriodDays} days)"
                        : "Receipt/documentation required",
                    Rule = $"Transactions over ${rules.DocumentationRequiredThreshold} require receipts",
                    ThresholdValue = rules.DocumentationRequiredThreshold,
                    ActualValue = amountUSD,
                    Severity = isOverdue ? PolicySeverity.High : PolicySeverity.Medium,
                    RequiresApproval = false
                });
            }
        }
    }

    private async Task CheckCategorizationPolicyAsync(Transaction transaction, PolicyRules rules, PolicyComplianceResult result)
    {
        await Task.CompletedTask;
        
        if (!rules.UncategorizedRequiresReview)
            return;

        if (transaction.Category?.Name == "Other" || transaction.Category?.Name == "Uncategorized" || 
            transaction.Category == null)
        {
            result.Violations.Add(new PolicyViolation
            {
                Type = PolicyViolationType.UncategorizedTransaction,
                Description = "Transaction requires proper categorization for tax purposes",
                Rule = "All transactions must be properly categorized",
                Severity = PolicySeverity.Medium,
                RequiresApproval = false
            });
        }
    }

    private async Task CheckCurrencyPolicyAsync(Transaction transaction, PolicyRules rules, PolicyComplianceResult result)
    {
        await Task.CompletedTask;
        
        if (!rules.ApprovedCurrencies.Contains(transaction.Currency, StringComparer.OrdinalIgnoreCase))
        {
            result.Violations.Add(new PolicyViolation
            {
                Type = PolicyViolationType.InvalidCurrency,
                Description = $"Currency {transaction.Currency} is not in approved list",
                Rule = $"Approved currencies: {string.Join(", ", rules.ApprovedCurrencies)}",
                Severity = PolicySeverity.Low,
                RequiresApproval = false
            });
        }
    }

    #endregion
}
