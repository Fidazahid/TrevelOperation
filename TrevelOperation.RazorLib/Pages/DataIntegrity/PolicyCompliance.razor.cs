using Microsoft.AspNetCore.Components;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;
using TrevelOperation.RazorLib.Components;

namespace TrevelOperation.RazorLib.Pages.DataIntegrity;

public partial class PolicyCompliance
{
    private ComplianceStats? stats;
    private List<PolicyComplianceResult>? complianceResults;
    private List<PolicyComplianceResult> filteredResults => ApplyClientSideFilters();
    private IEnumerable<PolicyComplianceResult> pagedResults => filteredResults.Skip((currentPage - 1) * pageSize).Take(pageSize);
    private List<Transaction>? transactions;

    private bool loading = true;
    private bool showApproveModal = false;
    private bool showFlagModal = false;
    private bool showTransactionDetailModal = false;
    private PolicyComplianceResult? selectedResult;
    private Transaction? selectedTransaction;
    private TransactionDetailModal? transactionDetailModal;
    private TransactionEditModal? transactionEditModal;

    private string selectedSeverity = string.Empty;
    private string selectedViolationType = string.Empty;
    private string requiresApprovalFilter = string.Empty;
    
    private string approvedBy = string.Empty;
    private string approvalReason = string.Empty;
    private string flagReason = string.Empty;
    private string selectedFlagViolationType = string.Empty;

    private int currentPage = 1;
    private int pageSize = 10;
    private int totalPages => filteredResults.Count > 0 ? (int)Math.Ceiling((double)filteredResults.Count / pageSize) : 1;

