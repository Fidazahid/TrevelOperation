namespace TravelOperation.Core.Models;

/// <summary>
/// Cache key constants for consistent caching across the application
/// </summary>
public static class CacheKeys
{
    // Lookup data cache keys (1 hour TTL)
    public const string Categories = "lookup_categories";
    public const string Sources = "lookup_sources";
    public const string Purposes = "lookup_purposes";
    public const string CabinClasses = "lookup_cabin_classes";
    public const string TripTypes = "lookup_trip_types";
    public const string Status = "lookup_status";
    public const string ValidationStatus = "lookup_validation_status";
    public const string BookingTypes = "lookup_booking_types";
    public const string BookingStatus = "lookup_booking_status";
    public const string Owners = "lookup_owners";

    // Settings data cache keys (30 minutes TTL)
    public const string HeadcountAll = "settings_headcount_all";
    public const string TaxSettingsAll = "settings_tax_all";
    public const string TaxSettingsByYear = "settings_tax_year_{0}"; // Format with year
    public const string TransformationRules = "settings_transformation_rules";
    public const string CountriesAndCities = "settings_countries_cities";
    
    // Pattern for clearing all lookup caches
    public const string LookupPattern = "lookup_*";
    
    // Pattern for clearing all settings caches
    public const string SettingsPattern = "settings_*";

    /// <summary>
    /// Gets a formatted tax settings cache key for a specific fiscal year
    /// </summary>
    public static string GetTaxSettingsKey(int fiscalYear) 
        => string.Format(TaxSettingsByYear, fiscalYear);
}
