using System.ComponentModel.DataAnnotations;
using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Models.Lookup;

public class Purpose
{
    [Key]
    public int PurposeId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Emoji { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}