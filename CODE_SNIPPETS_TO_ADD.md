# 🔧 Code Snippets - Add to Control Pages

This file contains the **exact code** you need to add to each control page to complete the bidirectional notification flow.

---

## 📝 Copy-Paste Instructions

### ⚠️ **Important Notes:**
1. **Don't disturb existing code** - Only add these snippets where indicated
2. **Test after each addition** - Verify notifications work before moving to next control
3. **Check for INotificationService injection** - Add if missing

---

## 1️⃣ **AirfareControl.razor**

### **Location:** `TrevelOperation.RazorLib/Pages/DataIntegrity/AirfareControl.razor`

### **Step 1: Check for NotificationService injection**

Look for this at the top of the file:
```csharp
@inject INotificationService NotificationService
```

If **NOT present**, add it after other `@inject` statements.

---

### **Step 2: Find the `MarkAsValid()` method**

Search for:
```csharp
private async Task MarkAsValid(string transactionId)
{
```

### **Step 3: Add notification code**

**FIND THIS CODE:**
```csharp
private async Task MarkAsValid(string transactionId)
{
    try
    {
        Console.WriteLine($"MarkAsValid called for: {transactionId}");
        
        await TransactionService.MarkAsValidAsync(transactionId);
        
        Console.WriteLine($"Transaction {transactionId} marked as valid in database");
        
        var localTransaction = filteredTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (localTransaction != null)
        {
            localTransaction.IsValid = true;
        }
        
        await LoadAirfareTransactions();
        CalculateSummary();
        StateHasChanged();
        
        ShowAlert("Success", "Transaction marked as valid", AlertDialog.AlertType.Success);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error marking as valid: {ex.Message}");
        ShowAlert("Error", $"Error marking as valid: {ex.Message}", AlertDialog.AlertType.Error);
    }
}
```

**REPLACE WITH:**
```csharp
private async Task MarkAsValid(string transactionId)
{
    try
    {
        Console.WriteLine($"MarkAsValid called for: {transactionId}");
        
        await TransactionService.MarkAsValidAsync(transactionId);
        
        Console.WriteLine($"Transaction {transactionId} marked as valid in database");
        
        var localTransaction = filteredTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (localTransaction != null)
        {
            localTransaction.IsValid = true;
            
            // ✅ NOTIFY EMPLOYEE
            try
            {
                var cabinClass = localTransaction.CabinClass?.Name ?? "Unknown";
                await NotificationService.NotifyEmployeeTransactionValidatedAsync(
                    employeeEmail: localTransaction.Email,
                    transactionId: localTransaction.TransactionId,
                    categoryName: "Airfare",
                    amount: localTransaction.AmountUSD ?? 0
                );
                Console.WriteLine($"Employee notification sent for transaction {transactionId}");
            }
            catch (Exception notifEx)
            {
                Console.WriteLine($"Warning: Could not send employee notification: {notifEx.Message}");
            }
        }
        
        await LoadAirfareTransactions();
        CalculateSummary();
        StateHasChanged();
        
        ShowAlert("Success", "Transaction marked as valid and employee notified", AlertDialog.AlertType.Success);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error marking as valid: {ex.Message}");
        ShowAlert("Error", $"Error marking as valid: {ex.Message}", AlertDialog.AlertType.Error);
    }
}
```

---

### **Step 4: Add to bulk validation method (if exists)**

**FIND THIS CODE:**
```csharp
private async Task MarkSelectedAsValid()
{
    try
    {
        Console.WriteLine($"MarkSelectedAsValid called for {selectedTransactions.Count} transactions");
        
        var successCount = 0;
        var errorCount = 0;
        
        foreach (var transactionId in selectedTransactions.ToList())
        {
            try
            {
                await TransactionService.MarkAsValidAsync(transactionId);
                
                var localTransaction = filteredTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
                if (localTransaction != null)
                {
                    localTransaction.IsValid = true;
                }
                
                successCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking {transactionId} as valid: {ex.Message}");
                errorCount++;
            }
        }
```

