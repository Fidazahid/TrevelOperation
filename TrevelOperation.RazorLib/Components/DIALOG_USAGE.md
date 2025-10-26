# Dialog Components Usage Guide

This guide explains how to use the **AlertDialog** and **ConfirmDialog** components in your Razor pages.

---

## 📦 Components Overview

### 1. **AlertDialog** - For Simple Messages
Shows a message with a single "OK" button. Used for notifications, errors, success messages, warnings, and info messages.

### 2. **ConfirmDialog** - For User Confirmation
Shows a message with two buttons (e.g., "Yes/No" or "OK/Cancel"). Used when you need the user to confirm an action.

---

## 🚀 Quick Start

### Step 1: Add Dialog Components to Your Page

Add both dialog components **outside** the `<AuthorizeRoleView>` tag (at the end of your page):

```razor
</AuthorizeRoleView>

<!-- Alert Dialog (for messages) -->
<AlertDialog 
    IsVisible="showAlertDialog"
    Title="@alertTitle"
    Message="@alertMessage"
    Type="@alertType"
    OkButtonText="@alertOkText"
    OnClose="CloseAlertDialog" />

<!-- Confirmation Dialog (for confirmations) -->
<ConfirmDialog 
    IsVisible="showConfirmDialog"
    Title="@confirmTitle"
    Message="@confirmMessage"
    Icon="@confirmIcon"
    ConfirmButtonText="@confirmButtonText"
    CancelButtonText="@cancelButtonText"
    ConfirmButtonClass="@confirmButtonClass"
    OnResult="HandleConfirmResult" />
```

### Step 2: Add State Variables to `@code` Block

```csharp
@code {
    // Alert Dialog state
    private bool showAlertDialog = false;
    private string alertTitle = "";
    private string alertMessage = "";
    private AlertDialog.AlertType alertType = AlertDialog.AlertType.Info;
    private string alertOkText = "OK";
    
    // Confirmation Dialog state
    private bool showConfirmDialog = false;
    private string confirmTitle = "";
    private string confirmMessage = "";
    private string confirmIcon = "❓";
    private string confirmButtonText = "Yes";
    private string cancelButtonText = "No";
    private string confirmButtonClass = "btn-primary";
    private Func<Task>? pendingConfirmAction = null;
}
```

### Step 3: Add Helper Methods

```csharp
// Alert Dialog Methods
private void ShowAlert(string title, string message, AlertDialog.AlertType type = AlertDialog.AlertType.Info, string okText = "OK")
{
    alertTitle = title;
    alertMessage = message;
    alertType = type;
    alertOkText = okText;
    showAlertDialog = true;
    StateHasChanged();
}

private void CloseAlertDialog()
{
    showAlertDialog = false;
    StateHasChanged();
}

// Confirmation Dialog Methods
private void ShowConfirm(string title, string message, Func<Task> onConfirm, string icon = "❓", 
    string confirmText = "Yes", string cancelText = "No", string buttonClass = "btn-primary")
{
    confirmTitle = title;
    confirmMessage = message;
    confirmIcon = icon;
    confirmButtonText = confirmText;
    cancelButtonText = cancelText;
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
        await pendingConfirmAction.Invoke();
    }
    
    pendingConfirmAction = null;
    StateHasChanged();
}
```

---

## 📖 Usage Examples

### AlertDialog Examples

#### 1. Success Message
```csharp
ShowAlert(
    "Success", 
    "Transaction saved successfully!", 
    AlertDialog.AlertType.Success
);
```

#### 2. Error Message
```csharp
ShowAlert(
    "Error", 
    "Failed to save transaction. Please try again.", 
    AlertDialog.AlertType.Error
);
```

#### 3. Warning Message
```csharp
ShowAlert(
    "No Trips Available", 
    "No trips found for this user. Please create a trip first.", 
    AlertDialog.AlertType.Warning
);
```

#### 4. Info Message
```csharp
ShowAlert(
    "Information", 
    "Your changes have been saved automatically.", 
    AlertDialog.AlertType.Info
);
```

#### 5. Custom Button Text
```csharp
ShowAlert(
    "Task Complete", 
    "All transactions have been processed.", 
    AlertDialog.AlertType.Success,
    "Got it!"
);
```

---

### ConfirmDialog Examples

#### 1. Delete Confirmation
```csharp
ShowConfirm(
    "Delete Transaction",
    "Are you sure you want to delete this transaction? This action cannot be undone.",
    async () =>
    {
        await TransactionService.DeleteAsync(transactionId);
        ShowAlert("Deleted", "Transaction deleted successfully", AlertDialog.AlertType.Success);
        await RefreshData();
    },
    "🗑️",
    "Delete",
    "Cancel",
    "btn-error"
);
```

#### 2. Link Confirmation
```csharp
ShowConfirm(
    "Link Transaction",
    $"Link this transaction to trip:\n\n{tripName}\n{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}",
    async () =>
    {
        await TransactionService.LinkTransactionToTripAsync(transactionId, tripId);
        ShowAlert("Success", "Transaction linked successfully", AlertDialog.AlertType.Success);
        await RefreshData();
    },
    "🧳",
    "Link",
    "Cancel",
    "btn-primary"
);
```

#### 3. Save Changes Confirmation
```csharp
ShowConfirm(
    "Save Changes",
    "Do you want to save your changes before leaving?",
    async () =>
    {
        await SaveChangesAsync();
        Navigation.NavigateTo("/trips");
    },
    "💾",
    "Save",
    "Discard",
    "btn-success"
);
```

#### 4. Approve Action
```csharp
ShowConfirm(
    "Approve Transaction",
    "Approve this transaction for payment?",
    async () =>
    {
        await TransactionService.ApproveAsync(transactionId);
        ShowAlert("Approved", "Transaction approved!", AlertDialog.AlertType.Success);
    },
    "✅",
    "Approve",
    "Cancel",
    "btn-success"
);
```

