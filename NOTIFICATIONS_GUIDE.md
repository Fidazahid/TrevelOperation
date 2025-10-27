# 🔔 Notifications System - Complete Guide

## Table of Contents
1. [Overview](#overview)
2. [Notification Entity Structure](#notification-entity-structure)
3. [How to Produce/Create Notifications](#how-to-producecreate-notifications)
4. [Notification Service Functions](#notification-service-functions)
5. [Notification Page Features](#notification-page-features)
6. [Notification Types and Categories](#notification-types-and-categories)
7. [Usage Examples](#usage-examples)
8. [Best Practices](#best-practices)

---

## Overview

The Notifications System is a comprehensive alert and messaging framework for the Travel Expense Management System. It provides:
- Real-time user notifications
- Multiple notification types and priorities
- Automatic notifications for policy violations, high-value transactions, and compliance issues
- Action-oriented notifications with direct links to related items
- Read/unread tracking
- Filtering and categorization
- Automatic cleanup of expired notifications

---

## Notification Entity Structure

### Notification Properties

| Property | Type | Description |
|----------|------|-------------|
| `NotificationId` | int | Primary key (auto-generated) |
| `RecipientEmail` | string | Email of user receiving notification |
| `Type` | string | Notification type: Info, Success, Warning, Error |
| `Category` | string | Category: Transaction, Trip, PolicyViolation, etc. |
| `Title` | string | Notification title/subject (max 500 chars) |
| `Message` | string | Detailed message content (max 2000 chars) |
| `ActionUrl` | string? | Link to related item (e.g., /trips/123) |
| `ActionLabel` | string? | Button text (e.g., "View Trip") |
| `RelatedEntityId` | string? | ID of related entity |
| `RelatedEntityType` | string? | Type of related entity |
| `Priority` | string | Priority level: Low, Normal, High, Urgent |
| `IsRead` | bool | Whether notification has been read |
| `ReadAt` | DateTime? | When notification was read |
| `EmailSent` | bool | Whether email notification was sent |
| `EmailSentAt` | DateTime? | When email was sent |
| `ExpiresAt` | DateTime? | Expiration date (default: 90 days) |
| `CreatedAt` | DateTime | When notification was created |
| `Icon` | string? | Emoji/icon to display (e.g., 💰, ⚠️) |

---

## How to Produce/Create Notifications

### Method 1: Using Helper Methods (Recommended)

The `INotificationService` provides pre-built helper methods for common notification scenarios:

#### 1. **High-Value Transaction Notification**
```csharp
await NotificationService.NotifyHighValueTransactionAsync(
    email: "user@example.com",
    transactionId: "TXN-12345",
    amount: 250.00m,
    category: "Meals"
);
```

**When to Use:**
- Transaction amount exceeds thresholds ($80 for meals, $200 for general)
- Requires user verification
- Called automatically by TransactionService

---

#### 2. **Policy Violation Notification**
```csharp
await NotificationService.NotifyPolicyViolationAsync(
    email: "user@example.com",
    transactionId: "TXN-12345",
    violationType: "Cabin Class Violation",
    message: "First class cabin detected. Company policy only allows Economy or Business."
);
```

**When to Use:**
- Cabin class violations (First class bookings)
- Spending cap violations
- Non-compliant bookings
- Called by PolicyComplianceService

---

#### 3. **Missing Documentation Notification**
```csharp
await NotificationService.NotifyMissingDocumentationAsync(
    email: "user@example.com",
    transactionId: "TXN-12345",
    amount: 150.00m
);
```

**When to Use:**
- Transaction has no receipt/document
- Compliance requirement
- Called during data validation

---

#### 4. **Trip Validation Needed**
```csharp
await NotificationService.NotifyTripValidationNeededAsync(
    email: "finance@example.com",
    tripId: 42,
    tripName: "NYC Business Trip"
);
```

**When to Use:**
- Trip status changes to "Ready to Validate"
- All transactions linked
- Notifies Finance team or assigned reviewer

---

#### 5. **Trip Owner Notification**
```csharp
await NotificationService.NotifyTripOwnerAsync(
    ownerEmail: "owner@example.com",
    tripId: 42,
    tripName: "NYC Business Trip",
    message: "Your trip has been approved and is ready for final review."
);
```

**When to Use:**
- Trip status changes
- Approval updates
- Action required from trip owner

---

#### 6. **Transaction Linked to Trip**
```csharp
await NotificationService.NotifyTransactionLinkedAsync(
    email: "user@example.com",
    transactionId: "TXN-12345",
    tripId: 42,
    tripName: "NYC Business Trip"
);
```

**When to Use:**
- Transaction successfully linked to trip (manual or automatic)
- Confirmation notification
- Low priority

---

#### 7. **Finance Team Alert**
```csharp
await NotificationService.NotifyFinanceTeamAsync(
    title: "Urgent: Approval Required",
    message: "5 trips are pending validation and require immediate review.",
    actionUrl: "/reports/trip-validation"
);
```

**When to Use:**
- System-wide alerts
- Batch processing notifications
- Urgent finance team action needed
- Sends to all users in Finance/Accounting departments

---

#### 8. **Tax Compliance Issue**
```csharp
await NotificationService.NotifyTaxComplianceIssueAsync(
    email: "user@example.com",
    tripId: 42,
    tripName: "NYC Business Trip",
    exposure: 450.00m
);
```

**When to Use:**
- Trip exceeds tax caps (meals/lodging)
- Tax exposure calculated
- High priority compliance alert

---

### Method 2: Create Custom Notification

For custom scenarios not covered by helper methods:

```csharp
var notification = new Notification
{
    RecipientEmail = "user@example.com",
    Type = NotificationType.Warning,           // Info, Success, Warning, Error
    Category = NotificationCategory.Transaction, // See categories below
    Priority = NotificationPriority.High,       // Low, Normal, High, Urgent
    Title = "Custom Notification Title",
    Message = "Your detailed message here...",
    ActionUrl = "/custom/page/123",
    ActionLabel = "View Details",
    RelatedEntityId = "ENTITY-123",
    RelatedEntityType = "CustomEntity",
    Icon = "🔥",
    ExpiresAt = DateTime.UtcNow.AddDays(30)   // Optional, default 90 days
};

await NotificationService.CreateNotificationAsync(notification);
```

---

## Notification Service Functions

### Core Functions

#### 1. **GetNotificationsByEmailAsync**
```csharp
// Get all notifications for a user
var allNotifications = await NotificationService.GetNotificationsByEmailAsync("user@example.com");

// Get only unread notifications
var unreadOnly = await NotificationService.GetNotificationsByEmailAsync("user@example.com", unreadOnly: true);
```

**Returns:** List of notifications ordered by CreatedAt (newest first)

---

#### 2. **GetUnreadCountAsync**
```csharp
int unreadCount = await NotificationService.GetUnreadCountAsync("user@example.com");
```

**Returns:** Count of unread notifications
**Use Case:** Display badge on notification bell icon

---

#### 3. **GetNotificationByIdAsync**
```csharp
var notification = await NotificationService.GetNotificationByIdAsync(42);
```

**Returns:** Single notification or null
**Use Case:** Retrieve specific notification details

---

#### 4. **CreateNotificationAsync**
```csharp
var notification = new Notification { /* properties */ };
var created = await NotificationService.CreateNotificationAsync(notification);
```

**Returns:** Created notification with ID
**Side Effects:**
- Sets CreatedAt to current UTC time
- Sets default ExpiresAt to 90 days if not specified
- Logs creation

---

#### 5. **MarkAsReadAsync**
```csharp
await NotificationService.MarkAsReadAsync(notificationId: 42);
```

**Side Effects:**
- Sets IsRead = true
- Sets ReadAt = current UTC time
- Logs action

---

#### 6. **MarkAllAsReadAsync**
```csharp
await NotificationService.MarkAllAsReadAsync("user@example.com");
```

**Side Effects:**
- Marks all unread notifications as read for user
- Updates ReadAt timestamp
- Logs count of notifications marked

---

#### 7. **DeleteNotificationAsync**
```csharp
await NotificationService.DeleteNotificationAsync(notificationId: 42);
```

**Side Effects:**
- Permanently deletes notification
- Logs deletion

---

#### 8. **DeleteAllReadAsync**
```csharp
await NotificationService.DeleteAllReadAsync("user@example.com");
```

**Side Effects:**
- Deletes all read notifications for user
- Cleans up notification history
- Logs count of deleted notifications

---

#### 9. **DeleteExpiredNotificationsAsync**
```csharp
await NotificationService.DeleteExpiredNotificationsAsync();
```

**Use Case:** Run as scheduled background job
**Side Effects:**
- Deletes all notifications where ExpiresAt < current time
- Automatic cleanup
- Logs count of expired notifications removed

---

## Notification Page Features

### User Interface Elements

#### 1. **Header Section**
- Page title: "🔔 Notifications"
- Action buttons:
  - **Refresh** - Reload notifications
  - **Mark All Read (count)** - Only shown if unread exist
  - **Clear Read** - Only shown if read notifications exist

#### 2. **Filter Tabs**
- **All** - Shows all notifications with count
- **Unread** - Filters to unread only
- **Transactions** - Transaction-related notifications
- **Trips** - Trip-related notifications
- **Policy** - Policy violation notifications

#### 3. **Notification Card**
Each notification displays:
- **Icon** - Visual indicator (emoji)
- **Title** - Notification subject
- **Badges:**
  - "New" badge if unread
  - Priority badge (Urgent/High Priority)
- **Message** - Detailed description
- **Metadata:**
  - Category badge
  - Time ago (e.g., "2h ago", "3d ago")
- **Action Button** - If ActionUrl exists
- **Menu** - Mark as read, Delete options

#### 4. **Visual Indicators**
- **Unread notifications:** Blue left border, darker text
- **Read notifications:** No border, lighter text
- **Empty state:** Shows friendly message with emoji

#### 5. **Pagination**
Automatically shown if more than 10 notifications

---

### User Actions

| Action | Description | What Happens |
|--------|-------------|--------------|
| **Click notification** | View details | - |
| **Click action button** | Navigate to related item | Marks as read, then navigates |
| **Mark as Read** | Mark single notification | Updates IsRead, sets ReadAt |
| **Mark All Read** | Mark all unread | Bulk update all unread to read |
| **Delete** | Remove notification | Permanent deletion |
| **Clear Read** | Remove all read | Bulk delete all read notifications |
| **Refresh** | Reload data | Fetches latest from database |
| **Filter tabs** | Change view | Filters by category/read status |

---

## Notification Types and Categories

### Notification Types
```csharp
NotificationType.Info      // 📢 Informational
NotificationType.Success   // ✅ Success/confirmation
NotificationType.Warning   // ⚠️ Warning/attention needed
NotificationType.Error     // ❌ Error/critical issue
```

### Notification Categories
```csharp
NotificationCategory.Transaction      // Transaction-related
NotificationCategory.Trip            // Trip-related
NotificationCategory.PolicyViolation // Policy compliance issues
NotificationCategory.Validation      // Validation/approval needed
NotificationCategory.Approval        // Approval workflows
NotificationCategory.System          // System-wide notifications
NotificationCategory.TaxCompliance   // Tax compliance issues
NotificationCategory.Documentation   // Missing docs/receipts
```

### Priority Levels
```csharp
NotificationPriority.Low     // No badge, informational
NotificationPriority.Normal  // No badge, standard
NotificationPriority.High    // "High Priority" badge (warning color)
NotificationPriority.Urgent  // "Urgent" badge (error color)
```

---

## Usage Examples

### Example 1: Notify User of High-Value Meal
```csharp
// In TransactionService or Control logic
if (transaction.Category.Name == "Meals" && transaction.AmountUSD >= 80)
{
    await _notificationService.NotifyHighValueTransactionAsync(
        email: transaction.Email,
        transactionId: transaction.TransactionId,
        amount: transaction.AmountUSD,
        category: "Meals"
    );
}
```

### Example 2: Notify on First Class Flight Booking
```csharp
// In PolicyComplianceService
if (transaction.CabinClass?.Name == "First")
{
    await _notificationService.NotifyPolicyViolationAsync(
        email: transaction.Email,
        transactionId: transaction.TransactionId,
        violationType: "First Class Cabin",
        message: "First class cabin detected. Per company policy, maximum cabin class allowed is Business."
    );
}
```

### Example 3: Notify Finance Team of Pending Validations
```csharp
// In scheduled job or batch process
var pendingTripsCount = await GetPendingValidationCountAsync();

if (pendingTripsCount > 0)
{
    await _notificationService.NotifyFinanceTeamAsync(
        title: $"{pendingTripsCount} Trips Awaiting Validation",
        message: "Multiple trips are ready for validation and require review.",
        actionUrl: "/reports/trip-validation"
    );
}
```

### Example 4: Notify User When Trip Status Changes
```csharp
// In TripService after status update
await _notificationService.NotifyTripOwnerAsync(
    ownerEmail: trip.Email,
    tripId: trip.TripId,
    tripName: trip.TripName,
    message: $"Your trip status has been updated to '{trip.Status.Name}'. No further action required at this time."
);
```

### Example 5: Create Custom Approval Notification
```csharp
var notification = new Notification
{
    RecipientEmail = approverEmail,
    Type = NotificationType.Info,
    Category = NotificationCategory.Approval,
    Priority = NotificationPriority.High,
    Title = "Approval Required: Budget Override",
    Message = $"Trip '{trip.TripName}' requires budget override approval due to exceeding department limits by ${overageAmount:N2}.",
    ActionUrl = $"/approvals/trip/{trip.TripId}",
    ActionLabel = "Review & Approve",
    RelatedEntityId = trip.TripId.ToString(),
    RelatedEntityType = "Trip",
    Icon = "⚖️",
    ExpiresAt = DateTime.UtcNow.AddDays(7) // Urgent, expires in 7 days
};

await _notificationService.CreateNotificationAsync(notification);
```

---

## Best Practices

### 1. **Use Helper Methods When Available**
- They include proper icons, priorities, and formatting
- Consistent user experience
- Reduces code duplication

### 2. **Set Appropriate Priority Levels**
```csharp
// Low - FYI, no action needed
NotificationPriority.Low      // "Transaction linked successfully"

// Normal - Standard notifications
NotificationPriority.Normal   // "Trip ready for validation"

// High - Action recommended
NotificationPriority.High     // "Policy violation detected"

// Urgent - Immediate action required
NotificationPriority.Urgent   // "Compliance deadline approaching"
```

### 3. **Always Provide Action URLs**
Users should be able to click and go directly to the related item:
```csharp
ActionUrl = $"/transactions?search={transactionId}",  // To transaction
ActionUrl = $"/trips/{tripId}",                       // To trip detail
ActionUrl = "/reports/trip-validation",               // To validation page
```

### 4. **Use Descriptive Titles and Messages**
```csharp
// ❌ Bad
Title = "Alert"
Message = "Please check transaction"

// ✅ Good
Title = "High-Value Meal Transaction"
Message = "A meal expense of $150.00 has been recorded. Please verify this expense and ensure proper documentation is attached."
```

### 5. **Set Expiration Dates**
```csharp
// Default: 90 days (auto-set if not specified)
ExpiresAt = DateTime.UtcNow.AddDays(90)

// Short-lived for urgent items
ExpiresAt = DateTime.UtcNow.AddDays(7)

// Long-lived for reference
ExpiresAt = DateTime.UtcNow.AddDays(180)
```

### 6. **Don't Over-Notify**
- Avoid duplicate notifications for same event
- Check if notification already exists before creating
- Use appropriate priority (don't make everything urgent)

### 7. **Include Metadata**
```csharp
RelatedEntityId = transactionId,      // For tracking and linking
RelatedEntityType = "Transaction",    // For querying
Icon = "💰"                           // Visual indicator
```

### 8. **Schedule Cleanup**
Create a background job to remove expired notifications:
```csharp
// In background service (Hangfire, Quartz, etc.)
public async Task CleanupExpiredNotificationsAsync()
{
    await _notificationService.DeleteExpiredNotificationsAsync();
}
```

### 9. **Batch Notifications for Finance Team**
Instead of notifying for each item individually:
```csharp
// ✅ Good - Single notification with summary
await _notificationService.NotifyFinanceTeamAsync(
    title: "Daily Summary: 15 Items Require Attention",
    message: "5 trips pending validation, 8 policy violations, 2 tax compliance issues.",
    actionUrl: "/dashboard"
);
```

### 10. **Consider Future Email Integration**
The notification entity includes `EmailSent` and `EmailSentAt` fields for future email integration:
```csharp
// Future implementation
if (notification.Priority == NotificationPriority.Urgent)
{
    await SendEmailNotification(notification);
    notification.EmailSent = true;
    notification.EmailSentAt = DateTime.UtcNow;
}
```

---

## Time Display Logic

The notification page shows relative time:
- **Just now** - < 1 minute
- **15m ago** - < 1 hour
- **3h ago** - < 24 hours
- **2d ago** - < 7 days
- **3w ago** - < 30 days
- **Oct 27, 2024** - >= 30 days

---

## Integration Points

### Where Notifications Are Triggered

| Location | Trigger | Method Called |
|----------|---------|---------------|
| **TransactionService** | High-value transaction imported | `NotifyHighValueTransactionAsync` |
| **TransactionService** | Transaction linked to trip | `NotifyTransactionLinkedAsync` |
| **PolicyComplianceService** | Policy violation detected | `NotifyPolicyViolationAsync` |
| **TripValidation** | Trip ready for validation | `NotifyTripValidationNeededAsync` |
| **TripService** | Trip status changed | `NotifyTripOwnerAsync` |
| **Data Integrity Controls** | Missing documentation | `NotifyMissingDocumentationAsync` |
| **Tax Calculation** | Tax compliance issue | `NotifyTaxComplianceIssueAsync` |
| **Batch Processing** | System-wide alerts | `NotifyFinanceTeamAsync` |

---

## Database Schema

```sql
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY,
    RecipientEmail NVARCHAR(255) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Title NVARCHAR(500) NOT NULL,
    Message NVARCHAR(2000) NOT NULL,
    ActionUrl NVARCHAR(500),
    ActionLabel NVARCHAR(100),
    RelatedEntityId NVARCHAR(100),
    RelatedEntityType NVARCHAR(50),
    Priority NVARCHAR(20) DEFAULT 'Normal',
    IsRead BIT DEFAULT 0,
    ReadAt DATETIME,
    EmailSent BIT DEFAULT 0,
    EmailSentAt DATETIME,
    ExpiresAt DATETIME,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    Icon NVARCHAR(10),
    
    INDEX IX_Notifications_RecipientEmail (RecipientEmail),
    INDEX IX_Notifications_IsRead (IsRead),
    INDEX IX_Notifications_CreatedAt (CreatedAt),
    INDEX IX_Notifications_ExpiresAt (ExpiresAt)
);
```

---

---

## Real-World Scenario: Employee Creates Meal Transaction

### 📝 **Example Transaction**
```
Transaction ID: 5d66566d-f819-4ae0-ae24-c4784d206cf1
Date: 27/10/2025
Email: john.doe@wsc.com
Category: Meals
Amount: $1,234.00
Source: Manual
Status: Unlinked (🔓)
```

### 🔔 **Who Gets Notified?**

#### ✅ **Employee (john.doe@wsc.com) - DOES GET NOTIFICATION**
**Condition:** Transaction amount ≥ $1,000
```csharp
// In TransactionService.CreateTransactionAsync()
if (transaction.AmountUSD >= 1000)
{
    await _notificationService.NotifyHighValueTransactionAsync(
        email: "john.doe@wsc.com",  // Employee receives notification
        transactionId: "5d66566d-f819-4ae0-ae24-c4784d206cf1",
        amount: 1234.00m,
        category: "Meals"
    );
}
```

**Notification Details:**
- **Type:** Warning ⚠️
- **Category:** Transaction
- **Priority:** High
- **Title:** "High-Value Meals Transaction"
- **Message:** "A transaction of $1,234.00 has been recorded. Please verify this expense and ensure proper documentation is attached."
- **Action:** "View Transaction" button linking to transaction page
- **Icon:** 💰

---

#### ❌ **Finance Team - DOES NOT GET NOTIFICATION**
**Current Behavior:** Finance is NOT automatically notified when:
- Employee creates a meal transaction
- Transaction is high-value ($1,234)
- Transaction is unlinked to a trip
- Transaction has no documentation

**Why?** The system only notifies the **transaction owner** (employee), not the Finance team.

---

### 🎯 **When DOES Finance Get Notified?**

Finance receives notifications in these scenarios:

#### 1. **Manual Finance Team Alert (Batch Processing)**
```csharp
await _notificationService.NotifyFinanceTeamAsync(
    title: "5 High-Value Transactions Require Review",
    message: "Multiple high-value transactions have been submitted today.",
    actionUrl: "/data-integrity/controls/meals"
);
```

#### 2. **Policy Violation Detected**
```csharp
// If meal exceeds $80 AND appears on Meals Control page
// Finance must manually check the control page
```

#### 3. **Trip Validation Request**
```csharp
// When trip status changes to "Ready to Validate"
await _notificationService.NotifyTripValidationNeededAsync(
    email: "finance@wsc.com",
    tripId: 42,
    tripName: "NYC Business Trip"
);
```

---

### 🔍 **How Finance Discovers High-Value Meals**

Instead of notifications, Finance uses **Data Integrity Control Pages**:

#### **Meals Control Page** (`/data-integrity/controls/meals`)
- Automatically shows meals ≥ $80
- Filters: `WHERE Category = 'Meals' AND AmountUSD >= 80 AND IsValid = FALSE`
- Finance reviews this page regularly
- **Your $1,234 meal WILL appear here**

**What Finance Sees:**
```
Document | Transaction ID | Email | Date | Vendor | Amount (USD) | Category
---------|----------------|-------|------|--------|--------------|----------
[📄]     | 5d66566d...    | john.doe | 27/10/2025 | fida | $1,234.00 | Meals
```

**Actions Finance Can Take:**
1. ✅ Mark as Valid
2. 📝 Update Category
3. 💬 Generate Message Template (to ask employee for details)
4. ➕ Add Participants (if client entertainment)

---

### 💡 **Recommendation: Add Finance Notification for High-Value Meals**

**Problem:** Finance must manually check control pages to discover high-value transactions.

**Solution:** Add automatic Finance notification for high-value meals (≥ $200):

```csharp
// In TransactionService.CreateTransactionAsync()
// After notifying employee

if (categoryName == "Meals" && transaction.AmountUSD >= 200)
{
    // Also notify Finance team
    await _notificationService.NotifyFinanceTeamAsync(
        title: "High-Value Meal Transaction Submitted",
        message: $"Employee {transaction.Email} submitted a meal expense of ${transaction.AmountUSD:N2}. Review required.",
        actionUrl: $"/transactions?search={transaction.TransactionId}"
    );
}
```

**This would notify:**
- All users in Finance department
- All users in Accounting department
- Provides direct link to transaction

---

### 📋 **Complete Notification Flow for Your Example**

```
1. Employee (john.doe@wsc.com) creates meal transaction
   ↓
2. TransactionService.CreateTransactionAsync() is called
   ↓
3. Transaction saved to database
   ↓
4. Audit log created
   ↓
5. ✅ NOTIFICATION SENT TO EMPLOYEE:
   - To: john.doe@wsc.com
   - Title: "High-Value Meals Transaction"
   - Amount: $1,234.00
   - Priority: High
   ↓
6. ❌ NO NOTIFICATION TO FINANCE
   ↓
7. Transaction appears on Meals Control page (≥ $80 threshold)
   ↓
8. Finance manually discovers transaction by checking control page
```

---

### ⚙️ **How to Implement Finance Notifications**

**Step 1:** Modify `TransactionService.CreateTransactionAsync()`:

```csharp
// After line 254 (existing employee notification)
// Add this code:

// Check if Finance team should be notified for high-value meals
if (categoryName == "Meals" && 
    transaction.AmountUSD.HasValue && 
    Math.Abs(transaction.AmountUSD.Value) >= 200)
{
    try
    {
        await _notificationService.NotifyFinanceTeamAsync(
            title: $"High-Value Meal: ${Math.Abs(transaction.AmountUSD.Value):N2}",
            message: $"Employee {transaction.Email} submitted a meal transaction that requires review. " +
                     $"Vendor: {transaction.Vendor ?? "N/A"}, Amount: ${Math.Abs(transaction.AmountUSD.Value):N2}",
            actionUrl: $"/data-integrity/controls/meals"
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to notify Finance team: {ex.Message}");
    }
}
```

**Step 2:** Adjust thresholds as needed:
- **$200+** → High priority, notify Finance immediately
- **$500+** → Urgent priority
- **$1,000+** → Urgent + email notification (future)

---

### 🎯 **Best Practice: Multi-Level Notification Strategy**

```csharp
// Threshold-based notifications
var amount = Math.Abs(transaction.AmountUSD.Value);

if (amount >= 1000)
{
    // Notify employee (current behavior)
    await _notificationService.NotifyHighValueTransactionAsync(...);
    
    // ALSO notify Finance - Urgent
    await _notificationService.NotifyFinanceTeamAsync(
        title: $"⚠️ URGENT: ${amount:N2} Meal Transaction",
        message: "Requires immediate review",
        actionUrl: $"/transactions?search={transactionId}"
    );
}
else if (amount >= 200)
{
    // Notify employee
    await _notificationService.NotifyHighValueTransactionAsync(...);
    
    // Notify Finance - Normal priority
    await _notificationService.NotifyFinanceTeamAsync(
        title: $"High-Value Meal: ${amount:N2}",
        message: "Review when convenient",
        actionUrl: "/data-integrity/controls/meals"
    );
}
```

---

## Summary

The Notifications System provides a robust, flexible way to alert users about important events in the Travel Expense Management System. Use the helper methods for common scenarios, set appropriate priorities, and always provide actionable links. Remember to schedule periodic cleanup of expired notifications to maintain system performance.

**Current Limitation:** Finance team is NOT automatically notified of high-value transactions. They must manually check Data Integrity Control pages.

**Recommended Enhancement:** Add automatic Finance notifications for transactions above specific thresholds.

For questions or enhancements, refer to:
- `NotificationService.cs` - Service implementation
- `INotificationService.cs` - Service interface
- `Notification.cs` - Entity model
- `Notifications.razor` - UI page
- `TransactionService.cs` - Transaction creation logic