**REPLACE WITH:**
```csharp
private async Task MarkSelectedAsValid()
{
    try
    {
        Console.WriteLine($"MarkSelectedAsValid called for {selectedTransactions.Count} transactions");
        
        var successCount = 0;
        var errorCount = 0;
        
        foreach (var transactionId in selectedTransactions.ToList())
        {
            try
            {
                await TransactionService.MarkAsValidAsync(transactionId);
                
                var localTransaction = filteredTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
                if (localTransaction != null)
                {
                    localTransaction.IsValid = true;
                    
                    // ✅ NOTIFY EMPLOYEE
                    try
                    {
                        await NotificationService.NotifyEmployeeTransactionValidatedAsync(
                            employeeEmail: localTransaction.Email,
                            transactionId: localTransaction.TransactionId,
                            categoryName: "Airfare",
                            amount: localTransaction.AmountUSD ?? 0
                        );
                    }
                    catch (Exception notifEx)
                    {
                        Console.WriteLine($"Warning: Could not send notification: {notifEx.Message}");
                    }
                }
                
                successCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking {transactionId} as valid: {ex.Message}");
                errorCount++;
            }
        }
```

---

## 2️⃣ **LodgingControl.razor**

### **Location:** `TrevelOperation.RazorLib/Pages/DataIntegrity/LodgingControl.razor`

### **Step 1: Check for NotificationService injection**

Add if missing:
```csharp
@inject INotificationService NotificationService
```

---

### **Step 2: Update `MarkAsValid()` method**

**FIND THIS CODE:**
```csharp
private async Task MarkAsValid(string transactionId)
{
    try
    {
        await TransactionService.MarkAsValidAsync(transactionId);
        
        var localTransaction = filteredTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (localTransaction != null)
        {
            localTransaction.IsValid = true;
        }
        
        await LoadLodgingTransactions();
        StateHasChanged();
        
        ShowAlert("Success", "Transaction marked as valid", AlertDialog.AlertType.Success);
    }
    catch (Exception ex)
    {
        ShowAlert("Error", $"Error: {ex.Message}", AlertDialog.AlertType.Error);
    }
}
```

**REPLACE WITH:**
```csharp
private async Task MarkAsValid(string transactionId)
{
    try
    {
        await TransactionService.MarkAsValidAsync(transactionId);
        
        var localTransaction = filteredTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (localTransaction != null)
        {
            localTransaction.IsValid = true;
            
            // ✅ NOTIFY EMPLOYEE
            try
            {
                await NotificationService.NotifyEmployeeTransactionValidatedAsync(
                    employeeEmail: localTransaction.Email,
                    transactionId: localTransaction.TransactionId,
                    categoryName: "Lodging",
                    amount: localTransaction.AmountUSD ?? 0
                );
                Console.WriteLine($"Employee notification sent for transaction {transactionId}");
            }
            catch (Exception notifEx)
            {
                Console.WriteLine($"Warning: Could not send employee notification: {notifEx.Message}");
            }
        }
        
        await LoadLodgingTransactions();
        StateHasChanged();
        
        ShowAlert("Success", "Transaction marked as valid and employee notified", AlertDialog.AlertType.Success);
    }
    catch (Exception ex)
    {
        ShowAlert("Error", $"Error: {ex.Message}", AlertDialog.AlertType.Error);
    }
}
```

---

### **Step 3: Update bulk validation (if exists)**

Same pattern as AirfareControl - add notification inside the loop.

---

## 3️⃣ **ClientEntertainmentControl.razor**

### **Location:** `TrevelOperation.RazorLib/Pages/DataIntegrity/ClientEntertainmentControl.razor`

### **Step 1: Check for NotificationService injection**

Add if missing:
```csharp
@inject INotificationService NotificationService
```

---

### **Step 2: Update `UpdateParticipants()` method**

**FIND THIS CODE:**
```csharp
private async Task UpdateParticipants(string transactionId, string participants)
{
    try
    {
        await TransactionService.UpdateTransactionAsync(new
        {
            TransactionId = transactionId,
            Participants = participants,
            ParticipantsValidated = true
        });
        
        await LoadClientEntertainmentTransactions();
        StateHasChanged();
        
        ShowAlert("Success", "Participants updated successfully", AlertDialog.AlertType.Success);
    }
    catch (Exception ex)
    {
        ShowAlert("Error", $"Error updating participants: {ex.Message}", AlertDialog.AlertType.Error);
    }
}
```