---

## 🎨 AlertDialog Alert Types

| Type | Icon | Color | Use Case |
|------|------|-------|----------|
| `AlertType.Success` | ✅ | Green | Success messages, completed actions |
| `AlertType.Error` | ❌ | Red | Error messages, failed operations |
| `AlertType.Warning` | ⚠️ | Yellow | Warnings, important notices |
| `AlertType.Info` | ℹ️ | Blue | Information, tips, neutral messages |

---

## 🔧 ConfirmDialog Button Styles

| Button Class | Color | Use Case |
|-------------|-------|----------|
| `btn-primary` | Blue | Default confirmations |
| `btn-success` | Green | Approve, save, accept actions |
| `btn-error` | Red | Delete, remove, destructive actions |
| `btn-warning` | Yellow | Caution actions |
| `btn-info` | Blue | Information actions |

---

## 🎭 Custom Icons for ConfirmDialog

You can use any emoji or icon for the ConfirmDialog:

```csharp
"❓" // Question
"🧳" // Trip/Travel
"🗑️" // Delete
"💾" // Save
"✅" // Approve
"🔗" // Link
"📧" // Email
"⚠️" // Warning
"🚀" // Launch/Start
"🔒" // Lock/Security
```

---

## 💡 Tips & Best Practices

### 1. **Use Descriptive Titles**
```csharp
// ❌ Bad
ShowAlert("Error", "An error occurred");

// ✅ Good
ShowAlert("Transaction Not Found", "The selected transaction could not be found in the database.");
```

### 2. **Provide Clear Actions**
```csharp
// ❌ Bad
ShowConfirm("Confirm", "Are you sure?", async () => { ... });

// ✅ Good
ShowConfirm(
    "Delete Transaction", 
    "Are you sure you want to delete this transaction? This action cannot be undone.",
    async () => { ... },
    "🗑️",
    "Delete",
    "Cancel",
    "btn-error"
);
```

### 3. **Use Appropriate Alert Types**
- **Success** ✅: After successful operations
- **Error** ❌: When operations fail
- **Warning** ⚠️: Before potentially dangerous actions
- **Info** ℹ️: For neutral information

### 4. **Chain Dialogs Properly**
```csharp
ShowConfirm(
    "Delete All",
    "Delete all selected transactions?",
    async () =>
    {
        await DeleteAllAsync();
        // Show success alert AFTER deletion
        ShowAlert("Success", "All transactions deleted", AlertDialog.AlertType.Success);
    }
);
```

### 5. **Handle Errors in Confirm Actions**
```csharp
ShowConfirm(
    "Save Changes",
    "Save all changes?",
    async () =>
    {
        try
        {
            await SaveAsync();
            ShowAlert("Success", "Changes saved", AlertDialog.AlertType.Success);
        }
        catch (Exception ex)
        {
            ShowAlert("Error", $"Failed to save: {ex.Message}", AlertDialog.AlertType.Error);
        }
    }
);
```

---

## 🔍 Common Patterns

### Pattern 1: Check Before Action
```csharp
private async Task DeleteTransaction(string transactionId)
{
    var transaction = await TransactionService.GetByIdAsync(transactionId);
    
    if (transaction == null)
    {
        ShowAlert("Not Found", "Transaction not found", AlertDialog.AlertType.Error);
        return;
    }
    
    ShowConfirm(
        "Delete Transaction",
        $"Delete transaction {transaction.TransactionId}?",
        async () =>
        {
            await TransactionService.DeleteAsync(transactionId);
            ShowAlert("Deleted", "Transaction deleted successfully", AlertDialog.AlertType.Success);
            await RefreshData();
        },
        "🗑️",
        "Delete",
        "Cancel",
        "btn-error"
    );
}
```

### Pattern 2: Validation with Alert
```csharp
private async Task LinkToTrip(string transactionId)
{
    var trips = await TripService.GetAvailableTripsAsync(email);
    
    if (!trips.Any())
    {
        ShowAlert(
            "No Trips Available",
            "No trips found. Please create a trip first.",
            AlertDialog.AlertType.Warning
        );
        return;
    }
    
    // Continue with linking logic...
}
```

### Pattern 3: Multi-step Confirmation
```csharp
private async Task ProcessBatch()
{
    ShowConfirm(
        "Process Batch",
        $"Process {selectedItems.Count} transactions?",
        async () =>
        {
            await ProcessTransactionsAsync();
            
            // Show result after processing
            ShowAlert(
                "Processing Complete",
                $"Successfully processed {selectedItems.Count} transactions",
                AlertDialog.AlertType.Success
            );
        }
    );
}
```

---

## 🐛 Troubleshooting

### Issue: Dialog Not Showing
**Solution**: Make sure dialogs are placed **outside** the `<AuthorizeRoleView>` tag.

```razor
</AuthorizeRoleView>

<!-- Place dialogs HERE -->
<AlertDialog ... />
<ConfirmDialog ... />
```

### Issue: Dialog Not Centered
**Solution**: The dialogs use inline styles for centering. They should appear in the center automatically.

### Issue: Multiple Dialogs at Once
**Solution**: Close the first dialog before opening another:

```csharp
CloseAlertDialog(); // Close alert first
ShowConfirm(...);   // Then show confirm
```

### Issue: StateHasChanged() Not Working
**Solution**: Make sure to call `StateHasChanged()` in helper methods (already included in the provided code).

---

## 📝 Full Example Page

See `AirfareControl.razor` for a complete working example with both dialogs integrated.

---

**Need Help?** Check the examples in `AirfareControl.razor` for real-world usage patterns! 🚀
