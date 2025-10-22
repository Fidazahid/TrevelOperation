using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TravelOperation.Core.Models.Entities;
using TrevelOperation.RazorLib.Components;

namespace TrevelOperation.RazorLib.Pages;

public partial class TableDemo : ComponentBase
{
    private List<Transaction> SampleTransactions = new();
    private List<TableColumn> TransactionColumns = new();
    private bool IsEditMode = false;
    private bool ShowValidation = false;
    private List<string> ValidationMessages = new();
    private int ValidRecords = 0;
    private int InvalidRecords = 0;

    protected override void OnInitialized()
    {
        SetupColumns();
    }

    private void SetupColumns()
    {
        TransactionColumns = new List<TableColumn>
        {
            new() { PropertyName = "TransactionId", DisplayName = "Transaction ID", DataType = TableDataType.Text, IsVisible = true, Width = 150, Order = 0 },
            new() { PropertyName = "Email", DisplayName = "Email", DataType = TableDataType.Text, IsVisible = true, IsEditable = true, Width = 200, Order = 1 },
            new() { PropertyName = "TransactionDate", DisplayName = "Date", DataType = TableDataType.Date, IsVisible = true, Width = 120, Order = 2 },
            new() { PropertyName = "AuthorizationDate", DisplayName = "Auth Date", DataType = TableDataType.Date, IsVisible = false, Width = 120, Order = 3 },
            new() { PropertyName = "TransactionType", DisplayName = "Type", DataType = TableDataType.Text, IsVisible = true, IsEditable = true, Width = 100, Order = 4 },
            new() { PropertyName = "Vendor", DisplayName = "Vendor", DataType = TableDataType.Text, IsVisible = true, IsEditable = true, Width = 180, Order = 5 },
            new() { PropertyName = "Address", DisplayName = "Address", DataType = TableDataType.Text, IsVisible = false, IsEditable = true, Width = 200, Order = 6 },
            new() { PropertyName = "Currency", DisplayName = "Currency", DataType = TableDataType.Text, IsVisible = true, Width = 80, Order = 7 },
            new() { PropertyName = "Amount", DisplayName = "Amount", DataType = TableDataType.Currency, IsVisible = true, IsEditable = true, Width = 100, Order = 8 },
            new() { PropertyName = "AmountUSD", DisplayName = "Amount USD", DataType = TableDataType.Currency, IsVisible = true, Width = 110, Order = 9 },
            new() { PropertyName = "ExchangeRate", DisplayName = "Rate", DataType = TableDataType.Number, IsVisible = false, Width = 80, Order = 10 },
            new() { PropertyName = "Participants", DisplayName = "Participants", DataType = TableDataType.Text, IsVisible = true, IsEditable = true, Width = 200, Order = 11 },
            new() { PropertyName = "Notes", DisplayName = "Notes", DataType = TableDataType.Text, IsVisible = false, IsEditable = true, Width = 200, Order = 12 },
            new() { PropertyName = "IsValid", DisplayName = "Valid", DataType = TableDataType.Boolean, IsVisible = true, IsEditable = true, Width = 80, Order = 13 },
            new() { PropertyName = "CreatedAt", DisplayName = "Created", DataType = TableDataType.Date, IsVisible = false, Width = 120, Order = 14 },
            new() { PropertyName = "ModifiedBy", DisplayName = "Modified By", DataType = TableDataType.Text, IsVisible = false, Width = 120, Order = 15 }
        };
    }

    private void GenerateSampleData()
    {
        var random = new Random();
        var vendors = new[] { "Marriott Hotels", "Delta Airlines", "Uber", "Starbucks", "Shell", "McDonald's", "Hertz", "Hilton", "Emirates", "Lufthansa" };
        var currencies = new[] { "USD", "EUR", "GBP", "ILS", "CAD" };
        var types = new[] { "Purchase", "Refund", "Reimbursement" };
        var emails = new[] { "john.doe@company.com", "jane.smith@company.com", "mike.johnson@company.com", "sarah.wilson@company.com" };
        var addresses = new[] { "New York, NY", "London, UK", "Tel Aviv, Israel", "Toronto, CA", "Berlin, Germany" };

        SampleTransactions.Clear();

        for (int i = 1; i <= 50; i++)
        {
            var currency = currencies[random.Next(currencies.Length)];
            var amount = random.Next(10, 5000);
            var exchangeRate = currency == "USD" ? 1 : random.NextDouble() * 2 + 0.5;
            
            var transaction = new Transaction
            {
                TransactionId = $"TXN-{DateTime.Now:yyyyMM}-{i:D4}",
                Email = emails[random.Next(emails.Length)],
                TransactionDate = DateTime.Now.AddDays(-random.Next(1, 90)),
                AuthorizationDate = DateTime.Now.AddDays(-random.Next(0, 7)),
                TransactionType = types[random.Next(types.Length)],
                Vendor = vendors[random.Next(vendors.Length)],
                Address = addresses[random.Next(addresses.Length)],
                Currency = currency,
                Amount = amount,
                AmountUSD = (decimal)(amount * exchangeRate),
                ExchangeRate = (decimal)exchangeRate,
                Participants = random.Next(3) == 0 ? emails[random.Next(emails.Length)] : "",
                Notes = random.Next(4) == 0 ? "Sample note for testing" : "",
                IsValid = random.Next(10) > 2, // 80% valid
                CreatedAt = DateTime.Now.AddDays(-random.Next(1, 30)),
                ModifiedBy = emails[random.Next(emails.Length)]
            };

            SampleTransactions.Add(transaction);
        }

        ValidateData();
        StateHasChanged();
    }

