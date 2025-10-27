using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;
using TrevelOperation.RazorLib.Components;

namespace TrevelOperation.RazorLib.Pages.DataIntegrity;

public partial class Matching
{
    [Inject] private IMatchingService MatchingService { get; set; } = default!;
    
    private IEnumerable<TripMatchingSuggestion>? matchingSuggestions;
    private IEnumerable<Transaction>? manualSearchResults;
    private MatchingStatistics? statistics;
    
    private bool loading = false;
    private string activeTab = "auto";
    
    // Manual matching fields
    private int selectedTripId = 0;
    private string emailFilter = string.Empty;
    private int daysTolerance = 5;

    // Alert Dialog state
    private AlertDialog? alertDialog;
    private bool showAlertDialog = false;
    private string alertTitle = "";
    private string alertMessage = "";
    private AlertDialog.AlertType alertType = AlertDialog.AlertType.Info;

    // Confirm Dialog state
    private ConfirmDialog? confirmDialog;
    private bool showConfirmDialog = false;
    private string confirmTitle = "";
    private string confirmMessage = "";
    private string confirmIcon = "⚠️";
    private string confirmButtonText = "Confirm";
    private string confirmButtonClass = "btn-primary";
    private Func<Task>? pendingConfirmAction;

    private void ShowAlert(string title, string message, AlertDialog.AlertType type)
    {
        alertTitle = title;
        alertMessage = message;
        alertType = type;
        showAlertDialog = true;
        StateHasChanged();
    }

    private void CloseAlertDialog()
    {
        showAlertDialog = false;
        StateHasChanged();
    }

    private void ShowConfirm(string title, string message, Func<Task> onConfirm, string icon = "⚠️", string buttonText = "Confirm", string buttonClass = "btn-primary")
    {
        confirmTitle = title;
        confirmMessage = message;
        confirmIcon = icon;
        confirmButtonText = buttonText;
        confirmButtonClass = buttonClass;
        pendingConfirmAction = onConfirm;
        showConfirmDialog = true;
        StateHasChanged();
    }

    private async Task HandleConfirmResult(bool confirmed)
    {
        showConfirmDialog = false;
        
        if (confirmed && pendingConfirmAction != null)
        {
            await pendingConfirmAction();
        }
        
        pendingConfirmAction = null;
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        loading = true;
        try
        {
            statistics = await MatchingService.GetMatchingStatisticsAsync();
            if (activeTab == "auto")
            {
                matchingSuggestions = await MatchingService.GetAutoMatchingSuggestionsAsync();
            }
        }
        finally
        {
            loading = false;
        }
    }

    private void SetActiveTab(string tab)
    {
        activeTab = tab;
        manualSearchResults = null;
        selectedTripId = 0;
        emailFilter = string.Empty;
        
        if (tab == "auto")
        {
            _ = RefreshSuggestions();
        }
    }

    private async Task RefreshSuggestions()
    {
        loading = true;
        try
        {
            matchingSuggestions = await MatchingService.GetAutoMatchingSuggestionsAsync();
            statistics = await MatchingService.GetMatchingStatisticsAsync();
        }
        finally
        {
            loading = false;
        }
    }

    private async Task ShowStatistics()
    {
        statistics = await MatchingService.GetMatchingStatisticsAsync();
        ShowAlert(
            "Matching Statistics",
            $"Total Transactions: {statistics.TotalTransactions}\n" +
            $"Linked: {statistics.LinkedTransactions} ({statistics.LinkingPercentage}%)\n" +
            $"Unlinked: {statistics.UnlinkedTransactions}\n" +
            $"Trips without transactions: {statistics.TripsWithoutTransactions}\n" +
            $"Pending suggestions: {statistics.PendingSuggestions}",
            AlertDialog.AlertType.Info
        );
    }

    private async Task SearchForMatches()
    {
        if (selectedTripId <= 0)
        {
            ShowAlert("Validation Error", "Please enter a valid Trip ID", AlertDialog.AlertType.Warning);
            return;
        }

        loading = true;
        try
        {
            manualSearchResults = await MatchingService.GetTransactionsForMatchingAsync(selectedTripId, daysTolerance);
            
            if (!string.IsNullOrEmpty(emailFilter))
            {
                manualSearchResults = manualSearchResults.Where(t => 
                    t.Email.Contains(emailFilter, StringComparison.OrdinalIgnoreCase));
            }
        }
        finally
        {
            loading = false;
        }
    }

