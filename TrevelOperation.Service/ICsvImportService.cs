using System.Globalization;
using TravelOperation.Core.Models.Entities;

namespace TrevelOperation.Service;

public interface ICsvImportService
{
    Task<ImportResult> ImportNavanCsvAsync(Stream csvStream, string fileName);
    Task<ImportResult> ImportAgentCsvAsync(Stream csvStream, string fileName);
    Task<ImportResult> ImportManualCsvAsync(Stream csvStream, string fileName);
    Task<List<TransformationRule>> GetTransformationRulesAsync();
    Task SaveTransformationRulesAsync(List<TransformationRule> rules);
}

public class ImportResult
{
    public bool Success { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecordsImported { get; set; }
    public int RecordsSkipped { get; set; }
    public int RecordsWithErrors { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string? FileName { get; set; }
    public DateTime ImportDate { get; set; } = DateTime.UtcNow;
}

// DTO for transformation rules (used by UI layer)
public class TransformationRule
{
    public int RuleId { get; set; }
    public string PolicyPattern { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Priority { get; set; } = 0;
    public bool IsRegex { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}

public class CsvFieldMapping
{
    // Common fields for all import types
    public string? TransactionIdColumn { get; set; }
    public string? EmailColumn { get; set; }
    public string? TransactionDateColumn { get; set; }
    public string? AuthorizationDateColumn { get; set; }
    public string? TransactionTypeColumn { get; set; }
    public string? VendorColumn { get; set; }
    public string? MerchantCategoryColumn { get; set; }
    public string? AddressColumn { get; set; }
    public string? SourceTripIdColumn { get; set; }
    public string? BookingIdColumn { get; set; }
    public string? BookingStartDateColumn { get; set; }
    public string? BookingEndDateColumn { get; set; }
    public string? PolicyColumn { get; set; }
    public string? CurrencyColumn { get; set; }
    public string? AmountColumn { get; set; }
    public string? ExchangeRateColumn { get; set; }
    public string? ParticipantsColumn { get; set; }
    public string? DocumentUrlColumn { get; set; }
    public string? NotesColumn { get; set; }

    // Format settings
    public string DateFormat { get; set; } = "yyyy-MM-dd";
    public string CultureInfo { get; set; } = "en-US";
    public char Delimiter { get; set; } = ',';
    public bool HasHeader { get; set; } = true;
}

public class ImportedTransactionData
{
    public string TransactionId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public string? TransactionType { get; set; }
    public string? Vendor { get; set; }
    public string? MerchantCategory { get; set; }
    public string? Address { get; set; }
    public string? SourceTripId { get; set; }
    public string? BookingId { get; set; }
    public DateTime? BookingStartDate { get; set; }
    public DateTime? BookingEndDate { get; set; }
    public string? Policy { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal Amount { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? Participants { get; set; }
    public string? DocumentUrl { get; set; }
    public string? Notes { get; set; }
    
    // Calculated fields
    public decimal AmountUSD => ExchangeRate.HasValue ? Amount * ExchangeRate.Value : Amount;
    public string DeterminedCategory { get; set; } = "Uncategorized";
    public List<string> ValidationErrors { get; set; } = new();
}