    private void ClearData()
    {
        SampleTransactions.Clear();
        ValidationMessages.Clear();
        ValidRecords = 0;
        InvalidRecords = 0;
        StateHasChanged();
    }

    private void ValidateData()
    {
        ValidationMessages.Clear();
        ValidRecords = 0;
        InvalidRecords = 0;

        foreach (var transaction in SampleTransactions)
        {
            var issues = new List<string>();

            // Validation rules
            if (string.IsNullOrWhiteSpace(transaction.Email))
                issues.Add("Email is required");
            
            if (transaction.Amount <= 0)
                issues.Add("Amount must be greater than 0");
            
            if (transaction.AmountUSD > 2000)
                issues.Add($"High amount: ${transaction.AmountUSD:N2} requires approval");
            
            if (string.IsNullOrWhiteSpace(transaction.Vendor))
                issues.Add("Vendor is required");
            
            if (transaction.TransactionDate > DateTime.Now)
                issues.Add("Transaction date cannot be in the future");

            if (issues.Any())
            {
                InvalidRecords++;
                ValidationMessages.AddRange(issues.Select(issue => $"{transaction.TransactionId}: {issue}"));
            }
            else
            {
                ValidRecords++;
            }
        }
    }

    private void OnTransactionDoubleClick(Transaction transaction)
    {
        Console.WriteLine($"Double clicked: {transaction.TransactionId}");
        // Here you would typically open a detail view or edit dialog
    }

    private void OnTransactionEdit(Transaction transaction)
    {
        Console.WriteLine($"Edited: {transaction.TransactionId}");
        transaction.ModifiedBy = "current.user@company.com";
        ValidateData();
        StateHasChanged();
    }

    private async Task OnExportCsv(List<Transaction> transactions)
    {
        await JSRuntime.InvokeVoidAsync("dataTable.exportTableToCsv", 
            null, $"transactions_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
    }

    private async Task OnExportExcel(List<Transaction> transactions)
    {
        // In a real application, this would call the ExportService
        Console.WriteLine($"Export to Excel: {transactions.Count} transactions");
        
        // Simulate file download
        await JSRuntime.InvokeVoidAsync("alert", 
            $"Excel export simulation: {transactions.Count} transactions would be exported");
    }

    private string GetTransactionRowClass(Transaction transaction)
    {
        var classes = new List<string>();

        if (!transaction.IsValid)
            classes.Add("invalid-record");
        
        if (transaction.AmountUSD > 1000)
            classes.Add("high-amount");

        return string.Join(" ", classes);
    }

    private void ToggleEditMode()
    {
        IsEditMode = !IsEditMode;
        
        if (IsEditMode)
        {
            // Enable editing for all editable columns
            foreach (var column in TransactionColumns.Where(c => c.IsEditable))
            {
                column.IsEditable = true;
            }
        }
        else
        {
            // Save changes and exit edit mode
            ValidateData();
        }
        
        StateHasChanged();
    }

    private void ShowValidationSummary()
    {
        ValidateData();
        ShowValidation = true;
        StateHasChanged();
    }

    private void HideValidationSummary()
    {
        ShowValidation = false;
        StateHasChanged();
    }

    private void FixValidationIssues()
    {
        foreach (var transaction in SampleTransactions)
        {
            // Auto-fix common issues
            if (string.IsNullOrWhiteSpace(transaction.Email))
                transaction.Email = "unknown@company.com";
            
            if (transaction.Amount <= 0)
                transaction.Amount = 1;
            
            if (string.IsNullOrWhiteSpace(transaction.Vendor))
                transaction.Vendor = "Unknown Vendor";
            
            if (transaction.TransactionDate > DateTime.Now)
                transaction.TransactionDate = DateTime.Now;
            
            transaction.IsValid = true;
            transaction.ModifiedBy = "system.auto-fix";
        }

        ValidateData();
        HideValidationSummary();
        StateHasChanged();
    }
}