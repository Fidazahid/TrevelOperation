namespace TravelOperation.Core.Models.Entities;

public class TransformationRule
{
    public int TransformationRuleId { get; set; }
    public string PolicyPattern { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Priority { get; set; } = 0; // Higher number = higher priority
    public bool IsRegex { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