**REPLACE WITH:**
```csharp
private async Task UpdateParticipants(string transactionId, string participants)
{
    try
    {
        await TransactionService.UpdateTransactionAsync(new
        {
            TransactionId = transactionId,
            Participants = participants,
            ParticipantsValidated = true
        });
        
        // ✅ NOTIFY EMPLOYEE
        var transaction = filteredTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (transaction != null)
        {
            try
            {
                var hasExternal = !string.IsNullOrEmpty(participants) && 
                                 participants.Split(',')
                                            .Any(p => !p.Trim().EndsWith("@wsc.com", StringComparison.OrdinalIgnoreCase));
                
                if (hasExternal)
                {
                    await NotificationService.CreateNotificationAsync(new Notification
                    {
                        RecipientEmail = transaction.Email,
                        Type = NotificationType.Warning,
                        Category = NotificationCategory.Transaction,
                        Priority = NotificationPriority.High,
                        Title = "Client Entertainment - External Participants Detected",
                        Message = $"Your client entertainment transaction of ${transaction.AmountUSD ?? 0:N2} includes external participants. Please ensure proper documentation for tax purposes.",
                        ActionUrl = $"/transactions?search={transactionId}",
                        ActionLabel = "View Transaction",
                        RelatedEntityId = transactionId,
                        RelatedEntityType = "Transaction",
                        Icon = "🍸"
                    });
                }
                else
                {
                    await NotificationService.CreateNotificationAsync(new Notification
                    {
                        RecipientEmail = transaction.Email,
                        Type = NotificationType.Success,
                        Category = NotificationCategory.Transaction,
                        Priority = NotificationPriority.Low,
                        Title = "Client Entertainment Participants Recorded",
                        Message = $"Participants have been recorded for your client entertainment transaction of ${transaction.AmountUSD ?? 0:N2}.",
                        ActionUrl = $"/transactions?search={transactionId}",
                        ActionLabel = "View Transaction",
                        RelatedEntityId = transactionId,
                        RelatedEntityType = "Transaction",
                        Icon = "✅"
                    });
                }
                Console.WriteLine($"Employee notification sent for transaction {transactionId}");
            }
            catch (Exception notifEx)
            {
                Console.WriteLine($"Warning: Could not send employee notification: {notifEx.Message}");
            }
        }
        
        await LoadClientEntertainmentTransactions();
        StateHasChanged();
        
        ShowAlert("Success", "Participants updated and employee notified", AlertDialog.AlertType.Success);
    }
    catch (Exception ex)
    {
        ShowAlert("Error", $"Error updating participants: {ex.Message}", AlertDialog.AlertType.Error);
    }
}
```

---

### **Step 3: Update `GenerateMessage()` method (if exists)**

**FIND THIS CODE:**
```csharp
private async Task GenerateMessage(Transaction transaction)
{
    var message = BuildMessageTemplate(transaction);
    await CopyToClipboard(message);
    ShowAlert("Success", "Message copied to clipboard", AlertDialog.AlertType.Success);
}
```

**REPLACE WITH:**
```csharp
private async Task GenerateMessage(Transaction transaction)
{
    var message = BuildMessageTemplate(transaction);
    await CopyToClipboard(message);
    
    // ✅ NOTIFY EMPLOYEE
    try
    {
        await NotificationService.NotifyEmployeeInquiryAsync(
            employeeEmail: transaction.Email,
            transactionId: transaction.TransactionId,
            categoryName: "Client Entertainment",
            inquiryReason: "Please provide participant information for tax compliance."
        );
        Console.WriteLine($"Employee inquiry notification sent for transaction {transaction.TransactionId}");
    }
    catch (Exception notifEx)
    {
        Console.WriteLine($"Warning: Could not send employee notification: {notifEx.Message}");
    }
    
    ShowAlert("Success", "Message copied and employee notified", AlertDialog.AlertType.Success);
}
```

---

## 4️⃣ **OtherControl.razor**

### **Location:** `TrevelOperation.RazorLib/Pages/DataIntegrity/OtherControl.razor`

### **Step 1: Check for NotificationService injection**

Add if missing:
```csharp
@inject INotificationService NotificationService
```

