using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Models.Lookup;

namespace TrevelOperation.Service;

public interface ISettingsService
{
    // Lookup Lists Management
    Task<List<Category>> GetCategoriesAsync();
    Task<Category> CreateCategoryAsync(string name, string emoji = "");
    Task<Category> UpdateCategoryAsync(int categoryId, string name, string emoji = "");
    Task DeleteCategoryAsync(int categoryId);

    Task<List<Source>> GetSourcesAsync();
    Task<Source> CreateSourceAsync(string name);
    Task<Source> UpdateSourceAsync(int sourceId, string name);
    Task DeleteSourceAsync(int sourceId);

    Task<List<Purpose>> GetPurposesAsync();
    Task<Purpose> CreatePurposeAsync(string name, string emoji = "");
    Task<Purpose> UpdatePurposeAsync(int purposeId, string name, string emoji = "");
    Task DeletePurposeAsync(int purposeId);

    Task<List<CabinClass>> GetCabinClassesAsync();
    Task<CabinClass> CreateCabinClassAsync(string name, string emoji = "");
    Task<CabinClass> UpdateCabinClassAsync(int cabinClassId, string name, string emoji = "");
    Task DeleteCabinClassAsync(int cabinClassId);

    Task<List<TripType>> GetTripTypesAsync();
    Task<TripType> CreateTripTypeAsync(string name, string emoji = "");
    Task<TripType> UpdateTripTypeAsync(int tripTypeId, string name, string emoji = "");
    Task DeleteTripTypeAsync(int tripTypeId);

    Task<List<Status>> GetStatusesAsync();
    Task<Status> CreateStatusAsync(string name, string emoji = "");
    Task<Status> UpdateStatusAsync(int statusId, string name, string emoji = "");
    Task DeleteStatusAsync(int statusId);

    Task<List<ValidationStatus>> GetValidationStatusesAsync();
    Task<ValidationStatus> CreateValidationStatusAsync(string name, string emoji = "");
    Task<ValidationStatus> UpdateValidationStatusAsync(int validationStatusId, string name, string emoji = "");
    Task DeleteValidationStatusAsync(int validationStatusId);

    Task<List<BookingType>> GetBookingTypesAsync();
    Task<BookingType> CreateBookingTypeAsync(string name, string emoji = "");
    Task<BookingType> UpdateBookingTypeAsync(int bookingTypeId, string name, string emoji = "");
    Task DeleteBookingTypeAsync(int bookingTypeId);

    Task<List<BookingStatus>> GetBookingStatusesAsync();
    Task<BookingStatus> CreateBookingStatusAsync(string name, string emoji = "");
    Task<BookingStatus> UpdateBookingStatusAsync(int bookingStatusId, string name, string emoji = "");
    Task DeleteBookingStatusAsync(int bookingStatusId);

    // Owners Management
    Task<List<Owner>> GetOwnersAsync();
    Task<Owner> CreateOwnerAsync(string name, string email, string? costCenter = null, string? department = null, string? domain = null);
    Task<Owner> UpdateOwnerAsync(int ownerId, string name, string email, string? costCenter = null, string? department = null, string? domain = null);
    Task DeleteOwnerAsync(int ownerId);

    // Countries & Cities Management
    Task<List<Country>> GetCountriesAsync();
    Task<List<City>> GetCitiesByCountryAsync(string countryName);
    Task<Country> CreateCountryAsync(string name);
    Task<City> CreateCityAsync(string name, string countryName);
    Task DeleteCountryAsync(string countryName);
    Task DeleteCityAsync(string cityName, string countryName);
    Task ImportCountriesAndCitiesAsync(Stream csvStream);
    
    // CountryCity Management (Combined entities)
    Task<List<CountryCity>> GetCountriesCitiesAsync();
    Task<CountryCity> CreateCountryCityAsync(string country, string city);
    Task<CountryCity> UpdateCountryCityAsync(int countryCityId, string country, string city);
    Task DeleteCountryCityAsync(int countryCityId);

    // Tax Settings Management
    Task<List<Tax>> GetTaxSettingsAsync();
    Task<Tax> CreateTaxSettingAsync(int fiscalYear, string country, string subsidiary, decimal mealsCap, decimal lodgingCap, decimal taxShield);
    Task<Tax> UpdateTaxSettingAsync(int taxId, int fiscalYear, string country, string subsidiary, decimal mealsCap, decimal lodgingCap, decimal taxShield);
    Task DeleteTaxSettingAsync(int taxId);

    // Headcount Management
    Task<List<Headcount>> GetHeadcountAsync();
    Task<List<Headcount>> GetAllHeadcountAsync(); // Alias for consistency
    Task<Headcount?> GetHeadcountByEmailAsync(string email);
    Task ImportHeadcountAsync(Stream csvStream);
    Task<Headcount> CreateHeadcountAsync(DateTime period, string userId, string email, string firstName, string lastName, 
        string subsidiary, string site, string department, string domain, string costCenter);
    Task<Headcount> UpdateHeadcountAsync(int headcountId, DateTime period, string userId, string email, string firstName, string lastName, 
        string subsidiary, string site, string department, string domain, string costCenter);
    Task DeleteHeadcountAsync(int headcountId);
    
    // Transaction queries for split engine
    Task<Transaction?> GetTransactionByIdAsync(string transactionId);
    
    // Category queries (aliases for consistency)
    Task<List<Category>> GetAllCategoriesAsync(); // Alias for GetCategoriesAsync

    // System Settings
    Task<Dictionary<string, string>> GetSystemSettingsAsync();
    Task UpdateSystemSettingAsync(string key, string value);
    Task UpdateSystemSettingsAsync(Dictionary<string, string> settings);
    Task<string?> GetSystemSettingAsync(string key);
}

public class Country
{
    public string Name { get; set; } = string.Empty;
    public List<City> Cities { get; set; } = new();
}

public class City
{
    public string Name { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
}