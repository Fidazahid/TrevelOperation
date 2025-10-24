using FluentAssertions;
using Moq;
using TravelOperation.Core.Interfaces;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Models.Lookup;
using TravelOperation.Core.Services;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.IntegrationTests.Services;

/// <summary>
/// Integration tests for SplitService using SQLite to test database transactions
/// These tests replace the 5 unit tests that failed with InMemory database
/// </summary>
public class SplitServiceIntegrationTests : TestBase
{
    private readonly Mock<IAuditService> _mockAuditService;
    private readonly SplitService _service;

    public SplitServiceIntegrationTests()
    {
        _mockAuditService = new Mock<IAuditService>();
        _service = new SplitService(Context, _mockAuditService.Object);
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Only seed if data doesn't already exist
        if (!Context.Categories.Any(c => c.CategoryId == 1))
        {
            var category = new Category { CategoryId = 1, Name = "Meals", Emoji = "🍽" };
            Context.Categories.Add(category);
        }

        if (!Context.Sources.Any(s => s.SourceId == 1))
        {
            var source = new Source { SourceId = 1, Name = "Navan", Emoji = "🔹" };
            Context.Sources.Add(source);
        }

        Context.SaveChanges();
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
        Context.Transactions.Add(originalTransaction);
        await Context.SaveChangesAsync();

        var splitItems = new List<SplitItem>
        {
            new() { Email = "user1@test.com", Name = "User 1", Amount = 40m, AmountUSD = 40m, CategoryId = 1 },
            new() { Email = "user2@test.com", Name = "User 2", Amount = 40m, AmountUSD = 40m, CategoryId = 1 },
            new() { Email = "user3@test.com", Name = "User 3", Amount = 40m, AmountUSD = 40m, CategoryId = 1 }
        };

        // Act
        var result = await _service.ApplySplitAsync("TXN001", splitItems, "test@example.com");

        // Assert
        result.Should().BeTrue();
        
        var splitTransactions = Context.Transactions
            .Where(t => t.OriginalTransactionId == "TXN001")
            .ToList();
        
        splitTransactions.Should().HaveCount(3);
        splitTransactions.All(t => t.AmountUSD == 40m).Should().BeTrue();
        
        var original = Context.Transactions.Find("TXN001");
        original.IsSplit.Should().BeTrue();
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
        Context.Transactions.Add(originalTransaction);
        await Context.SaveChangesAsync();

        var splitItems = new List<SplitItem>
        {
            new() { Email = "user1@test.com", Name = "User 1", Amount = 100m, AmountUSD = 100m, CategoryId = 1 },
            new() { Email = "user2@test.com", Name = "User 2", Amount = 50m, AmountUSD = 50m, CategoryId = 1 }
        };

        // Act
        var result = await _service.ApplySplitAsync("TXN002", splitItems, "test@example.com");

        // Assert
        result.Should().BeTrue();
        
        var splitTransactions = Context.Transactions
            .Where(t => t.OriginalTransactionId == "TXN002")
            .ToList()
            .OrderBy(t => t.AmountUSD)
            .ToList();
        
        splitTransactions.Should().HaveCount(2);
        splitTransactions[0].AmountUSD.Should().Be(50m);
        splitTransactions[1].AmountUSD.Should().Be(100m);
    }

    [Fact]
    public async Task ApplySplitAsync_PreservesOriginalTransactionProperties()
    {
        // Arrange
        var originalTransaction = new Transaction
        {
            TransactionId = "TXN007",
            Email = "test@example.com",
            TransactionDate = new DateTime(2025, 1, 15),
            Amount = 300m,
            AmountUSD = 300m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Vendor = "Test Vendor",
            Address = "123 Test St",
            Participants = "user1@test.com,user2@test.com"
        };
        Context.Transactions.Add(originalTransaction);
        await Context.SaveChangesAsync();

        var splitItems = new List<SplitItem>
        {
            new() { Email = "user1@test.com", Name = "User 1", Amount = 150m, AmountUSD = 150m, CategoryId = 1 },
            new() { Email = "user2@test.com", Name = "User 2", Amount = 150m, AmountUSD = 150m, CategoryId = 1 }
        };

        // Act
        var result = await _service.ApplySplitAsync("TXN007", splitItems, "test@example.com");

        // Assert
        result.Should().BeTrue();
        
        var splitTransactions = Context.Transactions
            .Where(t => t.OriginalTransactionId == "TXN007")
            .ToList();
        
        splitTransactions.Should().HaveCount(2);
        splitTransactions.All(t => t.Vendor == "Test Vendor").Should().BeTrue();
        splitTransactions.All(t => t.Address == "123 Test St").Should().BeTrue();
        splitTransactions.All(t => t.TransactionDate == new DateTime(2025, 1, 15)).Should().BeTrue();
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
            Amount = 200m,
            AmountUSD = 200m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1,
            Participants = "user1@test.com,user2@test.com"
        };
        Context.Transactions.Add(originalTransaction);
        await Context.SaveChangesAsync();

        var splitItems = new List<SplitItem>
        {
            new() { Email = "user1@test.com", Name = "User 1", Amount = 100m, AmountUSD = 100m, CategoryId = 1 },
            new() { Email = "user2@test.com", Name = "User 2", Amount = 100m, AmountUSD = 100m, CategoryId = 1 }
        };

        // Act
        await _service.ApplySplitAsync("TXN006", splitItems, "test@example.com");

        // Assert
        var original = Context.Transactions.Find("TXN006");
        original.IsSplit.Should().BeTrue();
        original.ModifiedBy.Should().Be("test@example.com");
        original.ModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ApplySplitAsync_NonExistentTransaction_ReturnsFalse()
    {
        // Arrange
        var splitItems = new List<SplitItem>
        {
            new() { Email = "user1@test.com", Name = "User 1", Amount = 50m, AmountUSD = 50m, CategoryId = 1 }
        };

        // Act
        var result = await _service.ApplySplitAsync("NONEXISTENT", splitItems, "test@example.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ApplySplitAsync_RollsBackOnError()
    {
        // Arrange
        var originalTransaction = new Transaction
        {
            TransactionId = "TXN999",
            Email = "test@example.com",
            TransactionDate = DateTime.Now,
            Amount = 100m,
            AmountUSD = 100m,
            Currency = "USD",
            CategoryId = 1,
            SourceId = 1
        };
        Context.Transactions.Add(originalTransaction);
        await Context.SaveChangesAsync();

        // Mismatched amounts - should fail validation
        var splitItems = new List<SplitItem>
        {
            new() { Email = "user1@test.com", Name = "User 1", Amount = 50m, AmountUSD = 50m, CategoryId = 1 },
            new() { Email = "user2@test.com", Name = "User 2", Amount = 40m, AmountUSD = 40m, CategoryId = 1 }
        };

        // Act
        var result = await _service.ApplySplitAsync("TXN999", splitItems, "test@example.com");

        // Assert
        result.Should().BeFalse();
        
        // Original transaction should remain unchanged
        var original = Context.Transactions.Find("TXN999");
        original.IsSplit.Should().BeFalse();
        
        // No split transactions should be created
        var splitTransactions = Context.Transactions
            .Where(t => t.OriginalTransactionId == "TXN999")
            .ToList();
        splitTransactions.Should().BeEmpty();
    }
}
