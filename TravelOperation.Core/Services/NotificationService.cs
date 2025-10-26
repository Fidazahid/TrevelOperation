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
        notification.CreatedAt = DateTime.UtcNow;
        
        // Set default expiration to 90 days if not specified
        if (notification.ExpiresAt == null)
        {
            notification.ExpiresAt = DateTime.UtcNow.AddDays(90);
        }

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Created notification {NotificationId} for {Email}: {Title}",
            notification.NotificationId,
            notification.RecipientEmail,
            notification.Title);

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

        foreach (var email in financeEmails)
        {
            var notification = new Notification
            {
                RecipientEmail = email,
                Type = NotificationType.Info,
                Category = NotificationCategory.System,
                Priority = NotificationPriority.High,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                ActionLabel = actionUrl != null ? "Take Action" : null,
                Icon = "💼"
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

    private async Task<List<string>> GetFinanceTeamEmailsAsync()
    {
        // Get users with Finance role from Headcount or Users table
        var financeEmails = await _context.Headcount
            .Where(h => h.Department == "Finance" || h.Department == "Accounting")
            .Select(h => h.Email)
            .Distinct()
            .ToListAsync();

        if (!financeEmails.Any())
        {
            // If no finance users found in headcount, log a warning
            _logger.LogWarning("No finance users found in Headcount table for notifications");
        }

        return financeEmails;
    }
}
