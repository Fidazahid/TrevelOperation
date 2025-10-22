using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelOperation.Core.Models.Entities;

public class Tax
{
    [Key]
    public int TaxId { get; set; }
    
    [Required]
    public int FiscalYear { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Subsidiary { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MealsCap { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? LodgingCap { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxShield { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}