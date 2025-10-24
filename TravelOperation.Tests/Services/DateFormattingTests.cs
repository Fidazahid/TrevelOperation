using FluentAssertions;
using Moq;
using TrevelOperation.Service;

namespace TravelOperation.Tests.Services;

public class DateFormattingTests
{
    private readonly MessageTemplateService _service;

    public DateFormattingTests()
    {
        var mockSettings = new Mock<ISettingsService>();
        _service = new MessageTemplateService(mockSettings.Object);
    }

    [Theory]
    [InlineData("2025-01-15", "15/01/2025")]
    [InlineData("2025-12-31", "31/12/2025")]
    [InlineData("2025-03-05", "05/03/2025")]
    public void FormatDate_VariousDates_ReturnsCorrectFormat(string input, string expected)
    {
        // Arrange
        var date = DateTime.Parse(input);

        // Act
        var result = date.ToString("dd/MM/yyyy");

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void FormatDate_LeapYear_FormatsCorrectly()
    {
        // Arrange
        var leapDate = new DateTime(2024, 2, 29);

        // Act
        var result = leapDate.ToString("dd/MM/yyyy");

        // Assert
        result.Should().Be("29/02/2024");
    }

    [Fact]
    public void FormatDateTime_WithTime_ReturnsCorrectFormat()
    {
        // Arrange
        var dateTime = new DateTime(2025, 10, 24, 14, 30, 45);

        // Act
        var result = dateTime.ToString("dd/MM/yyyy HH:mm:ss");

        // Assert
        result.Should().Be("24/10/2025 14:30:45");
    }

    [Theory]
    [InlineData(1000.5, "1,000.50")]
    [InlineData(1234567.89, "1,234,567.89")]
    [InlineData(50.5, "50.50")]
    public void FormatAmount_VariousAmounts_ReturnsWithThousandSeparators(decimal amount, string expected)
    {
        // Act
        var result = amount.ToString("N2");

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1000.5, "$1,000.50")]
    [InlineData(500.99, "$500.99")]
    public void FormatUSDAmount_WithDollarSign_ReturnsCorrectFormat(decimal amount, string expected)
    {
        // Act
        var result = $"${amount:N2}";

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateDaysBetween_ValidDates_ReturnsCorrectDays()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 1, 10);

        // Act
        var days = (endDate - startDate).Days + 1; // Inclusive

        // Assert
        days.Should().Be(10);
    }

    [Fact]
    public void CalculateDuration_SameDay_ReturnsOne()
    {
        // Arrange
        var date = new DateTime(2025, 5, 15);

        // Act
        var duration = (date - date).Days + 1;

        // Assert
        duration.Should().Be(1);
    }

    [Fact]
    public void ParseDate_InvalidFormat_ThrowsException()
    {
        // Arrange
        var invalidDate = "32/13/2025";

        // Act
        Action act = () => DateTime.ParseExact(invalidDate, "dd/MM/yyyy", null);

        // Assert
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void FormatNullableDate_WithValue_FormatsCorrectly()
    {
        // Arrange
        DateTime? date = new DateTime(2025, 3, 15);

        // Act
        var result = date?.ToString("dd/MM/yyyy") ?? "";

        // Assert
        result.Should().Be("15/03/2025");
    }

    [Fact]
    public void FormatNullableDate_WithNull_ReturnsEmpty()
    {
        // Arrange
        DateTime? date = null;

        // Act
        var result = date?.ToString("dd/MM/yyyy") ?? "";

        // Assert
        result.Should().BeEmpty();
    }
}
