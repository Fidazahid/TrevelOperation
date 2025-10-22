using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelOperation.Core.Models.Lookup;

namespace TravelOperation.Core.Models.Entities;

public class Transaction
{
    [Key]
    [MaxLength(100)]
    public string TransactionId { get; set; } = string.Empty;
    
    [ForeignKey("Source")]
    public int SourceId { get; set; }
    public virtual Source Source { get; set; } = null!;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public DateTime TransactionDate { get; set; }
    
    public DateTime? AuthorizationDate { get; set; }
    
    [MaxLength(50)]
    public string? TransactionType { get; set; }
    
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    
    [MaxLength(200)]
    public string? Vendor { get; set; }
    
    [MaxLength(100)]
    public string? MerchantCategory { get; set; }
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(100)]
    public string? SourceTripId { get; set; }
    
    [MaxLength(100)]
    public string? BookingId { get; set; }
    
    [ForeignKey("BookingStatus")]
    public int? BookingStatusId { get; set; }
    public virtual BookingStatus? BookingStatus { get; set; }
    
    public DateTime? BookingStartDate { get; set; }
    
    public DateTime? BookingEndDate { get; set; }
    
    [ForeignKey("BookingType")]
    public int? BookingTypeId { get; set; }
    public virtual BookingType? BookingType { get; set; }
    
    [MaxLength(500)]
    public string? Policy { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? AmountUSD { get; set; }
    
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ExchangeRate { get; set; }
    
    public string? Participants { get; set; }
    
    [MaxLength(1000)]
    public string? DocumentUrl { get; set; }
    
    public string? Notes { get; set; }
    
    [ForeignKey("Trip")]
    public int? TripId { get; set; }
    public virtual Trip? Trip { get; set; }
    
    public bool DataValidation { get; set; } = false;
    
    public bool ParticipantsValidated { get; set; } = false;
    
    public bool IsValid { get; set; } = false;
    
    [ForeignKey("CabinClass")]
    public int? CabinClassId { get; set; }
    public virtual CabinClass? CabinClass { get; set; }
    
    // Split transaction properties
    public bool IsSplit { get; set; } = false;
    
    [MaxLength(100)]
    public string? OriginalTransactionId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
}