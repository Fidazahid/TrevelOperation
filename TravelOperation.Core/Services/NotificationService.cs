using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Services;

public class NotificationService : INotificationService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        TravelDbContext context,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Notification>> GetAllNotificationsAsync()
    {
        return await _context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsByEmailAsync(string email, bool unreadOnly = false)
    {
        var query = _context.Notifications
            .Where(n => n.RecipientEmail == email);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string email)
    {
        return await _context.Notifications
            .Where(n => n.RecipientEmail == email && !n.IsRead)
            .CountAsync();
    }

    public async Task<Notification?> GetNotificationByIdAsync(int notificationId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
    }

    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        // Validate recipient email
        if (string.IsNullOrWhiteSpace(notification.RecipientEmail))
        {
            _logger.LogWarning("Attempted to create notification without recipient email. Title: {Title}", notification.Title);
            throw new ArgumentException("RecipientEmail is required for notifications");
        }

        notification.CreatedAt = DateTime.UtcNow;
        
        // Set default expiration to 90 days if not specified
        if (notification.ExpiresAt == null)
        {
            notification.ExpiresAt = DateTime.UtcNow.AddDays(90);
        }

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "✅ NOTIFICATION CREATED - ID: {NotificationId}, Email: {Email}, Title: {Title}, Type: {Type}, Category: {Category}",
            notification.NotificationId,
            notification.RecipientEmail,
            notification.Title,
            notification.Type,
            notification.Category);

        return notification;
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await GetNotificationByIdAsync(notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogDebug("Marked notification {NotificationId} as read", notificationId);
        }
    }

    public async Task MarkAllAsReadAsync(string email)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.RecipientEmail == email && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Marked {Count} notifications as read for {Email}",
            unreadNotifications.Count,
            email);
    }

    public async Task DeleteNotificationAsync(int notificationId)
    {
        var notification = await GetNotificationByIdAsync(notificationId);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted notification {NotificationId}", notificationId);
        }
    }

    public async Task DeleteAllReadAsync(string email)
    {
        var readNotifications = await _context.Notifications
            .Where(n => n.RecipientEmail == email && n.IsRead)
            .ToListAsync();

        _context.Notifications.RemoveRange(readNotifications);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Deleted {Count} read notifications for {Email}",
            readNotifications.Count,
            email);
    }

    public async Task DeleteExpiredNotificationsAsync()
    {
        var expiredNotifications = await _context.Notifications
            .Where(n => n.ExpiresAt.HasValue && n.ExpiresAt.Value < DateTime.UtcNow)
            .ToListAsync();

        _context.Notifications.RemoveRange(expiredNotifications);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted {Count} expired notifications", expiredNotifications.Count);
    }

    // Helper methods to create specific notification types

    public async Task NotifyHighValueTransactionAsync(string email, string transactionId, decimal amount, string category)
    {
        var notification = new Notification
        {
            RecipientEmail = email,
            Type = NotificationType.Warning,
            Category = NotificationCategory.Transaction,
            Priority = NotificationPriority.High,
            Title = $"High-Value {category} Transaction",
            Message = $"A transaction of ${amount:N2} has been recorded. Please verify this expense and ensure proper documentation is attached.",
            ActionUrl = $"/transactions?search={transactionId}",
            ActionLabel = "View Transaction",
            RelatedEntityId = transactionId,
            RelatedEntityType = "Transaction",
            Icon = "💰"
        };

        await CreateNotificationAsync(notification);
    }

    public async Task NotifyPolicyViolationAsync(string email, string transactionId, string violationType, string message)
    {
        var notification = new Notification
        {
            RecipientEmail = email,
            Type = NotificationType.Warning,
            Category = NotificationCategory.PolicyViolation,
            Priority = NotificationPriority.High,
            Title = $"Policy Violation: {violationType}",
            Message = message,
            ActionUrl = $"/transactions?search={transactionId}",
            ActionLabel = "Review Transaction",
            RelatedEntityId = transactionId,
            RelatedEntityType = "Transaction",
            Icon = "⚠️"
        };

        await CreateNotificationAsync(notification);
    }

    public async Task NotifyMissingDocumentationAsync(string email, string transactionId, decimal amount)
    {
        var notification = new Notification
        {
            RecipientEmail = email,
            Type = NotificationType.Warning,
            Category = NotificationCategory.Documentation,
            Priority = NotificationPriority.Normal,
            Title = "Missing Receipt/Documentation",
            Message = $"Transaction {transactionId} (${amount:N2}) is missing required documentation. Please upload a receipt to maintain compliance.",
            ActionUrl = $"/transactions?search={transactionId}",
            ActionLabel = "Upload Receipt",
            RelatedEntityId = transactionId,
            RelatedEntityType = "Transaction",
            Icon = "📄"
        };

        await CreateNotificationAsync(notification);
    }

    public async Task NotifyTripValidationNeededAsync(string email, int tripId, string tripName)
    {
        var notification = new Notification
        {
            RecipientEmail = email,
            Type = NotificationType.Info,
            Category = NotificationCategory.Validation,
            Priority = NotificationPriority.Normal,
            Title = "Trip Ready for Validation",
            Message = $"Trip '{tripName}' is ready for validation. Please review transactions and approve.",
            ActionUrl = $"/trips/{tripId}",
            ActionLabel = "Review Trip",
            RelatedEntityId = tripId.ToString(),
            RelatedEntityType = "Trip",
            Icon = "✈️"
        };

        await CreateNotificationAsync(notification);
    }

    public async Task NotifyTripOwnerAsync(string ownerEmail, int tripId, string tripName, string message)
    {
        var notification = new Notification
        {
            RecipientEmail = ownerEmail,
            Type = NotificationType.Info,
            Category = NotificationCategory.Trip,
            Priority = NotificationPriority.Normal,
            Title = $"Trip Update: {tripName}",
            Message = message,
            ActionUrl = $"/trips/{tripId}",
            ActionLabel = "View Trip",
            RelatedEntityId = tripId.ToString(),
            RelatedEntityType = "Trip",
            Icon = "📢"
        };

        await CreateNotificationAsync(notification);
    }

    public async Task NotifyTransactionLinkedAsync(string email, string transactionId, int tripId, string tripName)
    {
        var notification = new Notification
        {
            RecipientEmail = email,
            Type = NotificationType.Success,
            Category = NotificationCategory.Transaction,
            Priority = NotificationPriority.Low,
            Title = "Transaction Linked to Trip",
            Message = $"Transaction {transactionId} has been successfully linked to trip '{tripName}'.",
            ActionUrl = $"/trips/{tripId}",
            ActionLabel = "View Trip",
            RelatedEntityId = transactionId,
            RelatedEntityType = "Transaction",
            Icon = "🔗"
        };

        await CreateNotificationAsync(notification);
    }

    public async Task NotifyFinanceTeamAsync(string title, string message, string? actionUrl = null)
    {
        // Get finance team emails from system settings or headcount
        var financeEmails = await GetFinanceTeamEmailsAsync();

        // Extract entity type and ID from actionUrl if present (e.g., /trips/123 or /transactions?search=xxx)
        string? relatedEntityType = null;
        string? relatedEntityId = null;
        
        if (!string.IsNullOrEmpty(actionUrl))
        {
            if (actionUrl.StartsWith("/trips/"))
            {
                relatedEntityType = "Trip";
                relatedEntityId = actionUrl.Replace("/trips/", "").Split('?')[0];
            }
            else if (actionUrl.Contains("/transactions"))
            {
                relatedEntityType = "Transaction";
                // Try to extract transaction ID from search parameter
                var searchParam = actionUrl.Split("search=").LastOrDefault();
                if (!string.IsNullOrEmpty(searchParam))
                {
                    relatedEntityId = searchParam.Split('&')[0];
                }
            }
        }

        foreach (var email in financeEmails)
        {
            var notification = new Notification
            {
                RecipientEmail = email,
                Type = NotificationType.Info,
                Category = relatedEntityType == "Trip" ? NotificationCategory.Trip : NotificationCategory.System,
                Priority = NotificationPriority.High,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                ActionLabel = actionUrl != null ? "View Details" : null,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId,
                Icon = relatedEntityType == "Trip" ? "✈️" : "💼"
            };

            await CreateNotificationAsync(notification);
        }
    }

    public async Task NotifyTaxComplianceIssueAsync(string email, int tripId, string tripName, decimal exposure)
    {
        var notification = new Notification
        {
            RecipientEmail = email,
            Type = NotificationType.Warning,
            Category = NotificationCategory.TaxCompliance,
            Priority = NotificationPriority.High,
            Title = "Tax Compliance Issue Detected",
            Message = $"Trip '{tripName}' has a tax exposure of ${exposure:N2}. Please review and take necessary action.",
            ActionUrl = $"/trips/{tripId}",
            ActionLabel = "Review Trip",
            RelatedEntityId = tripId.ToString(),
            RelatedEntityType = "Trip",
            Icon = "💸"
        };

        await CreateNotificationAsync(notification);
    }

    public async Task NotifyEmployeeTransactionValidatedAsync(string createdByUserId, string transactionId, string categoryName, decimal amount, string? financeUserEmail = null)
    {
        // Get the registered user's email from the database using UserId
        var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.UserId == createdByUserId);
        
        if (user == null)
        {
            _logger.LogWarning("Cannot send notification: User with ID {UserId} not found", createdByUserId);
            return;
        }

        var employeeEmail = user.Email;

        _logger.LogInformation(
            "🔔 CREATING Transaction Validation Notification - UserId: {UserId}, Email: {Email}, TransactionId: {TransactionId}, Category: {Category}, CreatedBy: {CreatedBy}",
            createdByUserId,
            employeeEmail,
            transactionId,
            categoryName,
            financeUserEmail ?? "System");

        var notification = new Notification
        {
            RecipientEmail = employeeEmail, // This is now the registered user's current email
            Type = NotificationType.Success,
            Category = NotificationCategory.Transaction,
            Priority = NotificationPriority.Low,
            Title = $"{categoryName} Transaction Validated ✅",
            Message = $"Your {categoryName.ToLower()} transaction of ${amount:N2} has been validated by Finance. No further action required.",
            ActionUrl = $"/transactions?search={transactionId}",
            ActionLabel = "View Transaction",
            RelatedEntityId = transactionId,
            RelatedEntityType = "Transaction",
            Icon = "✅",
            CreatedByEmail = financeUserEmail
        };

        await CreateNotificationAsync(notification);

        _logger.LogInformation(
            "✅ Employee notification sent: Transaction {TransactionId} validated for {Email} by {CreatedBy}",
            transactionId,
            employeeEmail,
            financeUserEmail ?? "System");
    }

    public async Task NotifyEmployeeInquiryAsync(string employeeEmail, string transactionId, string categoryName, string inquiryReason)
    {
        var notification = new Notification
        {
            RecipientEmail = employeeEmail,
            Type = NotificationType.Info,
            Category = NotificationCategory.Transaction,
            Priority = NotificationPriority.Normal,
            Title = $"Finance Inquiry: {categoryName} Transaction",
            Message = $"Finance has questions about your {categoryName.ToLower()} transaction. {inquiryReason}",
            ActionUrl = $"/transactions?search={transactionId}",
            ActionLabel = "View Details",
            RelatedEntityId = transactionId,
            RelatedEntityType = "Transaction",
            Icon = "📧"
        };

        await CreateNotificationAsync(notification);

        _logger.LogInformation(
            "Employee inquiry notification sent: Transaction {TransactionId} for {Email}",
            transactionId,
            employeeEmail);
    }

    public async Task NotifyEmployeeTripValidatedAsync(string createdByUserId, int tripId, string tripName, DateTime startDate, DateTime endDate, decimal totalAmount, string? financeUserEmail = null)
    {
        // Get the employee's email from userId
        var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.UserId == createdByUserId);
        if (user == null)
        {
            _logger.LogWarning("Cannot send trip validation notification - User with ID {UserId} not found", createdByUserId);
            return;
        }

        var employeeEmail = user.Email;

        _logger.LogInformation(
            "🔔 CREATING Trip Validation Notification - UserId: {UserId}, Email: {Email}, TripId: {TripId}, TripName: {TripName}, CreatedBy: {CreatedBy}",
            createdByUserId,
            employeeEmail,
            tripId,
            tripName,
            financeUserEmail ?? "System");

        var notification = new Notification
        {
            RecipientEmail = employeeEmail,
            Type = NotificationType.Success,
            Category = NotificationCategory.Validation,
            Priority = NotificationPriority.Normal,
            Title = $"Trip Validated: {tripName} ✅",
            Message = $"Your trip '{tripName}' ({startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}) has been validated and approved by Finance. Total expenses: ${totalAmount:N2}.",
            ActionUrl = $"/trips/{tripId}",
            ActionLabel = "View Trip Details",
            RelatedEntityId = tripId.ToString(),
            RelatedEntityType = "Trip",
            Icon = "✅",
            CreatedByEmail = financeUserEmail
        };

        await CreateNotificationAsync(notification);

        _logger.LogInformation(
            "✅ Trip validation notification sent: Trip {TripId} ({TripName}) for {Email} by {CreatedBy}",
            tripId,
            tripName,
            employeeEmail,
            financeUserEmail ?? "System");
    }

    public async Task NotifyEmployeeTransactionLinkedToTripAsync(string createdByUserId, string transactionId, int tripId, string tripName, decimal amount, string? financeUserEmail = null)
    {
        // Get the employee's email from userId
        var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.UserId == createdByUserId);
        if (user == null)
        {
            _logger.LogWarning("Cannot send transaction linked notification - User with ID {UserId} not found", createdByUserId);
            return;
        }

        var employeeEmail = user.Email;

        _logger.LogInformation(
            "🔔 CREATING Transaction Linked Notification - UserId: {UserId}, Email: {Email}, TransactionId: {TransactionId}, TripId: {TripId}, CreatedBy: {CreatedBy}",
            createdByUserId,
            employeeEmail,
            transactionId,
            tripId,
            financeUserEmail ?? "System");

        var notification = new Notification
        {
            RecipientEmail = employeeEmail,
            Type = NotificationType.Info,
            Category = NotificationCategory.Transaction,
            Priority = NotificationPriority.Low,
            Title = "Transaction Linked to Trip 🔗",
            Message = $"Your transaction {transactionId} (${amount:N2}) has been linked to trip '{tripName}' by Finance.",
            ActionUrl = $"/trips/{tripId}",
            ActionLabel = "View Trip",
            RelatedEntityId = transactionId,
            RelatedEntityType = "Transaction",
            Icon = "🔗",
            CreatedByEmail = financeUserEmail
        };

        await CreateNotificationAsync(notification);

        _logger.LogInformation(
            "✅ Transaction linked notification sent: {TransactionId} to Trip {TripId} for {Email} by {CreatedBy}",
            transactionId,
            tripId,
            employeeEmail,
            financeUserEmail ?? "System");
    }

    public async Task NotifyEmployeeTransactionUnlinkedFromTripAsync(string createdByUserId, string transactionId, string tripName, decimal amount, string? financeUserEmail = null)
    {
        // Get the employee's email from userId
        var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.UserId == createdByUserId);
        if (user == null)
        {
            _logger.LogWarning("Cannot send transaction unlinked notification - User with ID {UserId} not found", createdByUserId);
            return;
        }

        var employeeEmail = user.Email;

        _logger.LogInformation(
            "🔔 CREATING Transaction Unlinked Notification - UserId: {UserId}, Email: {Email}, TransactionId: {TransactionId}, CreatedBy: {CreatedBy}",
            createdByUserId,
            employeeEmail,
            transactionId,
            financeUserEmail ?? "System");

        var notification = new Notification
        {
            RecipientEmail = employeeEmail,
            Type = NotificationType.Warning,
            Category = NotificationCategory.Transaction,
            Priority = NotificationPriority.Normal,
            Title = "Transaction Unlinked from Trip ⛓️‍💥",
            Message = $"Your transaction {transactionId} (${amount:N2}) has been unlinked from trip '{tripName}' by Finance. Please review.",
            ActionUrl = $"/transactions?search={transactionId}",
            ActionLabel = "View Transaction",
            RelatedEntityId = transactionId,
            RelatedEntityType = "Transaction",
            Icon = "⛓️‍💥",
            CreatedByEmail = financeUserEmail
        };

        await CreateNotificationAsync(notification);

        _logger.LogInformation(
            "✅ Transaction unlinked notification sent: {TransactionId} for {Email} by {CreatedBy}",
            transactionId,
            employeeEmail,
            financeUserEmail ?? "System");
    }

    public async Task NotifyEmployeeTransactionSplitAsync(string createdByUserId, string originalTransactionId, int splitCount, decimal originalAmount, string? financeUserEmail = null)
    {
        // Get the employee's email from userId
        var user = await _context.AuthUsers.FirstOrDefaultAsync(u => u.UserId == createdByUserId);
        if (user == null)
        {
            _logger.LogWarning("Cannot send transaction split notification - User with ID {UserId} not found", createdByUserId);
            return;
        }

        var employeeEmail = user.Email;

        _logger.LogInformation(
            "🔔 CREATING Transaction Split Notification - UserId: {UserId}, Email: {Email}, TransactionId: {TransactionId}, SplitCount: {Count}, CreatedBy: {CreatedBy}",
            createdByUserId,
            employeeEmail,
            originalTransactionId,
            splitCount,
            financeUserEmail ?? "System");

        var notification = new Notification
        {
            RecipientEmail = employeeEmail,
            Type = NotificationType.Info,
            Category = NotificationCategory.Transaction,
            Priority = NotificationPriority.Normal,
            Title = "Transaction Split ✂️",
            Message = $"Your transaction {originalTransactionId} (${originalAmount:N2}) has been split into {splitCount} separate transactions by Finance for proper categorization.",
            ActionUrl = $"/transactions?search={originalTransactionId}",
            ActionLabel = "View Transactions",
            RelatedEntityId = originalTransactionId,
            RelatedEntityType = "Transaction",
            Icon = "✂️",
            CreatedByEmail = financeUserEmail
        };

        await CreateNotificationAsync(notification);

        _logger.LogInformation(
            "✅ Transaction split notification sent: {TransactionId} split into {Count} for {Email} by {CreatedBy}",
            originalTransactionId,
            splitCount,
            employeeEmail,
            financeUserEmail ?? "System");
    }

    public async Task NotifyFinanceTeamNewTransactionAsync(string transactionId, string employeeEmail, string categoryName, decimal amount, string? vendor = null)
    {
        _logger.LogInformation(
            "🔔 CREATING New Transaction Notification for Finance Team - TransactionId: {TransactionId}, Employee: {Employee}, Category: {Category}, Amount: ${Amount}",
            transactionId,
            employeeEmail,
            categoryName,
            amount);

        // Get all finance team members
        var financeEmails = await GetFinanceTeamEmailsAsync();
        
        if (!financeEmails.Any())
        {
            _logger.LogWarning("No finance team members found - cannot send new transaction notifications");
            return;
        }

        // Create notifications for each finance team member
        foreach (var financeEmail in financeEmails)
        {
            var notification = new Notification
            {
                RecipientEmail = financeEmail,
                Type = NotificationType.Info,
                Category = NotificationCategory.Transaction,
                Priority = NotificationPriority.Normal,
                Title = "New Transaction Requires Review 📋",
                Message = $"New {categoryName.ToLower()} transaction submitted by {employeeEmail} for ${amount:N2}{(vendor != null ? $" at {vendor}" : "")}. Please review and validate.",
                ActionUrl = $"/transactions?search={transactionId}",
                ActionLabel = "Review Transaction",
                RelatedEntityId = transactionId,
                RelatedEntityType = "Transaction",
                Icon = "📋",
                CreatedByEmail = employeeEmail
            };

            await CreateNotificationAsync(notification);
        }

        _logger.LogInformation(
            "✅ New transaction notifications sent to {Count} finance team members for transaction {TransactionId}",
            financeEmails.Count,
            transactionId);
    }

    private async Task<List<string>> GetFinanceTeamEmailsAsync()
    {
        // Get ONLY users with Finance role from AuthUsers table
        var financeUsers = await _context.AuthUsers
            .Where(u => u.IsActive && 
                       u.Role != null && 
                       u.Role.ToLower() == "finance")
            .Select(u => u.Email)
            .Distinct()
            .ToListAsync();

        if (!financeUsers.Any())
        {
            _logger.LogWarning("No users with Role='Finance' found. Please add Finance users to the system.");
        }
        else
        {
            _logger.LogInformation("Found {Count} users with Finance role: {Emails}", 
                financeUsers.Count, 
                string.Join(", ", financeUsers));
        }

        return financeUsers;
    }
}
