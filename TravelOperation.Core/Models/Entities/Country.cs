using System.ComponentModel.DataAnnotations;

namespace TravelOperation.Core.Models.Entities;

public class Country
{
    [Key]
    public int CountryId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(3)]
    public string? Code { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}