    private void ClearManualSearch()
    {
        selectedTripId = 0;
        emailFilter = string.Empty;
        daysTolerance = 5;
        manualSearchResults = null;
    }

    private async Task LinkTransaction(string transactionId, int tripId)
    {
        ShowConfirm(
            "Link Transaction",
            $"Link transaction {transactionId} to Trip {tripId}?",
            async () =>
            {
                try
                {
                    var success = await MatchingService.LinkTransactionToTripAsync(transactionId, tripId, "System");
                    
                    if (success)
                    {
                        ShowAlert("Success", "Transaction linked successfully!", AlertDialog.AlertType.Success);
                        await RefreshCurrentView();
                    }
                    else
                    {
                        ShowAlert("Error", "Failed to link transaction. Please try again.", AlertDialog.AlertType.Error);
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error", $"Error linking transaction: {ex.Message}", AlertDialog.AlertType.Error);
                }
            },
            "🔗",
            "Link",
            "btn-primary"
        );
    }

    private async Task UnlinkTransaction(string transactionId)
    {
        ShowConfirm(
            "Unlink Transaction",
            $"Unlink transaction {transactionId} from its current trip?",
            async () =>
            {
                try
                {
                    var success = await MatchingService.UnlinkTransactionFromTripAsync(transactionId, "System");
                    
                    if (success)
                    {
                        ShowAlert("Success", "Transaction unlinked successfully!", AlertDialog.AlertType.Success);
                        await RefreshCurrentView();
                    }
                    else
                    {
                        ShowAlert("Error", "Failed to unlink transaction. Please try again.", AlertDialog.AlertType.Error);
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("Error", $"Error unlinking transaction: {ex.Message}", AlertDialog.AlertType.Error);
                }
            },
            "🔓",
            "Unlink",
            "btn-warning"
        );
    }

    private async Task LinkAllSuggestedTransactions(TripMatchingSuggestion suggestion)
    {
        var unlinkedTransactions = suggestion.SuggestedTransactions.Where(t => !t.IsAlreadyLinked).ToList();
        
        if (!unlinkedTransactions.Any())
        {
            ShowAlert("No Unlinked Transactions", "No unlinked transactions to process.", AlertDialog.AlertType.Info);
            return;
        }

        ShowConfirm(
            "Link All Transactions",
            $"Link {unlinkedTransactions.Count} transactions to trip '{suggestion.TripName}'?",
            async () =>
            {
                var successCount = 0;
                var failCount = 0;

                foreach (var transaction in unlinkedTransactions)
                {
                    try
                    {
                        var success = await MatchingService.LinkTransactionToTripAsync(
                            transaction.TransactionId, suggestion.TripId, "System");
                        
                        if (success)
                            successCount++;
                        else
                            failCount++;
                    }
                    catch
                    {
                        failCount++;
                    }
                }

                ShowAlert(
                    "Linking Completed",
                    $"Linking completed!\nSuccessful: {successCount}\nFailed: {failCount}",
                    successCount > 0 ? AlertDialog.AlertType.Success : AlertDialog.AlertType.Error
                );
                
                await RefreshCurrentView();
            },
            "🔗",
            "Link All",
            "btn-primary"
        );
    }

    private async Task RefreshCurrentView()
    {
        if (activeTab == "auto")
        {
            await RefreshSuggestions();
        }
        else if (activeTab == "manual" && selectedTripId > 0)
        {
            await SearchForMatches();
        }
    }

    private string GetConfidenceBadgeClass(decimal confidence)
    {
        return confidence switch
        {
            >= 80 => "badge-success",
            >= 60 => "badge-warning",
            >= 40 => "badge-info",
            _ => "badge-error"
        };
    }

    private string GetCategoryName(int categoryId)
    {
        // This is a simplified approach - in a real implementation, 
        // you'd want to cache category lookups or inject a category service
        return $"Category {categoryId}";
    }
}