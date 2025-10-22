using TravelOperation.Core.Models.Entities;

namespace TrevelOperation.Service;

public interface ITaxCalculationService
{
    Task<TaxExposureResult> CalculateTaxExposureAsync(int tripId);
    Task<TaxExposureResult> CalculateTaxExposureAsync(Trip trip, List<Transaction> transactions);
    Task<List<TaxExposureResult>> CalculateTaxExposureForTripsAsync(List<int> tripIds);
    Task<TaxBreakdown> GetTaxBreakdownAsync(int tripId);
}

public class TaxExposureResult
{
    public int TripId { get; set; }
    public string TripName { get; set; } = string.Empty;
    public decimal MealsExposure { get; set; }
    public decimal LodgingExposure { get; set; }
    public decimal TotalTaxExposure { get; set; }
    public bool HasPremiumAirfare { get; set; }
    public List<string> PremiumCabinClasses { get; set; } = new();
    public Tax? AppliedTaxSettings { get; set; }
    public string? TaxNote { get; set; }
}

public class TaxBreakdown
{
    public TaxExposureResult Summary { get; set; } = new();
    public MealsCalculation MealsCalculation { get; set; } = new();
    public LodgingCalculation LodgingCalculation { get; set; } = new();
    public AirfareAnalysis AirfareAnalysis { get; set; } = new();
}

public class MealsCalculation
{
    public decimal TotalMealsSpent { get; set; }
    public decimal MealsPerDay { get; set; }
    public decimal MealsCap { get; set; }
    public int TripDuration { get; set; }
    public decimal ExposurePerDay { get; set; }
    public decimal TotalExposure { get; set; }
}

public class LodgingCalculation
{
    public decimal TotalLodgingSpent { get; set; }
    public decimal LodgingPerNight { get; set; }
    public decimal LodgingCap { get; set; }
    public int TripNights { get; set; }
    public decimal ExposurePerNight { get; set; }
    public decimal TotalExposure { get; set; }
}

public class AirfareAnalysis
{
    public decimal TotalAirfareSpent { get; set; }
    public List<AirfareTransaction> AirfareTransactions { get; set; } = new();
    public bool HasPremiumCabins { get; set; }
    public List<string> PremiumCabinClasses { get; set; } = new();
}

public class AirfareTransaction
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? CabinClass { get; set; }
    public bool IsPremium { get; set; }
    public string Vendor { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}