---

### **Step 2: Update `GenerateMessage()` method**

**FIND THIS CODE:**
```csharp
private async Task GenerateMessage(Transaction transaction)
{
    var message = BuildMessageTemplate(transaction);
    await CopyToClipboard(message);
    ShowAlert("Success", "Message copied to clipboard", AlertDialog.AlertType.Success);
}
```

**REPLACE WITH:**
```csharp
private async Task GenerateMessage(Transaction transaction)
{
    var message = BuildMessageTemplate(transaction);
    await CopyToClipboard(message);
    
    // ✅ NOTIFY EMPLOYEE
    try
    {
        await NotificationService.NotifyEmployeeInquiryAsync(
            employeeEmail: transaction.Email,
            transactionId: transaction.TransactionId,
            categoryName: "Other",
            inquiryReason: "Please help us categorize this transaction properly."
        );
        Console.WriteLine($"Employee inquiry notification sent for transaction {transaction.TransactionId}");
    }
    catch (Exception notifEx)
    {
        Console.WriteLine($"Warning: Could not send employee notification: {notifEx.Message}");
    }
    
    ShowAlert("Success", "Message copied and employee notified", AlertDialog.AlertType.Success);
}
```

---

## 🧪 Testing After Each Addition

After adding notifications to each control page:

### **Test Steps:**

1. **Build the solution:**
   ```bash
   dotnet build TrevelOperation.sln
   ```

2. **Run the application**

3. **Test the specific control:**
   - Navigate to the control page (e.g., /data-integrity/controls/airfare)
   - Find a transaction
   - Mark it as valid (or perform the action)
   - Check console for: `Employee notification sent for transaction {id}`

4. **Verify notification received:**
   - Logout
   - Login as the employee who created the transaction
   - Check notification bell → should show (1)
   - Click bell → see notification
   - Navigate to /notifications → verify full notification

5. **Verify notification content:**
   - Title should match category (e.g., "Airfare Transaction Validated ✅")
   - Message should include amount
   - Icon should be ✅
   - Action button should work
   - Clicking notification should navigate to transaction

---

## 📊 Completion Checklist

After adding all code snippets:

- [ ] ✅ AirfareControl.razor - MarkAsValid() updated
- [ ] ✅ AirfareControl.razor - MarkSelectedAsValid() updated (if exists)
- [ ] ✅ LodgingControl.razor - MarkAsValid() updated
- [ ] ✅ LodgingControl.razor - MarkSelectedAsValid() updated (if exists)
- [ ] ✅ ClientEntertainmentControl.razor - UpdateParticipants() updated
- [ ] ✅ ClientEntertainmentControl.razor - GenerateMessage() updated (if exists)
- [ ] ✅ OtherControl.razor - GenerateMessage() updated
- [ ] ✅ Build successful with no errors
- [ ] ✅ All tests passed

---

## ⚠️ Common Issues & Solutions

### **Issue 1: INotificationService not found**
**Solution:** Add this to your DI registration in `Startup.cs`:
```csharp
services.AddScoped<INotificationService, NotificationService>();
```

### **Issue 2: Notification doesn't show in UI**
**Check:**
- Console for: "Employee notification sent for transaction {id}"
- Database: `SELECT * FROM Notifications WHERE RecipientEmail = 'employee@email.com'`
- User is logged in with correct email
- Bell auto-refresh (wait 30 seconds)

### **Issue 3: Build errors**
**Common fixes:**
- Add missing `using` statements
- Check for typos in method names
- Verify `filteredTransactions` variable exists in your control

---

## 🎯 Final Verification

After completing all additions, run this complete test:

1. **Employee creates meal transaction ($85)**
   - Finance gets notification ✅

2. **Finance marks as valid in MealsControl**
   - Employee gets notification ✅

3. **Finance marks as valid in AirfareControl**
   - Employee gets notification ✅

4. **Finance marks as valid in LodgingControl**
   - Employee gets notification ✅

5. **Finance updates participants in ClientEntertainmentControl**
   - Employee gets notification ✅

6. **Finance generates message in OtherControl**
   - Employee gets notification ✅

If all 6 tests pass → **COMPLETE!** 🎉

---

**Happy Coding!** 🚀
