using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

public interface IValidationService
{
    /// <summary>
    /// Validates a transaction against business rules
    /// </summary>
    Task<ValidationResult> ValidateTransactionAsync(Transaction transaction);
    
    /// <summary>
    /// Validates a trip against business rules
    /// </summary>
    Task<ValidationResult> ValidateTripAsync(Trip trip);
    
    /// <summary>
    /// Validates multiple transactions
    /// </summary>
    Task<List<ValidationResult>> ValidateTransactionsAsync(List<Transaction> transactions);
    
    /// <summary>
    /// Validates multiple trips
    /// </summary>
    Task<List<ValidationResult>> ValidateTripsAsync(List<Trip> trips);
    
    /// <summary>
    /// Gets all validation rules
    /// </summary>
    Task<List<ValidationRule>> GetValidationRulesAsync();
    
    /// <summary>
    /// Updates validation rules
    /// </summary>
    Task UpdateValidationRulesAsync(List<ValidationRule> rules);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string RuleCode { get; set; } = string.Empty;
    public string Severity { get; set; } = "Error"; // Error, Warning, Info
}

public class ValidationWarning
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string RuleCode { get; set; } = string.Empty;
}

public class ValidationRule
{
    public string RuleCode { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // Transaction, Trip
    public string Field { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Severity { get; set; } = "Error";
}
