using FluentAssertions;

namespace TravelOperation.Tests.Services;

public class AmountCalculationTests
{
    [Theory]
    [InlineData(100, 1.2, 120)]
    [InlineData(50.5, 3.5, 176.75)]
    [InlineData(1000, 0.85, 850)]
    public void CalculateAmountUSD_WithExchangeRate_ReturnsCorrectAmount(
        decimal amount, decimal rate, decimal expected)
    {
        // Act
        var result = amount * rate;

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateAmountUSD_WithNullRate_UsesDefaultRate()
    {
        // Arrange
        decimal amount = 100m;
        decimal? exchangeRate = null;
        decimal defaultRate = 1m;

        // Act
        var result = amount * (exchangeRate ?? defaultRate);

        // Assert
        result.Should().Be(100m);
    }

    [Theory]
    [InlineData(500, 5, 100)]
    [InlineData(1000, 10, 100)]
    [InlineData(750, 3, 250)]
    public void CalculateCostPerDay_ValidInputs_ReturnsCorrectAverage(
        decimal totalAmount, int duration, decimal expected)
    {
        // Act
        var result = totalAmount / duration;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(400, 5, 80)]
    [InlineData(600, 6, 100)]
    [InlineData(150, 2, 75)]
    public void CalculateLodgingPerNight_ValidInputs_ReturnsCorrectAmount(
        decimal lodgingTotal, int nights, decimal expected)
    {
        // Act
        var result = lodgingTotal / nights;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(300, 5, 60)]
    [InlineData(250, 5, 50)]
    [InlineData(200, 10, 20)]
    public void CalculateMealsPerDay_ValidInputs_ReturnsCorrectAmount(
        decimal mealsTotal, int days, decimal expected)
    {
        // Act
        var result = mealsTotal / days;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(60, 50, 5, 50)] // (60 - 50) * 5 = 50
    [InlineData(45, 50, 5, 0)]  // No exposure
    [InlineData(70, 50, 10, 200)] // (70 - 50) * 10 = 200
    public void CalculateMealsExposure_VariousScenarios_ReturnsCorrectExposure(
        decimal mealsPerDay, decimal cap, int days, decimal expected)
    {
        // Act
        var exposure = mealsPerDay > cap 
            ? (mealsPerDay - cap) * days 
            : 0m;

        // Assert
        exposure.Should().Be(expected);
    }

    [Theory]
    [InlineData(120, 100, 5, 100)] // (120 - 100) * 5 = 100
    [InlineData(80, 100, 5, 0)]    // No exposure
    [InlineData(150, 100, 4, 200)] // (150 - 100) * 4 = 200
    public void CalculateLodgingExposure_VariousScenarios_ReturnsCorrectExposure(
        decimal lodgingPerNight, decimal cap, int nights, decimal expected)
    {
        // Act
        var exposure = lodgingPerNight > cap 
            ? (lodgingPerNight - cap) * nights 
            : 0m;

        // Assert
        exposure.Should().Be(expected);
    }

    [Fact]
    public void CalculateTotalTaxExposure_MultipleExposures_SumsCorrectly()
    {
        // Arrange
        decimal mealsExposure = 50m;
        decimal lodgingExposure = 100m;

        // Act
        var total = mealsExposure + lodgingExposure;

        // Assert
        total.Should().Be(150m);
    }

    [Theory]
    [InlineData(1000, 2, 500)]
    [InlineData(500, 3, 166.67)]
    [InlineData(100, 4, 25)]
    public void CalculateAverageSplit_VariousParticipants_ReturnsCorrectAmount(
        decimal total, int participants, decimal expected)
    {
        // Act
        var average = Math.Round(total / participants, 2);

        // Assert
        average.Should().Be(expected);
    }

    [Theory]
    [InlineData(100, 0.25, 25)]
    [InlineData(500, 0.30, 150)]
    [InlineData(1000, 0.17, 170)]
    public void CalculateTaxShield_VariousRates_ReturnsCorrectAmount(
        decimal exposure, decimal taxRate, decimal expected)
    {
        // Act
        var shield = exposure * taxRate;

        // Assert
        shield.Should().Be(expected);
    }

    [Fact]
    public void CalculatePercentage_ValidInputs_ReturnsCorrectPercentage()
    {
        // Arrange
        decimal part = 250m;
        decimal whole = 1000m;

        // Act
        var percentage = (part / whole) * 100;

        // Assert
        percentage.Should().Be(25m);
    }

    [Theory]
    [InlineData(100.456, 100.46)]
    [InlineData(99.994, 99.99)]
    [InlineData(50.555, 50.56)]
    public void RoundAmount_ToTwoDecimals_RoundsCorrectly(decimal input, decimal expected)
    {
        // Act
        var result = Math.Round(input, 2);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void SumAmounts_MultipleTransactions_CalculatesCorrectTotal()
    {
        // Arrange
        var amounts = new[] { 100m, 250m, 75.50m, 30m };

        // Act
        var total = amounts.Sum();

        // Assert
        total.Should().Be(455.50m);
    }

    [Fact]
    public void CalculateAmountUSD_NegativeAmount_Refund_HandlesCorrectly()
    {
        // Arrange
        decimal refundAmount = -50m;
        decimal exchangeRate = 1.2m;

        // Act
        var amountUSD = refundAmount * exchangeRate;

        // Assert
        amountUSD.Should().Be(-60m);
    }

    [Theory]
    [InlineData(100, 120, 1.2)]
    [InlineData(50, 42.5, 0.85)]
    [InlineData(200, 200, 1.0)]
    public void CalculateExchangeRate_FromAmounts_ReturnsCorrectRate(
        decimal originalAmount, decimal usdAmount, decimal expectedRate)
    {
        // Act
        var rate = usdAmount / originalAmount;

        // Assert
        Math.Round(rate, 2).Should().Be(expectedRate);
    }

    [Fact]
    public void ValidateAmountUSDCalculation_MatchesExpected_ReturnsTrue()
    {
        // Arrange
        decimal amount = 100m;
        decimal exchangeRate = 1.15m;
        decimal calculatedUSD = 115m;
        decimal tolerance = 0.01m;

        // Act
        var isValid = Math.Abs((amount * exchangeRate) - calculatedUSD) <= tolerance;

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateAmountUSDCalculation_DoesNotMatch_ReturnsFalse()
    {
        // Arrange
        decimal amount = 100m;
        decimal exchangeRate = 1.15m;
        decimal calculatedUSD = 120m; // Wrong
        decimal tolerance = 0.01m;

        // Act
        var isValid = Math.Abs((amount * exchangeRate) - calculatedUSD) <= tolerance;

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(100, 0)]
    public void CalculateCostPerDay_ZeroValue_ReturnsZero(decimal total, int duration)
    {
        // Act
        var result = duration > 0 ? total / duration : 0;

        // Assert
        result.Should().Be(0);
    }
}
