# Notification Debugging Guide

## Changes Made

### Issue Identified
The `GetFinanceTeamEmailsAsync` method was only looking in the **Headcount** table, but Finance users are stored in the **User** table.

### Fix Applied
Updated `NotificationService.GetFinanceTeamEmailsAsync()` to:
1. **First** check the Users table for users with Role = "Finance" and IsActive = true
2. **Then** fallback to Headcount table if no users found
3. Added extensive debug logging throughout the notification flow

### Debug Logging Added
Now you'll see console output showing:
- When a transaction is created with category and amount
- The threshold check for Finance notification
- Whether the transaction meets the threshold
- All Finance team members found
- Each notification being created
- Any errors with full stack trace

## Testing Steps

### 1. Build the Solution
```powershell
dotnet build TrevelOperation.sln
```

### 2. Run the Application
Start the WPF app and watch the console output

### 3. Create a Meal Transaction as Employee
1. Login as employee (e.g., `john.doe@wsc.com` / `emp123`)
2. Create a new transaction:
   - Category: **Meals**
   - Amount: **$1200** (or any amount ≥ $80)
   - Fill in other required fields
3. Save the transaction

### 4. Check Console Output
You should see messages like:
```
[TransactionService] Transaction created - Category: Meals, Amount: $1200
[TransactionService] Threshold for Meals: $80, Actual: $1200
[TransactionService] Notifying Finance team for Meals transaction
[NotificationService] NotifyFinanceTeamAsync called
[NotificationService] Getting Finance team emails...
[NotificationService] Found 3 Finance users in Users table
[NotificationService] Finance team emails: admin@corporate.com, martina.popinsk@wsc.com, maayan.chesler@wsc.com
[NotificationService] Found 3 finance team members
[NotificationService] Creating notification for: admin@corporate.com
[NotificationService] Notification created successfully for admin@corporate.com
[NotificationService] Creating notification for: martina.popinsk@wsc.com
[NotificationService] Notification created successfully for martina.popinsk@wsc.com
[NotificationService] Creating notification for: maayan.chesler@wsc.com
[NotificationService] Notification created successfully for maayan.chesler@wsc.com
[NotificationService] All Finance team notifications sent
[TransactionService] Finance team notification sent successfully
```

### 5. Login as Finance User
1. Logout from employee account
2. Login as Finance user (e.g., `martina.popinsk@wsc.com` / `finance123`)
3. Navigate to **Notifications** page
4. You should see the notification for the new meal transaction

### 6. Verify Notification Details
The notification should show:
- **Icon**: 💼
- **Title**: "New Meals Transaction Requires Review"
- **Message**: "Employee john.doe@wsc.com submitted a Meals expense of $1,200.00. Review required."
- **Action Button**: "Take Action" → redirects to `/data-integrity/controls/meals`
- **Badge**: "New" (if unread)

## Finance Users with Notification Access

Based on `UserManagementService.cs`, these users have Finance role:
1. `admin@corporate.com` - Finance Department
2. `martina.popinsk@wsc.com` - Finance Department
3. `maayan.chesler@wsc.com` - Finance Department
4. `ceo@wsc.com` - Executive Department
5. `cfo@wsc.com` - Executive Department
6. `cto@wsc.com` - Executive Department

## Notification Thresholds

Finance team gets notified when transactions exceed:
- **Meals**: ≥ $80
- **Lodging**: ≥ $100
- **Client Entertainment**: ≥ $50
- **Other Categories**: ≥ $200

Additionally, **all** users get notified for high-value transactions ≥ $1000.

## Troubleshooting

### No Console Output
- Make sure you're running in Debug mode
- Check that the application is writing to the console window

### No Notifications Showing
1. Check if Finance users exist in the database:
   ```sql
   SELECT * FROM Users WHERE Role = 'Finance' AND IsActive = 1
   ```

2. Check if notifications were created:
   ```sql
   SELECT * FROM Notifications ORDER BY CreatedAt DESC LIMIT 10
   ```

3. Verify you're logged in with the correct Finance email

### Notifications Created but Not Visible
- Ensure `currentUserEmail` in Notifications.razor matches the logged-in user
- Check the LoadNotifications() method is being called on page load
- Verify AuthenticationService is returning the correct email

## Next Steps

Once notifications are working:
1. Remove or reduce debug logging (make it conditional on debug flag)
2. Test all other Data Integrity Control pages
3. Test bidirectional flow (Finance marks as valid → Employee gets notified)
4. Implement remaining phases (Trip notifications, Policy compliance)
