using System.ComponentModel.DataAnnotations;
using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Models.Lookup;

public class Source
{
    [Key]
    public int SourceId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Emoji { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}