using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TravelOperation.Core.Interfaces;
using TravelOperation.Core.Models;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Models.Lookup;
using TravelOperation.Core.Services.Interfaces;
using TrevelOperation.Service;

namespace TrevelOperation.RazorLib.Pages;

public partial class Split
{
    [Inject] private ISplitService SplitService { get; set; } = null!;
    [Inject] private ITransactionService TransactionService { get; set; } = null!;
    [Inject] private ISettingsService SettingsService { get; set; } = null!;

    private const string suggestionsTab = "suggestions";
    private const string manualTab = "manual";
    
    private string activeTab = suggestionsTab;
    private bool isLoading = true;
    private bool isProcessing = false;
    private bool showSplitModal = false;

    private SplitStatistics? statistics;
    private IEnumerable<SplitSuggestion>? suggestions;
    private IEnumerable<Transaction>? searchResults;
    private IEnumerable<Category>? categories;

    private string searchQuery = string.Empty;
    private Transaction? selectedTransaction;
    private List<SplitItem> splitItems = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        isLoading = true;
        try
        {
            var statisticsTask = SplitService.GetSplitStatisticsAsync();
            var suggestionsTask = SplitService.GetSplitSuggestionsAsync();
            var categoriesTask = SettingsService.GetCategoriesAsync();

            await Task.WhenAll(statisticsTask, suggestionsTask, categoriesTask);

            statistics = await statisticsTask;
            suggestions = await suggestionsTask;
            categories = await categoriesTask;
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task RefreshSuggestions()
    {
        isLoading = true;
        try
        {
            suggestions = await SplitService.GetSplitSuggestionsAsync();
            statistics = await SplitService.GetSplitStatisticsAsync();
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task ApplySuggestion(SplitSuggestion suggestion)
    {
        isProcessing = true;
        try
        {
            var success = await SplitService.ApplySplitAsync(
                suggestion.TransactionId, 
                suggestion.SuggestedSplits, 
                "current-user"); // TODO: Get actual user

            if (success)
            {
                await RefreshSuggestions();
                // TODO: Show success toast
            }
            else
            {
                // TODO: Show error toast
            }
        }
        finally
        {
            isProcessing = false;
        }
    }

    private async Task RejectSuggestion(SplitSuggestion suggestion)
    {
        isProcessing = true;
        try
        {
            await SplitService.RejectSplitSuggestionAsync(suggestion.TransactionId, "current-user");
            await RefreshSuggestions();
            // TODO: Show success toast
        }
        finally
        {
            isProcessing = false;
        }
    }

    private void OpenManualEdit(SplitSuggestion suggestion)
    {
        selectedTransaction = suggestion.Transaction;
        splitItems = suggestion.SuggestedSplits.ToList();
        showSplitModal = true;
    }

    private async Task SearchTransactions()
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            searchResults = null;
            return;
        }

        try
        {
            searchResults = await TransactionService.SearchTransactionsAsync(
                searchQuery, 
                pageSize: 20,
                includeAlreadySplit: false);
        }
        catch
        {
            searchResults = null;
            // TODO: Show error toast
        }
    }

    private async Task HandleSearchKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SearchTransactions();
        }
    }

    private void SelectTransactionForSplit(Transaction transaction)
    {
        selectedTransaction = transaction;
        InitializeSplitItems();
        showSplitModal = true;
    }

    private void InitializeSplitItems()
    {
        splitItems.Clear();
        
        if (selectedTransaction == null) return;

        // Create initial split with original person
        splitItems.Add(new SplitItem
        {
            Email = selectedTransaction.Email,
            Name = GetDisplayNameForEmail(selectedTransaction.Email),
            Amount = selectedTransaction.Amount / 2,
            AmountUSD = (selectedTransaction.AmountUSD ?? 0) / 2,
            CategoryId = selectedTransaction.CategoryId,
            IsExternal = false
        });

        // Add second split item for another person
        splitItems.Add(new SplitItem
        {
            Email = "",
            Name = "",
            Amount = selectedTransaction.Amount / 2,
            AmountUSD = (selectedTransaction.AmountUSD ?? 0) / 2,
            CategoryId = selectedTransaction.CategoryId,
            IsExternal = true
        });
    }

    private void AddSplitItem()
    {
        if (selectedTransaction == null) return;

        var remainingAmount = (selectedTransaction.AmountUSD ?? 0) - splitItems.Sum(s => s.AmountUSD);
        var remainingOriginalAmount = selectedTransaction.Amount - splitItems.Sum(s => s.Amount);

        splitItems.Add(new SplitItem
        {
            Email = "",
            Name = "",
            Amount = remainingOriginalAmount,
            AmountUSD = remainingAmount,
            CategoryId = selectedTransaction.CategoryId,
            IsExternal = true
        });
    }

    private void RemoveSplitItem(int index)
    {
        if (splitItems.Count > 2) // Minimum 2 splits required
        {
            splitItems.RemoveAt(index);
        }
    }

    private async Task SaveSplit()
    {
        if (selectedTransaction == null) return;

        isProcessing = true;
        try
        {
            var success = await SplitService.CreateManualSplitAsync(
                selectedTransaction.TransactionId,
                splitItems,
                "current-user"); // TODO: Get actual user

            if (success)
            {
                CloseSplitModal();
                await RefreshSuggestions();
                // TODO: Show success toast
            }
            else
            {
                // TODO: Show error toast
            }
        }
        finally
        {
            isProcessing = false;
        }
    }

    private void CloseSplitModal()
    {
        showSplitModal = false;
        selectedTransaction = null;
        splitItems.Clear();
    }

    private string GetConfidenceBadgeClass(int confidence)
    {
        return confidence switch
        {
            >= 80 => "bg-green-100 text-green-800",
            >= 60 => "bg-yellow-100 text-yellow-800",
            _ => "bg-red-100 text-red-800"
        };
    }

    private string GetDisplayNameForEmail(string email)
    {
        if (email.Contains("@"))
        {
            return email.Split('@')[0].Replace(".", " ").Replace("_", " ");
        }
        return email;
    }

    private string GetTabClass(string tabName)
    {
        return activeTab == tabName
            ? "border-blue-500 text-blue-600"
            : "border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300";
    }
}