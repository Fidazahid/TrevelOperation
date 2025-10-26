using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelOperation.Core.Models.Entities;

/// <summary>
/// Notification entity for user alerts and messages
/// </summary>
[Table("Notifications")]
public class Notification
{
    [Key]
    public int NotificationId { get; set; }

    /// <summary>
    /// User email who should receive this notification
    /// </summary>
    [Required]
    [StringLength(255)]
    public string RecipientEmail { get; set; } = string.Empty;

    /// <summary>
    /// Type of notification (Info, Warning, Error, Success)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Type { get; set; } = "Info";

    /// <summary>
    /// Category of notification (Transaction, Trip, Policy, Validation, etc.)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Notification title/subject
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed message content
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Link/URL to related item (e.g., /trips/123, /transactions/ABC)
    /// </summary>
    [StringLength(500)]
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Label for action button (e.g., "View Trip", "Review Transaction")
    /// </summary>
    [StringLength(100)]
    public string? ActionLabel { get; set; }

    /// <summary>
    /// Related entity ID (TransactionId, TripId, etc.)
    /// </summary>
    [StringLength(100)]
    public string? RelatedEntityId { get; set; }

    /// <summary>
    /// Related entity type (Transaction, Trip, PolicyViolation, etc.)
    /// </summary>
    [StringLength(50)]
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Priority level (Low, Normal, High, Urgent)
    /// </summary>
    [StringLength(20)]
    public string Priority { get; set; } = "Normal";

    /// <summary>
    /// Whether notification has been read
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// When notification was read
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Whether notification was sent via email
    /// </summary>
    public bool EmailSent { get; set; } = false;

    /// <summary>
    /// When email was sent
    /// </summary>
    public DateTime? EmailSentAt { get; set; }

    /// <summary>
    /// Notification expiration date (optional, for cleanup)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// When notification was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Icon/emoji to display with notification
    /// </summary>
    [StringLength(10)]
    public string? Icon { get; set; }
}

/// <summary>
/// Notification types enum
/// </summary>
public static class NotificationType
{
    public const string Info = "Info";
    public const string Success = "Success";
    public const string Warning = "Warning";
    public const string Error = "Error";
}

/// <summary>
/// Notification categories enum
/// </summary>
public static class NotificationCategory
{
    public const string Transaction = "Transaction";
    public const string Trip = "Trip";
    public const string PolicyViolation = "PolicyViolation";
    public const string Validation = "Validation";
    public const string Approval = "Approval";
    public const string System = "System";
    public const string TaxCompliance = "TaxCompliance";
    public const string Documentation = "Documentation";
}

/// <summary>
/// Notification priority levels
/// </summary>
public static class NotificationPriority
{
    public const string Low = "Low";
    public const string Normal = "Normal";
    public const string High = "High";
    public const string Urgent = "Urgent";
}
