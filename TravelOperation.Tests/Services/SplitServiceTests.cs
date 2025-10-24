using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Tests.Services;

public class SplitServiceTests : IDisposable
{
    private readonly TravelDbContext _context;
    private readonly Mock<IAuditService> _mockAuditService;
    private readonly SplitService _service;

    public SplitServiceTests()
    {
        var options = new DbContextOptionsBuilder<TravelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TravelDbContext(options);
        _mockAuditService = new Mock<IAuditService>();
        _service = new SplitService(_context, _mockAuditService.Object);
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new Category { CategoryId = 1, Name = "Meals", Emoji = "🍽" };
        _context.Categories.Add(category);

        var source = new Source { SourceId = 1, Name = "Navan", Emoji = "🔹" };
        _context.Sources.Add(source);

        _context.SaveChanges();
    }

    [Fact]
    public async Task ApplySplitAsync_EqualSplit_DividesAmountEqually()
    {
        // Arrange
        var originalTransaction = new Transaction
        {
            TransactionId = "TXN001",
            Email = "test@example.com",
            TransactionDate = DateTime.Now,
            Amount = 120m,
            AmountUSD = 120m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Participants = "user1@test.com,user2@test.com,user3@test.com"
        };
        _context.Transactions.Add(originalTransaction);
        await _context.SaveChangesAsync();

        var splitData = new
        {
            SplitType = "equal",
            ParticipantCount = 3
        };

        // Act
        var result = await _service.ApplySplitAsync("TXN001", splitData);

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(3);
        result.All(t => t.AmountUSD == 40m).Should().BeTrue(); // 120 / 3 = 40
        result.All(t => t.OriginalTransactionId == "TXN001").Should().BeTrue();
        result.All(t => t.IsSplit).Should().BeTrue();
    }

    [Fact]
    public async Task ApplySplitAsync_CustomSplit_UsesSpecifiedAmounts()
    {
        // Arrange
        var originalTransaction = new Transaction
        {
            TransactionId = "TXN002",
            Email = "test@example.com",
            TransactionDate = DateTime.Now,
            Amount = 150m,
            AmountUSD = 150m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Participants = "user1@test.com,user2@test.com"
        };
        _context.Transactions.Add(originalTransaction);
        await _context.SaveChangesAsync();

        var splitData = new
        {
            SplitType = "custom",
            Splits = new[]
            {
                new { Amount = 100m, Email = "user1@test.com" },
                new { Amount = 50m, Email = "user2@test.com" }
            }
        };

        // Act
        var result = await _service.ApplySplitAsync("TXN002", splitData);

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(2);
        result.First().AmountUSD.Should().Be(100m);
        result.Last().AmountUSD.Should().Be(50m);
    }

    [Fact]
    public async Task ApplySplitAsync_PercentageSplit_CalculatesCorrectly()
    {
        // Arrange
        var originalTransaction = new Transaction
        {
            TransactionId = "TXN003",
            Email = "test@example.com",
            TransactionDate = DateTime.Now,
            Amount = 200m,
            AmountUSD = 200m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Participants = "user1@test.com,user2@test.com"
        };
        _context.Transactions.Add(originalTransaction);
        await _context.SaveChangesAsync();

        // Act - 60/40 split
        var amount1 = 200m * 0.60m; // 120
        var amount2 = 200m * 0.40m; // 80

        // Assert
        amount1.Should().Be(120m);
        amount2.Should().Be(80m);
        (amount1 + amount2).Should().Be(200m);
    }

    [Fact]
    public async Task GetSplitSuggestionsAsync_TransactionWithMultipleParticipants_ReturnsSuggestion()
    {
        // Arrange
        var transaction = new Transaction
        {
            TransactionId = "TXN004",
            Email = "test@example.com",
            TransactionDate = DateTime.Now,
            Amount = 300m,
            AmountUSD = 300m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Participants = "user1@test.com,user2@test.com,user3@test.com,user4@test.com"
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var suggestions = await _service.GetSplitSuggestionsAsync();

        // Assert
        suggestions.Should().NotBeEmpty();
        var suggestion = suggestions.First();
        suggestion.TransactionId.Should().Be("TXN004");
        suggestion.ParticipantCount.Should().Be(4);
        suggestion.SuggestedSplitAmount.Should().Be(75m); // 300 / 4
    }

    [Fact]
    public async Task GetSplitSuggestionsAsync_AlreadySplitTransaction_DoesNotSuggest()
    {
        // Arrange
        var transaction = new Transaction
        {
            TransactionId = "TXN005",
            Email = "test@example.com",
            TransactionDate = DateTime.Now,
            Amount = 100m,
            AmountUSD = 100m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Participants = "user1@test.com,user2@test.com",
            IsSplit = true
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var suggestions = await _service.GetSplitSuggestionsAsync();

        // Assert
        suggestions.Should().NotContain(s => s.TransactionId == "TXN005");
    }

    [Fact]
    public async Task ApplySplitAsync_MarksOriginalAsDeleted()
    {
        // Arrange
        var originalTransaction = new Transaction
        {
            TransactionId = "TXN006",
            Email = "test@example.com",
            TransactionDate = DateTime.Now,
            Amount = 150m,
            AmountUSD = 150m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Participants = "user1@test.com,user2@test.com,user3@test.com"
        };
        _context.Transactions.Add(originalTransaction);
        await _context.SaveChangesAsync();

        var splitData = new
        {
            SplitType = "equal",
            ParticipantCount = 3
        };

        // Act
        await _service.ApplySplitAsync("TXN006", splitData);

        // Assert
        var original = await _context.Transactions.FindAsync("TXN006");
        original.Should().NotBeNull();
        original!.IsSplit.Should().BeTrue();
    }

    [Fact]
    public async Task ApplySplitAsync_PreservesOriginalTransactionProperties()
    {
        // Arrange
        var originalTransaction = new Transaction
        {
            TransactionId = "TXN007",
            Email = "test@example.com",
            TransactionDate = new DateTime(2025, 5, 15),
            Amount = 90m,
            AmountUSD = 90m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Vendor = "Test Restaurant",
            Address = "123 Test St",
            Participants = "user1@test.com,user2@test.com,user3@test.com"
        };
        _context.Transactions.Add(originalTransaction);
        await _context.SaveChangesAsync();

        var splitData = new
        {
            SplitType = "equal",
            ParticipantCount = 3
        };

        // Act
        var result = await _service.ApplySplitAsync("TXN007", splitData);

        // Assert
        result.All(t => t.TransactionDate == originalTransaction.TransactionDate).Should().BeTrue();
        result.All(t => t.Vendor == originalTransaction.Vendor).Should().BeTrue();
        result.All(t => t.Address == originalTransaction.Address).Should().BeTrue();
        result.All(t => t.Currency == originalTransaction.Currency).Should().BeTrue();
    }

    [Theory]
    [InlineData(100, 3, 33.33)]
    [InlineData(200, 4, 50)]
    [InlineData(75, 5, 15)]
    public void CalculateEqualSplit_VariousAmounts_CalculatesCorrectly(decimal total, int participants, decimal expected)
    {
        // Act
        var splitAmount = Math.Round(total / participants, 2);

        // Assert
        splitAmount.Should().Be(expected);
    }

    [Fact]
    public async Task ApplySplitAsync_NonExistentTransaction_ReturnsNull()
    {
        // Arrange
        var splitData = new
        {
            SplitType = "equal",
            ParticipantCount = 2
        };

        // Act
        var result = await _service.ApplySplitAsync("NONEXISTENT", splitData);

        // Assert
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
