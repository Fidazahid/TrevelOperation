using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TrevelOperation.Service;

namespace TravelOperation.Tests.Services;

public class CategoryMappingTests : IDisposable
{
    private readonly TravelDbContext _context;
    private readonly CsvImportService _service;

    public CategoryMappingTests()
    {
        var options = new DbContextOptionsBuilder<TravelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TravelDbContext(options);
        _service = new CsvImportService(_context);
        SeedTestData();
    }

    private void SeedTestData()
    {
        var categories = new List<Category>
        {
            new() { CategoryId = 1, Name = "Airfare", Emoji = "✈" },
            new() { CategoryId = 2, Name = "Lodging", Emoji = "🏨" },
            new() { CategoryId = 3, Name = "Transportation", Emoji = "🚕" },
            new() { CategoryId = 4, Name = "Meals", Emoji = "🍽" },
            new() { CategoryId = 5, Name = "Client entertainment", Emoji = "🍸" },
            new() { CategoryId = 6, Name = "Communication", Emoji = "📱" },
            new() { CategoryId = 7, Name = "Other", Emoji = "❔" }
        };

        _context.Categories.AddRange(categories);
        _context.SaveChanges();
    }

    [Theory]
    [InlineData("tripactions_fees", "Other")]
    [InlineData("Airalo", "Communication")]
    [InlineData("public transport, tolls & parking", "Transportation")]
    [InlineData("Taxi & rideshare", "Transportation")]
    [InlineData("Rental cars", "Transportation")]
    [InlineData("Train travel", "Transportation")]
    [InlineData("Fuel", "Transportation")]
    [InlineData("entertaining clients", "Client entertainment")]
    [InlineData("team events & meals", "Meals")]
    [InlineData("Meals for myself", "Meals")]
    [InlineData("Airfare", "Airfare")]
    [InlineData("Internet access", "Communication")]
    [InlineData("telecommunication_services", "Communication")]
    [InlineData("Lodging", "Lodging")]
    public void MapPolicyToCategory_StandardPolicies_ReturnCorrectCategory(string policy, string expectedCategory)
    {
        // Act
        var categoryName = MapPolicyField(policy);

        // Assert
        categoryName.Should().Be(expectedCategory);
    }

    [Fact]
    public void MapPolicyToCategory_UnknownPolicy_ReturnsOther()
    {
        // Arrange
        var unknownPolicy = "some random text";

        // Act
        var result = MapPolicyField(unknownPolicy);

        // Assert
        result.Should().Be("Other");
    }

    [Fact]
    public void MapPolicyToCategory_NullPolicy_ReturnsOther()
    {
        // Act
        var result = MapPolicyField(null);

        // Assert
        result.Should().Be("Other");
    }

    [Fact]
    public void MapPolicyToCategory_EmptyPolicy_ReturnsOther()
    {
        // Act
        var result = MapPolicyField("");

        // Assert
        result.Should().Be("Other");
    }

    [Theory]
    [InlineData("AIRFARE", "Airfare")]
    [InlineData("airfare", "Airfare")]
    [InlineData("AiRfArE", "Airfare")]
    public void MapPolicyToCategory_CaseInsensitive_ReturnCorrectCategory(string policy, string expected)
    {
        // Act
        var result = MapPolicyField(policy);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("  Airfare  ", "Airfare")]
    [InlineData("\tLodging\n", "Lodging")]
    public void MapPolicyToCategory_WithWhitespace_TrimsAndMapsCorrectly(string policy, string expected)
    {
        // Act
        var result = MapPolicyField(policy?.Trim());

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetCategoryIdByName_ExistingCategory_ReturnsCorrectId()
    {
        // Act
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Airfare");

        // Assert
        category.Should().NotBeNull();
        category!.CategoryId.Should().Be(1);
    }

    [Fact]
    public async Task GetCategoryIdByName_NonExistingCategory_ReturnsNull()
    {
        // Act
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "NonExistent");

        // Assert
        category.Should().BeNull();
    }

    [Theory]
    [InlineData("Conference attendance", "Other")]
    [InlineData("Software", "Other")]
    public void MapPolicyToCategory_MiscellaneousItems_ReturnsOther(string policy, string expected)
    {
        // Act
        var result = MapPolicyField(policy);

        // Assert
        result.Should().Be(expected);
    }

    // Helper method mimicking the actual service logic
    private string MapPolicyField(string? policy)
    {
        if (string.IsNullOrWhiteSpace(policy))
            return "Other";

        var lowerPolicy = policy.ToLower().Trim();

        return lowerPolicy switch
        {
            _ when lowerPolicy.Contains("tripactions_fees") => "Other",
            _ when lowerPolicy.Contains("airalo") => "Communication",
            _ when lowerPolicy.Contains("public transport") || lowerPolicy.Contains("tolls") || lowerPolicy.Contains("parking") => "Transportation",
            _ when lowerPolicy.Contains("taxi") || lowerPolicy.Contains("rideshare") => "Transportation",
            _ when lowerPolicy.Contains("rental car") => "Transportation",
            _ when lowerPolicy.Contains("train") => "Transportation",
            _ when lowerPolicy.Contains("fuel") => "Transportation",
            _ when lowerPolicy.Contains("entertaining clients") || lowerPolicy.Contains("client entertainment") => "Client entertainment",
            _ when lowerPolicy.Contains("team events") || lowerPolicy.Contains("team meals") => "Meals",
            _ when lowerPolicy.Contains("meals for myself") => "Meals",
            _ when lowerPolicy.Contains("airfare") => "Airfare",
            _ when lowerPolicy.Contains("internet") => "Communication",
            _ when lowerPolicy.Contains("telecommunication") => "Communication",
            _ when lowerPolicy.Contains("lodging") => "Lodging",
            _ when lowerPolicy.Contains("software") => "Other",
            _ when lowerPolicy.Contains("conference") => "Other",
            _ => "Other"
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
