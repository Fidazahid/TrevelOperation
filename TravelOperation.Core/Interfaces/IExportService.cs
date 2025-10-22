using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Models;

namespace TravelOperation.Core.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportTransactionsToCsvAsync(IEnumerable<Transaction> transactions);
    Task<byte[]> ExportTransactionsToExcelAsync(IEnumerable<Transaction> transactions);
    Task<byte[]> ExportTripsToCsvAsync(IEnumerable<Trip> trips);
    Task<byte[]> ExportTripsToExcelAsync(IEnumerable<Trip> trips);
    Task<byte[]> ExportTravelSpendToCsvAsync(IEnumerable<TravelSpendReportItem> travelSpendData);
    Task<byte[]> ExportTravelSpendToExcelAsync(IEnumerable<TravelSpendReportItem> travelSpendData);
    Task<byte[]> ExportTravelSpendToPdfAsync(IEnumerable<TravelSpendReportItem> travelSpendData, TravelSpendSummary summary);
    Task<byte[]> ExportAuditLogToCsvAsync(IEnumerable<AuditLog> auditLogs);
    Task<byte[]> ExportAuditLogToExcelAsync(IEnumerable<AuditLog> auditLogs);
    Task<byte[]> ExportTaxComplianceReportToCsvAsync(IEnumerable<TaxComplianceReportItem> taxData);
    Task<byte[]> ExportTaxComplianceReportToExcelAsync(IEnumerable<TaxComplianceReportItem> taxData);
    Task<byte[]> ExportTaxComplianceReportToPdfAsync(IEnumerable<TaxComplianceReportItem> taxData);
    Task<byte[]> ExportMonthlyReportToPdfAsync(MonthlyReportData reportData);
    Task<string> GetExportFileName(string reportType, string format, DateTime? reportDate = null);
}

public class TaxComplianceReportItem
{
    public string TripName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Country { get; set; } = string.Empty;
    public decimal MealsAmount { get; set; }
    public decimal LodgingAmount { get; set; }
    public decimal AirfareAmount { get; set; }
    public string CabinClasses { get; set; } = string.Empty;
    public decimal MealsCap { get; set; }
    public decimal LodgingCap { get; set; }
    public decimal MealsExposure { get; set; }
    public decimal LodgingExposure { get; set; }
    public decimal TotalTaxExposure { get; set; }
    public bool HasBusinessClassTravel { get; set; }
}

public class MonthlyReportData
{
    public DateTime ReportMonth { get; set; }
    public string ReportTitle { get; set; } = string.Empty;
    public decimal TotalMoneyIn { get; set; }
    public decimal TotalMoneyOut { get; set; }
    public decimal CurrentTotalCost { get; set; }
    public decimal Budget { get; set; }
    public decimal BudgetRemaining { get; set; }
    public int PlannedHours { get; set; }
    public int ActualHours { get; set; }
    public decimal HoursUsedPercentage { get; set; }
    public decimal BudgetSpentPercentage { get; set; }
    public decimal MilestonesCompletedPercentage { get; set; }
    public decimal OverallProgressPercentage { get; set; }
    public List<MonthlyExpenseBreakdown> ExpenseBreakdown { get; set; } = new();
    public List<MonthlyMilestone> Milestones { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
}

public class MonthlyExpenseBreakdown
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class MonthlyMilestone
{
    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
}