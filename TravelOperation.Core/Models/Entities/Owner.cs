using System.ComponentModel.DataAnnotations;
using TravelOperation.Core.Models.Lookup;

namespace TravelOperation.Core.Models.Entities;

public class Owner
{
    [Key]
    public int OwnerId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? CostCenter { get; set; }
    
    [MaxLength(100)]
    public string? Department { get; set; }
    
    [MaxLength(100)]
    public string? Domain { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}