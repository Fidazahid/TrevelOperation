using System.ComponentModel.DataAnnotations;

namespace TravelOperation.Core.Models.Entities;

public class Headcount
{
    [Key]
    public int HeadcountId { get; set; }
    
    [Required]
    public DateTime Period { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Subsidiary { get; set; }
    
    [MaxLength(100)]
    public string? Site { get; set; }
    
    [MaxLength(100)]
    public string? Department { get; set; }
    
    [MaxLength(100)]
    public string? Domain { get; set; }
    
    [MaxLength(100)]
    public string? CostCenter { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}