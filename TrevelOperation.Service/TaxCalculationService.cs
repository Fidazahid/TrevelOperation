using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;

namespace TrevelOperation.Service;

public class TaxCalculationService : ITaxCalculationService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<TaxCalculationService> _logger;

    private readonly HashSet<string> PremiumCabinClasses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Business", "First", "🧳 Business", "👑 First"
    };

    public TaxCalculationService(TravelDbContext context, ILogger<TaxCalculationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaxExposureResult> CalculateTaxExposureAsync(int tripId)
    {
        var trip = await _context.Trips
            .Include(t => t.Transactions)
                .ThenInclude(t => t.Category)
            .Include(t => t.Transactions)
                .ThenInclude(t => t.CabinClass)
            .FirstOrDefaultAsync(t => t.TripId == tripId);

        if (trip == null)
        {
            throw new ArgumentException($"Trip with ID {tripId} not found");
        }

        return await CalculateTaxExposureAsync(trip, trip.Transactions.ToList());
    }

    public async Task<TaxExposureResult> CalculateTaxExposureAsync(Trip trip, List<Transaction> transactions)
    {
        try
        {
            var taxSettings = await GetApplicableTaxSettingsAsync(trip);
            
            var result = new TaxExposureResult
            {
                TripId = trip.TripId,
                TripName = trip.TripName,
                AppliedTaxSettings = taxSettings
            };

            if (taxSettings == null)
            {
                result.TaxNote = $"No tax settings found for {trip.Country1}, fiscal year {trip.StartDate.Year}";
                return result;
            }

            var mealsCalculation = CalculateMealsExposure(trip, transactions, taxSettings);
            var lodgingCalculation = CalculateLodgingExposure(trip, transactions, taxSettings);
            var airfareAnalysis = AnalyzeAirfare(transactions);

            result.MealsExposure = mealsCalculation.TotalExposure;
            result.LodgingExposure = lodgingCalculation.TotalExposure;
            result.TotalTaxExposure = result.MealsExposure + result.LodgingExposure;
            result.HasPremiumAirfare = airfareAnalysis.HasPremiumCabins;
            result.PremiumCabinClasses = airfareAnalysis.PremiumCabinClasses;

            _logger.LogInformation("Calculated tax exposure for trip {TripId}: Meals={MealsExposure}, Lodging={LodgingExposure}, Total={TotalExposure}", 
                trip.TripId, result.MealsExposure, result.LodgingExposure, result.TotalTaxExposure);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating tax exposure for trip {TripId}", trip.TripId);
            throw;
        }
    }

    public async Task<List<TaxExposureResult>> CalculateTaxExposureForTripsAsync(List<int> tripIds)
    {
        var results = new List<TaxExposureResult>();

        foreach (var tripId in tripIds)
        {
            try
            {
                var result = await CalculateTaxExposureAsync(tripId);
                results.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating tax exposure for trip {TripId}", tripId);
                results.Add(new TaxExposureResult 
                { 
                    TripId = tripId, 
                    TaxNote = $"Error calculating tax exposure: {ex.Message}" 
                });
            }
        }

        return results;
    }

    public async Task<TaxBreakdown> GetTaxBreakdownAsync(int tripId)
    {
        var trip = await _context.Trips
            .Include(t => t.Transactions)
                .ThenInclude(t => t.Category)
            .Include(t => t.Transactions)
                .ThenInclude(t => t.CabinClass)
            .FirstOrDefaultAsync(t => t.TripId == tripId);

        if (trip == null)
        {
            throw new ArgumentException($"Trip with ID {tripId} not found");
        }

        var transactions = trip.Transactions.ToList();
        var taxSettings = await GetApplicableTaxSettingsAsync(trip);

        var breakdown = new TaxBreakdown
        {
            Summary = await CalculateTaxExposureAsync(trip, transactions)
        };

        if (taxSettings != null)
        {
            breakdown.MealsCalculation = CalculateMealsExposure(trip, transactions, taxSettings);
            breakdown.LodgingCalculation = CalculateLodgingExposure(trip, transactions, taxSettings);
        }

        breakdown.AirfareAnalysis = AnalyzeAirfare(transactions);

        return breakdown;
    }

    private async Task<Tax?> GetApplicableTaxSettingsAsync(Trip trip)
    {
        var fiscalYear = trip.StartDate.Year;
        var country = trip.Country1;

        var taxSettings = await _context.TaxRules
            .FirstOrDefaultAsync(t => 
                t.FiscalYear == fiscalYear && 
                t.Country == country);

        if (taxSettings == null)
        {
            _logger.LogWarning("No tax settings found for country {Country}, fiscal year {FiscalYear}", country, fiscalYear);
        }

        return taxSettings;
    }

    private MealsCalculation CalculateMealsExposure(Trip trip, List<Transaction> transactions, Tax taxSettings)
    {
        var mealsTransactions = transactions
            .Where(t => t.Category != null && t.Category.Name.Contains("Meals", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var totalMealsSpent = mealsTransactions.Sum(t => t.AmountUSD ?? 0);
        var tripDuration = Math.Max(1, trip.Duration);
        var mealsPerDay = totalMealsSpent / tripDuration;

        var calculation = new MealsCalculation
        {
            TotalMealsSpent = totalMealsSpent,
            MealsPerDay = mealsPerDay,
            MealsCap = taxSettings.MealsCap ?? 0,
            TripDuration = tripDuration
        };

        if (mealsPerDay > calculation.MealsCap)
        {
            calculation.ExposurePerDay = mealsPerDay - calculation.MealsCap;
            calculation.TotalExposure = calculation.ExposurePerDay * tripDuration;
        }

        return calculation;
    }

    private LodgingCalculation CalculateLodgingExposure(Trip trip, List<Transaction> transactions, Tax taxSettings)
    {
        var lodgingTransactions = transactions
            .Where(t => t.Category != null && t.Category.Name.Contains("Lodging", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var totalLodgingSpent = lodgingTransactions.Sum(t => t.AmountUSD ?? 0);
        var tripNights = Math.Max(1, trip.Duration - 1); // Duration - 1 for nights
        var lodgingPerNight = totalLodgingSpent / tripNights;

        var calculation = new LodgingCalculation
        {
            TotalLodgingSpent = totalLodgingSpent,
            LodgingPerNight = lodgingPerNight,
            LodgingCap = taxSettings.LodgingCap ?? 0,
            TripNights = tripNights
        };

        if (lodgingPerNight > calculation.LodgingCap)
        {
            calculation.ExposurePerNight = lodgingPerNight - calculation.LodgingCap;
            calculation.TotalExposure = calculation.ExposurePerNight * tripNights;
        }

        return calculation;
    }

    private AirfareAnalysis AnalyzeAirfare(List<Transaction> transactions)
    {
        var airfareTransactions = transactions
            .Where(t => t.Category != null && t.Category.Name.Contains("Airfare", StringComparison.OrdinalIgnoreCase))
            .Select(t => new AirfareTransaction
            {
                TransactionId = t.TransactionId,
                Amount = t.AmountUSD ?? 0,
                CabinClass = t.CabinClass?.Name,
                IsPremium = t.CabinClass != null && PremiumCabinClasses.Contains(t.CabinClass.Name),
                Vendor = t.Vendor ?? string.Empty,
                Date = t.TransactionDate
            })
            .ToList();

        var premiumCabinClasses = airfareTransactions
            .Where(a => a.IsPremium)
            .Select(a => a.CabinClass!)
            .Distinct()
            .ToList();

        return new AirfareAnalysis
        {
            TotalAirfareSpent = airfareTransactions.Sum(a => a.Amount),
            AirfareTransactions = airfareTransactions,
            HasPremiumCabins = premiumCabinClasses.Any(),
            PremiumCabinClasses = premiumCabinClasses
        };
    }
}