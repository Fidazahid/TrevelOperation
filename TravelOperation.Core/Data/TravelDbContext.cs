using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Models.Lookup;

namespace TravelOperation.Core.Data;

public class TravelDbContext : DbContext
{
    public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // Suppress the pending model changes warning for seed data
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<Source> Sources { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Purpose> Purposes { get; set; }
    public DbSet<CabinClass> CabinClasses { get; set; }
    public DbSet<TripType> TripTypes { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<ValidationStatus> ValidationStatuses { get; set; }
    public DbSet<BookingType> BookingTypes { get; set; }
    public DbSet<BookingStatus> BookingStatuses { get; set; }
    
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Headcount> Headcount { get; set; }
    public DbSet<Tax> TaxRules { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<CountryCity> CountriesCities { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<TransformationRule> TransformationRules { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    // Approval Workflow entities
    public DbSet<Employee> Employees { get; set; }
    public DbSet<TripRequest> TripRequests { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<Approval> Approvals { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.TripId);
            
            entity.HasOne(d => d.Source)
                .WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SourceId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Category)
                .WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Trip)
                .WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(d => d.BookingStatus)
                .WithMany()
                .HasForeignKey(d => d.BookingStatusId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(d => d.BookingType)
                .WithMany()
                .HasForeignKey(d => d.BookingTypeId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(d => d.CabinClass)
                .WithMany()
                .HasForeignKey(d => d.CabinClassId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.TripId);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
            entity.HasIndex(e => e.StatusId);
            
            entity.HasOne(d => d.Purpose)
                .WithMany(p => p.Trips)
                .HasForeignKey(d => d.PurposeId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.TripType)
                .WithMany()
                .HasForeignKey(d => d.TripTypeId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Status)
                .WithMany()
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.ValidationStatus)
                .WithMany()
                .HasForeignKey(d => d.ValidationStatusId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(d => d.Owner)
                .WithMany(p => p.Trips)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId);
            
            entity.HasOne(d => d.Country)
                .WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.LinkedTable);
            entity.HasIndex(e => e.LinkedRecordId);
        });

        // Approval Workflow entity configurations
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Department);
            entity.HasIndex(e => e.Role);
        });

        modelBuilder.Entity<TripRequest>(entity =>
        {
            entity.HasKey(e => e.TripRequestId);
            entity.HasIndex(e => e.EmployeeId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartDate);
            
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.TripRequests)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId);
            entity.HasIndex(e => e.TripRequestId);
            entity.HasIndex(e => e.EmployeeId);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.ExpenseDate);
            
            entity.HasOne(d => d.TripRequest)
                .WithMany(p => p.Expenses)
                .HasForeignKey(d => d.TripRequestId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Expenses)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyId);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<Approval>(entity =>
        {
            entity.HasKey(e => e.ApprovalId);
            entity.HasIndex(e => e.TripRequestId);
            entity.HasIndex(e => e.ApprovedByEmployeeId);
            entity.HasIndex(e => e.Status);
            
            entity.HasOne(d => d.TripRequest)
                .WithMany(p => p.Approvals)
                .HasForeignKey(d => d.TripRequestId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(d => d.ApprovedBy)
                .WithMany(p => p.ApprovalsGiven)
                .HasForeignKey(d => d.ApprovedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.EmployeeId).IsUnique();
            
            entity.HasOne(d => d.Employee)
                .WithOne()
                .HasForeignKey<User>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        modelBuilder.Entity<Source>().HasData(
            new Source { SourceId = 1, Name = "Navan", Emoji = "🧳", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Source { SourceId = 2, Name = "Agent", Emoji = "👤", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Source { SourceId = 3, Name = "Manual", Emoji = "✏️", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Airfare", Emoji = "✈️", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Category { CategoryId = 2, Name = "Lodging", Emoji = "🏨", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Category { CategoryId = 3, Name = "Transportation", Emoji = "🚕", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Category { CategoryId = 4, Name = "Communication", Emoji = "📱", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Category { CategoryId = 5, Name = "Client entertainment", Emoji = "🍸", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Category { CategoryId = 6, Name = "Meals", Emoji = "🍽️", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Category { CategoryId = 7, Name = "Other", Emoji = "❔", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Category { CategoryId = 8, Name = "Non-travel", Emoji = "❓", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<Purpose>().HasData(
            new Purpose { PurposeId = 1, Name = "Business trip", Emoji = "💼", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Purpose { PurposeId = 2, Name = "Onboarding", Emoji = "🎓", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Purpose { PurposeId = 3, Name = "Company trip", Emoji = "🏖️", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Purpose { PurposeId = 4, Name = "BCP", Emoji = "🛡️", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<CabinClass>().HasData(
            new CabinClass { CabinClassId = 1, Name = "Economy", Emoji = "💺", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CabinClass { CabinClassId = 2, Name = "Premium economy", Emoji = "🛫", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CabinClass { CabinClassId = 3, Name = "Business", Emoji = "🧳", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CabinClass { CabinClassId = 4, Name = "First", Emoji = "👑", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<TripType>().HasData(
            new TripType { TripTypeId = 1, Name = "Domestic", Emoji = "🏠", CreatedAt = seedDate, ModifiedAt = seedDate },
            new TripType { TripTypeId = 2, Name = "International", Emoji = "🌍", CreatedAt = seedDate, ModifiedAt = seedDate },
            new TripType { TripTypeId = 3, Name = "Local", Emoji = "📍", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<Status>().HasData(
            new Status { StatusId = 1, Name = "Canceled", Emoji = "🔴", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Status { StatusId = 2, Name = "Upcoming", Emoji = "⚪", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Status { StatusId = 3, Name = "Ongoing", Emoji = "🔵", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Status { StatusId = 4, Name = "Completed", Emoji = "🟢", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<ValidationStatus>().HasData(
            new ValidationStatus { ValidationStatusId = 1, Name = "Not ready to validate", Emoji = "⚪", CreatedAt = seedDate, ModifiedAt = seedDate },
            new ValidationStatus { ValidationStatusId = 2, Name = "Ready to validate", Emoji = "🟡", CreatedAt = seedDate, ModifiedAt = seedDate },
            new ValidationStatus { ValidationStatusId = 3, Name = "Validated", Emoji = "🟢", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<BookingType>().HasData(
            new BookingType { BookingTypeId = 1, Name = "Flight", Emoji = "✈️", CreatedAt = seedDate, ModifiedAt = seedDate },
            new BookingType { BookingTypeId = 2, Name = "Hotel", Emoji = "🏨", CreatedAt = seedDate, ModifiedAt = seedDate },
            new BookingType { BookingTypeId = 3, Name = "Car", Emoji = "🚗", CreatedAt = seedDate, ModifiedAt = seedDate },
            new BookingType { BookingTypeId = 4, Name = "Train", Emoji = "🚆", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<BookingStatus>().HasData(
            new BookingStatus { BookingStatusId = 1, Name = "Canceled", Emoji = "🔴", CreatedAt = seedDate, ModifiedAt = seedDate },
            new BookingStatus { BookingStatusId = 2, Name = "Approved", Emoji = "🟢", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<Owner>().HasData(
            new Owner { OwnerId = 1, Name = "Maayan Chesler", Email = "maayan@company.com", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Owner { OwnerId = 2, Name = "Martina Poplinsk", Email = "martina@company.com", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<CountryCity>(entity =>
        {
            entity.HasKey(e => e.CountryCityId);
            entity.HasIndex(e => new { e.Country, e.City }).IsUnique();
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasKey(e => e.SystemSettingId);
            entity.HasIndex(e => e.Key).IsUnique();
        });

        modelBuilder.Entity<TransformationRule>(entity =>
        {
            entity.HasKey(e => e.TransformationRuleId);
            entity.HasIndex(e => e.Priority).IsDescending();
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<CountryCity>().HasData(
            new CountryCity { CountryCityId = 1, Country = "Israel", City = "Tel Aviv", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CountryCity { CountryCityId = 2, Country = "Israel", City = "Jerusalem", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CountryCity { CountryCityId = 3, Country = "United States", City = "New York", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CountryCity { CountryCityId = 4, Country = "United States", City = "San Francisco", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CountryCity { CountryCityId = 5, Country = "United Kingdom", City = "London", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CountryCity { CountryCityId = 6, Country = "Germany", City = "Berlin", CreatedAt = seedDate, ModifiedAt = seedDate },
            new CountryCity { CountryCityId = 7, Country = "France", City = "Paris", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<SystemSetting>().HasData(
            new SystemSetting { SystemSettingId = 1, Key = "DefaultCurrency", Value = "USD", Description = "Default currency for calculations", CreatedAt = seedDate, ModifiedAt = seedDate },
            new SystemSetting { SystemSettingId = 2, Key = "DateFormat", Value = "dd/MM/yyyy", Description = "Date display format", CreatedAt = seedDate, ModifiedAt = seedDate },
            new SystemSetting { SystemSettingId = 3, Key = "HighValueMealThreshold", Value = "80", Description = "Threshold for flagging high-value meals", CreatedAt = seedDate, ModifiedAt = seedDate },
            new SystemSetting { SystemSettingId = 4, Key = "LowValueLodgingThreshold", Value = "100", Description = "Threshold for flagging low-value lodging", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        // Approval Workflow seed data
        modelBuilder.Entity<Employee>().HasData(
            new Employee { EmployeeId = 1, Name = "John Doe", Email = "john.doe@company.com", Department = "Engineering", Title = "Software Engineer", ManagerEmail = "sarah.manager@company.com", Role = "Employee", CostCenter = "CC-ENG-001", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Employee { EmployeeId = 2, Name = "Sara Manager", Email = "sara.manager@company.com", Department = "Engineering", Title = "Engineering Manager", Role = "Manager", CostCenter = "CC-ENG-001", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Employee { EmployeeId = 3, Name = "Finance Officer", Email = "finance@company.com", Department = "Finance", Title = "Finance Officer", Role = "Finance", CostCenter = "CC-FIN-001", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Employee { EmployeeId = 4, Name = "Admin User", Email = "admin@company.com", Department = "Operations", Title = "System Administrator", Role = "Admin", CostCenter = "CC-OPS-001", CreatedAt = seedDate, ModifiedAt = seedDate }
        );

        modelBuilder.Entity<Policy>().HasData(
            new Policy { PolicyId = 1, Category = "Travel", Description = "Flight bookings", MaxAmount = 1500.00m, Period = "Per Trip", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Policy { PolicyId = 2, Category = "Meals", Description = "Daily meal allowance", MaxAmount = 75.00m, Period = "Daily", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Policy { PolicyId = 3, Category = "Accommodation", Description = "Hotel accommodation", MaxAmount = 200.00m, Period = "Daily", CreatedAt = seedDate, ModifiedAt = seedDate },
            new Policy { PolicyId = 4, Category = "Miscellaneous", Description = "Other travel expenses", MaxAmount = 100.00m, Period = "Daily", CreatedAt = seedDate, ModifiedAt = seedDate }
        );
    }
}