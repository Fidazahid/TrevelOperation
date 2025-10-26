using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

/// <summary>
/// Service for managing notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Get all notifications for a specific user
    /// </summary>
    Task<List<Notification>> GetNotificationsByEmailAsync(string email, bool unreadOnly = false);

    /// <summary>
    /// Get unread notification count for a user
    /// </summary>
    Task<int> GetUnreadCountAsync(string email);

    /// <summary>
    /// Get notification by ID
    /// </summary>
    Task<Notification?> GetNotificationByIdAsync(int notificationId);

    /// <summary>
    /// Create a new notification
    /// </summary>
    Task<Notification> CreateNotificationAsync(Notification notification);

    /// <summary>
    /// Mark notification as read
    /// </summary>
    Task MarkAsReadAsync(int notificationId);

    /// <summary>
    /// Mark all notifications as read for a user
    /// </summary>
    Task MarkAllAsReadAsync(string email);

    /// <summary>
    /// Delete a notification
    /// </summary>
    Task DeleteNotificationAsync(int notificationId);

    /// <summary>
    /// Delete all read notifications for a user
    /// </summary>
    Task DeleteAllReadAsync(string email);

    /// <summary>
    /// Delete expired notifications (cleanup)
    /// </summary>
    Task DeleteExpiredNotificationsAsync();

    // Helper methods to create specific notification types

    /// <summary>
    /// Notify about a high-value transaction
    /// </summary>
    Task NotifyHighValueTransactionAsync(string email, string transactionId, decimal amount, string category);

    /// <summary>
    /// Notify about policy violation
    /// </summary>
    Task NotifyPolicyViolationAsync(string email, string transactionId, string violationType, string message);

    /// <summary>
    /// Notify about missing documentation
    /// </summary>
    Task NotifyMissingDocumentationAsync(string email, string transactionId, decimal amount);

    /// <summary>
    /// Notify about trip validation needed
    /// </summary>
    Task NotifyTripValidationNeededAsync(string email, int tripId, string tripName);

    /// <summary>
    /// Notify trip owner about their trip
    /// </summary>
    Task NotifyTripOwnerAsync(string ownerEmail, int tripId, string tripName, string message);

    /// <summary>
    /// Notify about transaction linked to trip
    /// </summary>
    Task NotifyTransactionLinkedAsync(string email, string transactionId, int tripId, string tripName);

    /// <summary>
    /// Notify finance team about approval needed
    /// </summary>
    Task NotifyFinanceTeamAsync(string title, string message, string? actionUrl = null);

    /// <summary>
    /// Notify about tax compliance issue
    /// </summary>
    Task NotifyTaxComplianceIssueAsync(string email, int tripId, string tripName, decimal exposure);
}
