using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

/// <summary>
/// Service for managing notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Get all notifications (Finance only)
    /// </summary>
    Task<List<Notification>> GetAllNotificationsAsync();

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
    /// Notify finance team about new transaction that requires review
    /// </summary>
    Task NotifyFinanceTeamNewTransactionAsync(string transactionId, string employeeEmail, string categoryName, decimal amount, string? vendor = null);

    /// <summary>
    /// Notify about tax compliance issue
    /// </summary>
    Task NotifyTaxComplianceIssueAsync(string email, int tripId, string tripName, decimal exposure);

    /// <summary>
    /// Notify employee when their transaction is validated by Finance
    /// </summary>
    Task NotifyEmployeeTransactionValidatedAsync(string employeeEmail, string transactionId, string categoryName, decimal amount, string? financeUserEmail = null);

    /// <summary>
    /// Notify employee when Finance has questions about their transaction
    /// </summary>
    Task NotifyEmployeeInquiryAsync(string employeeEmail, string transactionId, string categoryName, string inquiryReason);

    /// <summary>
    /// Notify employee when their trip is validated/approved by Finance
    /// </summary>
    Task NotifyEmployeeTripValidatedAsync(string employeeEmail, int tripId, string tripName, DateTime startDate, DateTime endDate, decimal totalAmount, string? financeUserEmail = null);

    /// <summary>
    /// Notify employee when their transaction is linked to a trip by Finance
    /// </summary>
    Task NotifyEmployeeTransactionLinkedToTripAsync(string employeeEmail, string transactionId, int tripId, string tripName, decimal amount, string? financeUserEmail = null);

    /// <summary>
    /// Notify employee when their transaction is unlinked from a trip by Finance
    /// </summary>
    Task NotifyEmployeeTransactionUnlinkedFromTripAsync(string employeeEmail, string transactionId, string tripName, decimal amount, string? financeUserEmail = null);

    /// <summary>
    /// Notify employee when their transaction is split by Finance
    /// </summary>
    Task NotifyEmployeeTransactionSplitAsync(string employeeEmail, string originalTransactionId, int splitCount, decimal originalAmount, string? financeUserEmail = null);
}
