using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

public interface IPolicyComplianceService
{
    /// <summary>
    /// Checks a transaction against all policy rules
    /// </summary>
    Task<PolicyComplianceResult> CheckComplianceAsync(Transaction transaction);
    
    /// <summary>
    /// Checks multiple transactions for compliance
    /// </summary>
    Task<List<PolicyComplianceResult>> CheckMultipleComplianceAsync(IEnumerable<Transaction> transactions);
    
    /// <summary>
    /// Gets all non-compliant transactions
    /// </summary>
    Task<IEnumerable<Transaction>> GetNonCompliantTransactionsAsync();
    
    /// <summary>
    /// Flags a transaction as non-compliant with reason
    /// </summary>
    Task FlagTransactionAsync(string transactionId, string reason, PolicyViolationType violationType);
    
    /// <summary>
    /// Approves an exception for a flagged transaction
    /// </summary>
    Task ApproveExceptionAsync(string transactionId, string approvedBy, string approvalReason);
    
    /// <summary>
    /// Gets policy rules configuration
    /// </summary>
    Task<PolicyRules> GetPolicyRulesAsync();
    
    /// <summary>
    /// Updates policy rules configuration
    /// </summary>
    Task UpdatePolicyRulesAsync(PolicyRules rules);
}

public class PolicyComplianceResult
{
    public string TransactionId { get; set; } = string.Empty;
    public bool IsCompliant { get; set; }
    public List<PolicyViolation> Violations { get; set; } = new();
    public DateTime CheckedAt { get; set; }
}

public class PolicyViolation
{
    public PolicyViolationType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Rule { get; set; } = string.Empty;
    public decimal? ThresholdValue { get; set; }
    public decimal? ActualValue { get; set; }
    public PolicySeverity Severity { get; set; }
    public bool RequiresApproval { get; set; }
}

public enum PolicyViolationType
{
    HighValueMeal,
    LowValueLodging,
    PremiumCabinClass,
    MissingParticipants,
    MissingDocumentation,
    UncategorizedTransaction,
    ExcessiveSpending,
    InvalidCurrency,
    DuplicateTransaction,
    PersonalExpense
}

public enum PolicySeverity
{
    Low,
    Medium,
    High,
    Critical
}

public class PolicyRules
{
    // Meal policies
    public decimal HighValueMealThreshold { get; set; } = 80m;
    public bool MealsRequireParticipants { get; set; } = true;
    
    // Lodging policies
    public decimal LowValueLodgingThreshold { get; set; } = 100m;
    public bool LodgingRequiresReceipt { get; set; } = true;
    
    // Airfare policies
    public bool PremiumCabinRequiresApproval { get; set; } = true;
    public List<string> ApprovedPremiumCabinClasses { get; set; } = new() { "Business", "First" };
    
    // Client Entertainment policies
    public bool ClientEntertainmentRequiresParticipants { get; set; } = true;
    public decimal ClientEntertainmentThreshold { get; set; } = 50m;
    
    // Documentation policies
    public decimal DocumentationRequiredThreshold { get; set; } = 25m;
    public int DocumentationGracePeriodDays { get; set; } = 30;
    
    // General policies
    public bool UncategorizedRequiresReview { get; set; } = true;
    public List<string> ApprovedCurrencies { get; set; } = new() { "USD", "EUR", "ILS", "GBP" };
    public decimal ExcessiveSpendingDailyLimit { get; set; } = 500m;
}