    // Alert Dialog state
    private bool isAlertVisible;
    private string alertTitle = string.Empty;
    private string alertMessage = string.Empty;
    private AlertDialog.AlertType alertType;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        loading = true;
        try
        {
            // Get all non-compliant transactions
            var nonCompliantTransactions = await PolicyComplianceService.GetNonCompliantTransactionsAsync();
            transactions = nonCompliantTransactions.ToList();

            // Check compliance for each
            complianceResults = await PolicyComplianceService.CheckMultipleComplianceAsync(transactions);

            // Calculate stats
            CalculateStats();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading policy compliance data: {ex.Message}");
            complianceResults = new List<PolicyComplianceResult>();
            transactions = new List<Transaction>();
        }
        finally
        {
            loading = false;
        }
    }

    private void CalculateStats()
    {
        if (complianceResults == null || !complianceResults.Any())
        {
            stats = new ComplianceStats();
            return;
        }

        stats = new ComplianceStats
        {
            CriticalCount = complianceResults.Count(r => r.Violations.Any(v => v.Severity == PolicySeverity.Critical)),
            HighCount = complianceResults.Count(r => r.Violations.Any(v => v.Severity == PolicySeverity.High)),
            MediumCount = complianceResults.Count(r => r.Violations.Any(v => v.Severity == PolicySeverity.Medium)),
            LowCount = complianceResults.Count(r => r.Violations.Any(v => v.Severity == PolicySeverity.Low)),
            TotalAmount = transactions?.Where(t => complianceResults.Any(r => r.TransactionId == t.TransactionId))
                .Sum(t => t.AmountUSD ?? 0) ?? 0
        };
    }

    private async Task RefreshData()
    {
        currentPage = 1;
        await LoadData();
    }

    private async Task RunComplianceCheck()
    {
        loading = true;
        try
        {
            // Get ALL transactions
            var allTransactions = await TransactionService.GetAllTransactionsAsync();
            
            // Check compliance for all
            complianceResults = await PolicyComplianceService.CheckMultipleComplianceAsync(allTransactions);
            
            // Filter to only non-compliant
            complianceResults = complianceResults.Where(r => !r.IsCompliant).ToList();
            
            // Get transaction details for non-compliant ones
            var nonCompliantIds = complianceResults.Select(r => r.TransactionId).ToList();
            transactions = allTransactions.Where(t => nonCompliantIds.Contains(t.TransactionId)).ToList();

            CalculateStats();

            ShowAlert("Compliance Check Complete", $"Found {complianceResults.Count} non-compliant transactions.", AlertDialog.AlertType.Info);
        }
        catch (Exception ex)
        {
            ShowAlert("Error", $"Error running compliance check: {ex.Message}", AlertDialog.AlertType.Error);
        }
        finally
        {
            loading = false;
        }
    }

    private List<PolicyComplianceResult> ApplyClientSideFilters()
    {
        if (complianceResults == null)
            return new List<PolicyComplianceResult>();

        var results = complianceResults.AsEnumerable();

        // Filter by severity
        if (!string.IsNullOrEmpty(selectedSeverity))
        {
            var severityEnum = Enum.Parse<PolicySeverity>(selectedSeverity);
            results = results.Where(r => r.Violations.Any(v => v.Severity == severityEnum));
        }

        // Filter by violation type
        if (!string.IsNullOrEmpty(selectedViolationType))
        {
            var violationTypeEnum = Enum.Parse<PolicyViolationType>(selectedViolationType);
            results = results.Where(r => r.Violations.Any(v => v.Type == violationTypeEnum));
        }

        // Filter by approval requirement
        if (!string.IsNullOrEmpty(requiresApprovalFilter))
        {
            var requiresApproval = bool.Parse(requiresApprovalFilter);
            results = results.Where(r => r.Violations.Any(v => v.RequiresApproval == requiresApproval));
        }

        return results.ToList();
    }

    private void ApplyFilters()
    {
        currentPage = 1;
        StateHasChanged();
    }

    private void ChangePage(int page)
    {
        if (page >= 1 && page <= totalPages)
        {
            currentPage = page;
        }
    }

    private string GetSeverityBorderClass(PolicyComplianceResult result)
    {
        var highestSeverity = GetHighestSeverityEnum(result);
        return highestSeverity switch
        {
            PolicySeverity.Critical => "border-error",
            PolicySeverity.High => "border-warning",
            PolicySeverity.Medium => "border-info",
            PolicySeverity.Low => "border-base-300",
            _ => "border-base-300"
        };
    }

    private string GetSeverityBadgeClass(PolicyComplianceResult result)
    {
        var highestSeverity = GetHighestSeverityEnum(result);
        return highestSeverity switch
        {
            PolicySeverity.Critical => "badge-error",
            PolicySeverity.High => "badge-warning",
            PolicySeverity.Medium => "badge-info",
            PolicySeverity.Low => "badge-ghost",
            _ => "badge-ghost"
        };
    }

    private string GetSeverityIcon(PolicyComplianceResult result)
    {
        var highestSeverity = GetHighestSeverityEnum(result);
        return highestSeverity switch
        {
            PolicySeverity.Critical => "🔴",
            PolicySeverity.High => "🟠",
            PolicySeverity.Medium => "🟡",
            PolicySeverity.Low => "⚪",
            _ => "⚪"
        };
    }

    private string GetHighestSeverity(PolicyComplianceResult result)
    {
        return GetHighestSeverityEnum(result).ToString();
    }

    private PolicySeverity GetHighestSeverityEnum(PolicyComplianceResult result)
    {
        if (!result.Violations.Any())
            return PolicySeverity.Low;

        return result.Violations.Max(v => v.Severity);
    }

    private string GetViolationAlertClass(PolicySeverity severity)
    {
        return severity switch
        {
            PolicySeverity.Critical => "alert-error",
            PolicySeverity.High => "alert-warning",
            PolicySeverity.Medium => "alert-info",
            PolicySeverity.Low => "alert-ghost",
            _ => "alert-ghost"
        };
    }

    private void ApproveException(PolicyComplianceResult result)
    {
        selectedResult = result;
        approvedBy = string.Empty;
        approvalReason = string.Empty;
        showApproveModal = true;
    }

    private async Task ConfirmApprove()
    {
        if (selectedResult == null || string.IsNullOrWhiteSpace(approvedBy) || string.IsNullOrWhiteSpace(approvalReason))
            return;

        try
        {
            await PolicyComplianceService.ApproveExceptionAsync(selectedResult.TransactionId, approvedBy, approvalReason);
            
            CloseApproveModal();
            await RefreshData();
            
            ShowAlert("Success", "Exception approved successfully!", AlertDialog.AlertType.Success);
        }
        catch (Exception ex)
        {
            ShowAlert("Error", $"Error approving exception: {ex.Message}", AlertDialog.AlertType.Error);
        }
    }

    private void CloseApproveModal()
    {
        showApproveModal = false;
        selectedResult = null;
        approvedBy = string.Empty;
        approvalReason = string.Empty;
    }

    private void FlagTransaction(PolicyComplianceResult result)
    {
        selectedResult = result;
        flagReason = string.Empty;
        selectedFlagViolationType = string.Empty;
        showFlagModal = true;
    }

    private async Task ConfirmFlag()
    {
        if (selectedResult == null || string.IsNullOrWhiteSpace(flagReason) || string.IsNullOrWhiteSpace(selectedFlagViolationType))
            return;

        try
        {
            var violationType = Enum.Parse<PolicyViolationType>(selectedFlagViolationType);
            await PolicyComplianceService.FlagTransactionAsync(selectedResult.TransactionId, flagReason, violationType);
            
            CloseFlagModal();
            await RefreshData();
            
            ShowAlert("Success", "Transaction flagged successfully!", AlertDialog.AlertType.Success);
        }
        catch (Exception ex)
        {
            ShowAlert("Error", $"Error flagging transaction: {ex.Message}", AlertDialog.AlertType.Error);
        }
    }

    private void CloseFlagModal()
    {
        showFlagModal = false;
        selectedResult = null;
        flagReason = string.Empty;
        selectedFlagViolationType = string.Empty;
    }

    private void ViewTransaction(Transaction transaction)
    {
        selectedTransaction = transaction;
        showTransactionDetailModal = true;
    }

    private void CloseTransactionDetailModal()
    {
        showTransactionDetailModal = false;
        selectedTransaction = null;
    }

    private async Task HandleTransactionEdit(Transaction transaction)
    {
        // Close the detail modal and open the edit modal
        showTransactionDetailModal = false;
        
        if (transactionEditModal != null)
        {
            await transactionEditModal.ShowAsync(transaction);
        }
    }

    private async Task HandleTransactionUpdated()
    {
        // Refresh data after transaction is updated
        await RefreshData();
    }

    private async Task HandleTransactionDelete(Transaction transaction)
    {
        // Refresh data after delete
        await RefreshData();
    }

    private void ShowAlert(string title, string message, AlertDialog.AlertType type)
    {
        alertTitle = title;
        alertMessage = message;
        alertType = type;
        isAlertVisible = true;
    }

    private void CloseAlertDialog()
    {
        isAlertVisible = false;
    }

    private class ComplianceStats
    {
        public int CriticalCount { get; set; }
        public int HighCount { get; set; }
        public int MediumCount { get; set; }
        public int LowCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
