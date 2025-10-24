using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TrevelOperation.Service;

namespace TravelOperation.Tests.Services;

public class TaxCalculationServiceTests : IDisposable
{
    private readonly TravelDbContext _context;
    private readonly TaxCalculationService _service;

    public TaxCalculationServiceTests()
    {
        var options = new DbContextOptionsBuilder<TravelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TravelDbContext(options);
        _service = new TaxCalculationService(_context);
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Seed tax rules
        var taxRule = new TaxRule
        {
            FiscalYear = 2025,
            Country = "Israel",
            Subsidiary = "WSC IL",
            MealsCap = 50.00m,
            LodgingCap = 100.00m,
            TaxShield = 0.25m
        };

        _context.TaxRules.Add(taxRule);

        // Seed categories
        var categories = new List<Category>
        {
            new() { CategoryId = 1, Name = "Meals", Emoji = "🍽" },
            new() { CategoryId = 2, Name = "Lodging", Emoji = "🏨" },
            new() { CategoryId = 3, Name = "Airfare", Emoji = "✈" }
        };

        _context.Categories.AddRange(categories);

        // Seed cabin classes
        var cabinClasses = new List<CabinClass>
        {
            new() { CabinClassId = 1, Name = "Economy", Emoji = "💺" },
            new() { CabinClassId = 2, Name = "Business", Emoji = "🧳" },
            new() { CabinClassId = 3, Name = "First", Emoji = "👑" }
        };

        _context.CabinClasses.AddRange(cabinClasses);

        // Seed a trip with transactions
        var trip = new Trip
        {
            TripId = 1,
            TripName = "Test Trip",
            Email = "test@example.com",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 1, 5),
            Duration = 5,
            Country1 = "Israel",
            City1 = "Tel Aviv"
        };

        _context.Trips.Add(trip);

        var transactions = new List<Transaction>
        {
            // High meals (exceeds cap)
            new() 
            { 
                TransactionId = "TXN001",
                Email = "test@example.com",
                TransactionDate = new DateTime(2025, 1, 1),
                CategoryId = 1,
                AmountUSD = 300m, // $60/day for 5 days
                TripId = 1
            },
            // Low lodging (within cap)
            new() 
            { 
                TransactionId = "TXN002",
                Email = "test@example.com",
                TransactionDate = new DateTime(2025, 1, 1),
                CategoryId = 2,
                AmountUSD = 400m, // $80/night for 5 nights
                TripId = 1
            },
            // Premium cabin airfare
            new() 
            { 
                TransactionId = "TXN003",
                Email = "test@example.com",
                TransactionDate = new DateTime(2025, 1, 1),
                CategoryId = 3,
                CabinClassId = 2, // Business
                AmountUSD = 2000m,
                TripId = 1
            }
        };

        _context.Transactions.AddRange(transactions);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CalculateTaxExposureAsync_WithHighMeals_ReturnsCorrectExposure()
    {
        // Act
        var result = await _service.CalculateTaxExposureAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.TripId.Should().Be(1);
        result.MealsExposure.Should().Be(50m); // (60 - 50) * 5 days = 50
        result.HasMealsIssue.Should().BeTrue();
    }

    [Fact]
    public async Task CalculateTaxExposureAsync_WithLowLodging_NoExposure()
    {
        // Act
        var result = await _service.CalculateTaxExposureAsync(1);

        // Assert
        result.LodgingExposure.Should().Be(0m); // 80 < 100 cap, no exposure
        result.HasLodgingIssue.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateTaxExposureAsync_WithPremiumCabin_FlagsAirfare()
    {
        // Act
        var result = await _service.CalculateTaxExposureAsync(1);

        // Assert
        result.HasPremiumCabinClass.Should().BeTrue();
    }

    [Fact]
    public async Task CalculateTaxExposureAsync_CalculatesTotalCorrectly()
    {
        // Act
        var result = await _service.CalculateTaxExposureAsync(1);

        // Assert
        result.TotalTaxExposure.Should().Be(50m); // Only meals exposure
    }

    [Fact]
    public async Task CalculateTaxExposureAsync_NonExistentTrip_ReturnsNull()
    {
        // Act
        var result = await _service.CalculateTaxExposureAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CalculateTaxExposureAsync_TripWithoutTransactions_ReturnsZeroExposure()
    {
        // Arrange
        var emptyTrip = new Trip
        {
            TripId = 2,
            TripName = "Empty Trip",
            Email = "test@example.com",
            StartDate = new DateTime(2025, 2, 1),
            EndDate = new DateTime(2025, 2, 3),
            Duration = 3,
            Country1 = "Israel",
            City1 = "Tel Aviv"
        };
        _context.Trips.Add(emptyTrip);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CalculateTaxExposureAsync(2);

        // Assert
        result.Should().NotBeNull();
        result.TotalTaxExposure.Should().Be(0m);
        result.HasMealsIssue.Should().BeFalse();
        result.HasLodgingIssue.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateTaxExposureAsync_ExcessiveLodging_CalculatesCorrectExposure()
    {
        // Arrange
        var trip = new Trip
        {
            TripId = 3,
            TripName = "Luxury Trip",
            Email = "test@example.com",
            StartDate = new DateTime(2025, 3, 1),
            EndDate = new DateTime(2025, 3, 4),
            Duration = 4,
            Country1 = "Israel",
            City1 = "Tel Aviv"
        };
        _context.Trips.Add(trip);

        var transaction = new Transaction
        {
            TransactionId = "TXN004",
            Email = "test@example.com",
            TransactionDate = new DateTime(2025, 3, 1),
            CategoryId = 2, // Lodging
            AmountUSD = 600m, // $150/night for 4 nights
            TripId = 3
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CalculateTaxExposureAsync(3);

        // Assert
        result.LodgingExposure.Should().Be(200m); // (150 - 100) * 4 = 200
        result.HasLodgingIssue.Should().BeTrue();
        result.TotalTaxExposure.Should().Be(200m);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
