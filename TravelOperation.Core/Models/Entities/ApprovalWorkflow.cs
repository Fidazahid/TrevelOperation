using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelOperation.Core.Models.Entities;

/// <summary>
/// Employee entity for approval workflow system
/// Represents system users with roles and department information
/// </summary>
[Table("Employees")]
public class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [StringLength(50)]
    public string Department { get; set; } = "";

    [StringLength(50)]
    public string? Title { get; set; }

    [StringLength(50)]
    public string? ManagerEmail { get; set; }

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "Employee"; // Employee, Manager, Finance, Admin

    [StringLength(20)]
    public string CostCenter { get; set; } = "";

    [Column(TypeName = "decimal(10,2)")]
    public decimal MonthlyCreditLimit { get; set; } = 5000.00m;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<TripRequest> TripRequests { get; set; } = new List<TripRequest>();
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public virtual ICollection<Approval> ApprovalsGiven { get; set; } = new List<Approval>();
}

/// <summary>
/// Trip request entity for approval workflow
/// Represents employee travel requests that need approval
/// </summary>
[Table("TripRequests")]
public class TripRequest
{
    [Key]
    public int TripRequestId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(200)]
    public string Destination { get; set; } = "";

    [Required]
    [StringLength(500)]
    public string Purpose { get; set; } = "";

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal EstimatedCost { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved, Rejected, Completed

    [StringLength(1000)]
    public string? BusinessJustification { get; set; }

    [StringLength(1000)]
    public string? Comments { get; set; }

    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
    
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();
}

/// <summary>
/// Expense entity for individual expense items within trip requests
/// </summary>
[Table("Expenses")]
public class Expense
{
    [Key]
    public int ExpenseId { get; set; }

    [Required]
    public int TripRequestId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateTime ExpenseDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = ""; // Travel, Meals, Accommodation, Miscellaneous

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = "";

    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } = "USD";

    [Column(TypeName = "decimal(10,2)")]
    public decimal AmountUSD { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; } = ""; // Credit Card, Cash, Bank Transfer

    [StringLength(200)]
    public string VendorName { get; set; } = "";

    [StringLength(500)]
    public string? ReceiptPath { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved, Rejected, Reimbursed

    [StringLength(1000)]
    public string? Comments { get; set; }

    public bool IsReimbursable { get; set; } = true;
    public DateTime? ReimbursedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("TripRequestId")]
    public virtual TripRequest TripRequest { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}

/// <summary>
/// Policy entity for expense validation rules
/// </summary>
[Table("Policies")]
public class Policy
{
    [Key]
    public int PolicyId { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = "";

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = "";

    [Column(TypeName = "decimal(10,2)")]
    public decimal MaxAmount { get; set; }

    [StringLength(20)]
    public string Period { get; set; } = "Daily"; // Daily, Weekly, Monthly, Per Trip

    [StringLength(100)]
    public string ApplicableRegion { get; set; } = "Global";

    public bool IsActive { get; set; } = true;

    [StringLength(1000)]
    public string? AdditionalRules { get; set; }

    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Approval entity for tracking approval workflow
/// </summary>
[Table("Approvals")]
public class Approval
{
    [Key]
    public int ApprovalId { get; set; }

    [Required]
    public int TripRequestId { get; set; }

    [Required]
    public int ApprovedByEmployeeId { get; set; }

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = ""; // Manager, Finance

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = ""; // Pending, Approved, Rejected

    [StringLength(1000)]
    public string? Comments { get; set; }

    public DateTime? ApprovedDate { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? ApprovedAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("TripRequestId")]
    public virtual TripRequest TripRequest { get; set; } = null!;

    [ForeignKey("ApprovedByEmployeeId")]
    public virtual Employee ApprovedBy { get; set; } = null!;
}

/// <summary>
/// User entity for authentication (optional)
/// </summary>
[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(500)]
    public string PasswordHash { get; set; } = "";

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "Employee";

    [Required]
    public int EmployeeId { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; } = null!;
}