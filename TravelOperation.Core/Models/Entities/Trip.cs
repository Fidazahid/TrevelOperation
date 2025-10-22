using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelOperation.Core.Models.Lookup;

namespace TravelOperation.Core.Models.Entities;

public class Trip
{
    [Key]
    public int TripId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string TripName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    public int Duration { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Country1 { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string City1 { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Country2 { get; set; }
    
    [MaxLength(100)]
    public string? City2 { get; set; }
    
    [ForeignKey("Purpose")]
    public int PurposeId { get; set; }
    public virtual Purpose Purpose { get; set; } = null!;
    
    [ForeignKey("TripType")]
    public int TripTypeId { get; set; }
    public virtual TripType TripType { get; set; } = null!;
    
    [ForeignKey("Status")]
    public int StatusId { get; set; }
    public virtual Status Status { get; set; } = null!;
    
    [ForeignKey("ValidationStatus")]
    public int ValidationStatusId { get; set; }
    public virtual ValidationStatus ValidationStatus { get; set; } = null!;
    
    public bool IsManual { get; set; } = false;
    
    [ForeignKey("Owner")]
    public int OwnerId { get; set; }
    public virtual Owner Owner { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}