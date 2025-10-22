using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TrevelOperation.RazorLib.Pages.DataIntegrity;

public partial class Matching
{
    private IEnumerable<TripMatchingSuggestion>? matchingSuggestions;
    private IEnumerable<Transaction>? manualSearchResults;
    private MatchingStatistics? statistics;
    
    private bool loading = false;
    private string activeTab = "auto";
    
    // Manual matching fields
    private int selectedTripId = 0;
    private string emailFilter = string.Empty;
    private int daysTolerance = 5;

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
        await JSRuntime.InvokeVoidAsync("alert", 
            $"Matching Statistics:\n" +
            $"• Total Transactions: {statistics.TotalTransactions}\n" +
            $"• Linked: {statistics.LinkedTransactions} ({statistics.LinkingPercentage}%)\n" +
            $"• Unlinked: {statistics.UnlinkedTransactions}\n" +
            $"• Trips without transactions: {statistics.TripsWithoutTransactions}\n" +
            $"• Pending suggestions: {statistics.PendingSuggestions}");
    }

    private async Task SearchForMatches()
    {
        if (selectedTripId <= 0)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Please enter a valid Trip ID");
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
        try
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
                $"Link transaction {transactionId} to Trip {tripId}?");
            
            if (confirmed)
            {
                var success = await MatchingService.LinkTransactionToTripAsync(transactionId, tripId, "System");
                
                if (success)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Transaction linked successfully!");
                    await RefreshCurrentView();
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Failed to link transaction. Please try again.");
                }
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error linking transaction: {ex.Message}");
        }
    }

    private async Task UnlinkTransaction(string transactionId)
    {
        try
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
                $"Unlink transaction {transactionId} from its current trip?");
            
            if (confirmed)
            {
                var success = await MatchingService.UnlinkTransactionFromTripAsync(transactionId, "System");
                
                if (success)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Transaction unlinked successfully!");
                    await RefreshCurrentView();
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Failed to unlink transaction. Please try again.");
                }
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error unlinking transaction: {ex.Message}");
        }
    }

    private async Task LinkAllSuggestedTransactions(TripMatchingSuggestion suggestion)
    {
        var unlinkedTransactions = suggestion.SuggestedTransactions.Where(t => !t.IsAlreadyLinked).ToList();
        
        if (!unlinkedTransactions.Any())
        {
            await JSRuntime.InvokeVoidAsync("alert", "No unlinked transactions to process.");
            return;
        }

        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
            $"Link {unlinkedTransactions.Count} transactions to trip '{suggestion.TripName}'?");
        
        if (!confirmed) return;

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

        await JSRuntime.InvokeVoidAsync("alert", 
            $"Linking completed!\nSuccessful: {successCount}\nFailed: {failCount}");
        
        await RefreshCurrentView();
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