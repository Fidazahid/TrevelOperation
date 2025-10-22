using System.ComponentModel.DataAnnotations;

namespace TravelOperation.Core.Models.Entities;

public class AuditLog
{
    [Key]
    public int AuditId { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(255)]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LinkedTable { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LinkedRecordId { get; set; } = string.Empty;
    
    public string? OldValue { get; set; }
    
    public string? NewValue { get; set; }
    
    public string? Comments { get; set; }
}