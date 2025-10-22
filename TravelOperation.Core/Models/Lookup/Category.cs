using System.ComponentModel.DataAnnotations;
using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Models.Lookup;

public class Category
{
    [Key]
    public int CategoryId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Emoji { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}