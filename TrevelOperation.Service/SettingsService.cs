using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Models.Lookup;

namespace TrevelOperation.Service;

public class SettingsService : ISettingsService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<SettingsService> _logger;

    public SettingsService(TravelDbContext context, ILogger<SettingsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Categories Management

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category> CreateCategoryAsync(string name, string emoji = "")
    {
        var category = new Category { Name = name, Emoji = emoji };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created category: {Name}", name);
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(int categoryId, string name, string emoji = "")
    {
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {categoryId} not found");

        category.Name = name;
        category.Emoji = emoji;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated category: {CategoryId} -> {Name}", categoryId, name);
        return category;
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        var category = await _context.Categories.FindAsync(categoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {categoryId} not found");

        // Check if category is in use
        var transactionCount = await _context.Transactions.CountAsync(t => t.CategoryId == categoryId);
        if (transactionCount > 0)
            throw new InvalidOperationException($"Cannot delete category '{category.Name}' - it is used by {transactionCount} transactions");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted category: {Name}", category.Name);
    }

    #endregion

    #region Sources Management

    public async Task<List<Source>> GetSourcesAsync()
    {
        return await _context.Sources.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Source> CreateSourceAsync(string name)
    {
        var source = new Source { Name = name };
        _context.Sources.Add(source);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created source: {Name}", name);
        return source;
    }

    public async Task<Source> UpdateSourceAsync(int sourceId, string name)
    {
        var source = await _context.Sources.FindAsync(sourceId);
        if (source == null)
            throw new InvalidOperationException($"Source with ID {sourceId} not found");

        source.Name = name;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated source: {SourceId} -> {Name}", sourceId, name);
        return source;
    }

    public async Task DeleteSourceAsync(int sourceId)
    {
        var source = await _context.Sources.FindAsync(sourceId);
        if (source == null)
            throw new InvalidOperationException($"Source with ID {sourceId} not found");

        var transactionCount = await _context.Transactions.CountAsync(t => t.SourceId == sourceId);
        if (transactionCount > 0)
            throw new InvalidOperationException($"Cannot delete source '{source.Name}' - it is used by {transactionCount} transactions");

        _context.Sources.Remove(source);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted source: {Name}", source.Name);
    }

    #endregion

    #region Purposes Management

    public async Task<List<Purpose>> GetPurposesAsync()
    {
        return await _context.Purposes.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Purpose> CreatePurposeAsync(string name, string emoji = "")
    {
        var purpose = new Purpose { Name = name, Emoji = emoji };
        _context.Purposes.Add(purpose);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created purpose: {Name}", name);
        return purpose;
    }

    public async Task<Purpose> UpdatePurposeAsync(int purposeId, string name, string emoji = "")
    {
        var purpose = await _context.Purposes.FindAsync(purposeId);
        if (purpose == null)
            throw new InvalidOperationException($"Purpose with ID {purposeId} not found");

        purpose.Name = name;
        purpose.Emoji = emoji;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated purpose: {PurposeId} -> {Name}", purposeId, name);
        return purpose;
    }

    public async Task DeletePurposeAsync(int purposeId)
    {
        var purpose = await _context.Purposes.FindAsync(purposeId);
        if (purpose == null)
            throw new InvalidOperationException($"Purpose with ID {purposeId} not found");

        var tripCount = await _context.Trips.CountAsync(t => t.PurposeId == purposeId);
        if (tripCount > 0)
            throw new InvalidOperationException($"Cannot delete purpose '{purpose.Name}' - it is used by {tripCount} trips");

        _context.Purposes.Remove(purpose);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted purpose: {Name}", purpose.Name);
    }

    #endregion

    #region CabinClass Management

    public async Task<List<CabinClass>> GetCabinClassesAsync()
    {
        return await _context.CabinClasses.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<CabinClass> CreateCabinClassAsync(string name, string emoji = "")
    {
        var cabinClass = new CabinClass { Name = name, Emoji = emoji };
        _context.CabinClasses.Add(cabinClass);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created cabin class: {Name}", name);
        return cabinClass;
    }

    public async Task<CabinClass> UpdateCabinClassAsync(int cabinClassId, string name, string emoji = "")
    {
        var cabinClass = await _context.CabinClasses.FindAsync(cabinClassId);
        if (cabinClass == null)
            throw new InvalidOperationException($"Cabin class with ID {cabinClassId} not found");

        cabinClass.Name = name;
        cabinClass.Emoji = emoji;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated cabin class: {CabinClassId} -> {Name}", cabinClassId, name);
        return cabinClass;
    }

    public async Task DeleteCabinClassAsync(int cabinClassId)
    {
        var cabinClass = await _context.CabinClasses.FindAsync(cabinClassId);
        if (cabinClass == null)
            throw new InvalidOperationException($"Cabin class with ID {cabinClassId} not found");

        var transactionCount = await _context.Transactions.CountAsync(t => t.CabinClassId == cabinClassId);
        if (transactionCount > 0)
            throw new InvalidOperationException($"Cannot delete cabin class '{cabinClass.Name}' - it is used by {transactionCount} transactions");

        _context.CabinClasses.Remove(cabinClass);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted cabin class: {Name}", cabinClass.Name);
    }

    #endregion

    #region TripType Management

    public async Task<List<TripType>> GetTripTypesAsync()
    {
        return await _context.TripTypes.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<TripType> CreateTripTypeAsync(string name, string emoji = "")
    {
        var tripType = new TripType { Name = name, Emoji = emoji };
        _context.TripTypes.Add(tripType);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created trip type: {Name}", name);
        return tripType;
    }

    public async Task<TripType> UpdateTripTypeAsync(int tripTypeId, string name, string emoji = "")
    {
        var tripType = await _context.TripTypes.FindAsync(tripTypeId);
        if (tripType == null)
            throw new InvalidOperationException($"Trip type with ID {tripTypeId} not found");

        tripType.Name = name;
        tripType.Emoji = emoji;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated trip type: {TripTypeId} -> {Name}", tripTypeId, name);
        return tripType;
    }

    public async Task DeleteTripTypeAsync(int tripTypeId)
    {
        var tripType = await _context.TripTypes.FindAsync(tripTypeId);
        if (tripType == null)
            throw new InvalidOperationException($"Trip type with ID {tripTypeId} not found");

        var tripCount = await _context.Trips.CountAsync(t => t.TripTypeId == tripTypeId);
        if (tripCount > 0)
            throw new InvalidOperationException($"Cannot delete trip type '{tripType.Name}' - it is used by {tripCount} trips");

        _context.TripTypes.Remove(tripType);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted trip type: {Name}", tripType.Name);
    }

    #endregion

    #region Status Management

    public async Task<List<Status>> GetStatusesAsync()
    {
        return await _context.Statuses.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Status> CreateStatusAsync(string name, string emoji = "")
    {
        var status = new Status { Name = name, Emoji = emoji };
        _context.Statuses.Add(status);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created status: {Name}", name);
        return status;
    }

    public async Task<Status> UpdateStatusAsync(int statusId, string name, string emoji = "")
    {
        var status = await _context.Statuses.FindAsync(statusId);
        if (status == null)
            throw new InvalidOperationException($"Status with ID {statusId} not found");

        status.Name = name;
        status.Emoji = emoji;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated status: {StatusId} -> {Name}", statusId, name);
        return status;
    }

    public async Task DeleteStatusAsync(int statusId)
    {
        var status = await _context.Statuses.FindAsync(statusId);
        if (status == null)
            throw new InvalidOperationException($"Status with ID {statusId} not found");

        var tripCount = await _context.Trips.CountAsync(t => t.StatusId == statusId);
        if (tripCount > 0)
            throw new InvalidOperationException($"Cannot delete status '{status.Name}' - it is used by {tripCount} trips");

        _context.Statuses.Remove(status);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted status: {Name}", status.Name);
    }

    #endregion

    #region ValidationStatus Management

    public async Task<List<ValidationStatus>> GetValidationStatusesAsync()
    {
        return await _context.ValidationStatuses.OrderBy(v => v.Name).ToListAsync();
    }

    public async Task<ValidationStatus> CreateValidationStatusAsync(string name, string emoji = "")
    {
        var validationStatus = new ValidationStatus { Name = name, Emoji = emoji };
        _context.ValidationStatuses.Add(validationStatus);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created validation status: {Name}", name);
        return validationStatus;
    }

    public async Task<ValidationStatus> UpdateValidationStatusAsync(int validationStatusId, string name, string emoji = "")
    {
        var validationStatus = await _context.ValidationStatuses.FindAsync(validationStatusId);
        if (validationStatus == null)
            throw new InvalidOperationException($"Validation status with ID {validationStatusId} not found");

        validationStatus.Name = name;
        validationStatus.Emoji = emoji;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated validation status: {ValidationStatusId} -> {Name}", validationStatusId, name);
        return validationStatus;
    }

    public async Task DeleteValidationStatusAsync(int validationStatusId)
    {
        var validationStatus = await _context.ValidationStatuses.FindAsync(validationStatusId);
        if (validationStatus == null)
            throw new InvalidOperationException($"Validation status with ID {validationStatusId} not found");

        var tripCount = await _context.Trips.CountAsync(t => t.ValidationStatusId == validationStatusId);
        if (tripCount > 0)
            throw new InvalidOperationException($"Cannot delete validation status '{validationStatus.Name}' - it is used by {tripCount} trips");

        _context.ValidationStatuses.Remove(validationStatus);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted validation status: {Name}", validationStatus.Name);
    }

    #endregion

    #region BookingType Management

    public async Task<List<BookingType>> GetBookingTypesAsync()
    {
        return await _context.BookingTypes.OrderBy(b => b.Name).ToListAsync();
    }

    public async Task<BookingType> CreateBookingTypeAsync(string name, string emoji = "")
    {
        var bookingType = new BookingType { Name = name, Emoji = emoji };
        _context.BookingTypes.Add(bookingType);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created booking type: {Name}", name);
        return bookingType;
    }

    public async Task<BookingType> UpdateBookingTypeAsync(int bookingTypeId, string name, string emoji = "")
    {
        var bookingType = await _context.BookingTypes.FindAsync(bookingTypeId);
        if (bookingType == null)
            throw new InvalidOperationException($"Booking type with ID {bookingTypeId} not found");

        bookingType.Name = name;
        bookingType.Emoji = emoji;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated booking type: {BookingTypeId} -> {Name}", bookingTypeId, name);
        return bookingType;
    }

    public async Task DeleteBookingTypeAsync(int bookingTypeId)
    {
        var bookingType = await _context.BookingTypes.FindAsync(bookingTypeId);
        if (bookingType == null)
            throw new InvalidOperationException($"Booking type with ID {bookingTypeId} not found");

        var transactionCount = await _context.Transactions.CountAsync(t => t.BookingTypeId == bookingTypeId);
        if (transactionCount > 0)
            throw new InvalidOperationException($"Cannot delete booking type '{bookingType.Name}' - it is used by {transactionCount} transactions");

        _context.BookingTypes.Remove(bookingType);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted booking type: {Name}", bookingType.Name);
    }

    #endregion

    #region BookingStatus Management

    public async Task<List<BookingStatus>> GetBookingStatusesAsync()
    {
        return await _context.BookingStatuses.OrderBy(b => b.Name).ToListAsync();
    }

    public async Task<BookingStatus> CreateBookingStatusAsync(string name, string emoji = "")
    {
        var bookingStatus = new BookingStatus { Name = name, Emoji = emoji };
        _context.BookingStatuses.Add(bookingStatus);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created booking status: {Name}", name);
        return bookingStatus;
    }

    public async Task<BookingStatus> UpdateBookingStatusAsync(int bookingStatusId, string name, string emoji = "")
    {
        var bookingStatus = await _context.BookingStatuses.FindAsync(bookingStatusId);
        if (bookingStatus == null)
            throw new InvalidOperationException($"Booking status with ID {bookingStatusId} not found");

        bookingStatus.Name = name;
        bookingStatus.Emoji = emoji;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated booking status: {BookingStatusId} -> {Name}", bookingStatusId, name);
        return bookingStatus;
    }

    public async Task DeleteBookingStatusAsync(int bookingStatusId)
    {
        var bookingStatus = await _context.BookingStatuses.FindAsync(bookingStatusId);
        if (bookingStatus == null)
            throw new InvalidOperationException($"Booking status with ID {bookingStatusId} not found");

        var transactionCount = await _context.Transactions.CountAsync(t => t.BookingStatusId == bookingStatusId);
        if (transactionCount > 0)
            throw new InvalidOperationException($"Cannot delete booking status '{bookingStatus.Name}' - it is used by {transactionCount} transactions");

        _context.BookingStatuses.Remove(bookingStatus);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted booking status: {Name}", bookingStatus.Name);
    }

    #endregion

    #region Owners Management

    public async Task<List<Owner>> GetOwnersAsync()
    {
        return await _context.Owners.OrderBy(o => o.Name).ToListAsync();
    }

    public async Task<Owner> CreateOwnerAsync(string name, string email, string? costCenter = null, string? department = null, string? domain = null)
    {
        var owner = new Owner 
        { 
            Name = name, 
            Email = email, 
            CostCenter = costCenter, 
            Department = department, 
            Domain = domain 
        };
        _context.Owners.Add(owner);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created owner: {Name} ({Email})", name, email);
        return owner;
    }

    public async Task<Owner> UpdateOwnerAsync(int ownerId, string name, string email, string? costCenter = null, string? department = null, string? domain = null)
    {
        var owner = await _context.Owners.FindAsync(ownerId);
        if (owner == null)
            throw new InvalidOperationException($"Owner with ID {ownerId} not found");

        owner.Name = name;
        owner.Email = email;
        owner.CostCenter = costCenter;
        owner.Department = department;
        owner.Domain = domain;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated owner: {OwnerId} -> {Name} ({Email})", ownerId, name, email);
        return owner;
    }

    public async Task DeleteOwnerAsync(int ownerId)
    {
        var owner = await _context.Owners.FindAsync(ownerId);
        if (owner == null)
            throw new InvalidOperationException($"Owner with ID {ownerId} not found");

        var tripCount = await _context.Trips.CountAsync(t => t.OwnerId == ownerId);
        if (tripCount > 0)
            throw new InvalidOperationException($"Cannot delete owner '{owner.Name}' - they own {tripCount} trips");

        _context.Owners.Remove(owner);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted owner: {Name}", owner.Name);
    }

    #endregion

    #region Countries & Cities Management

    public async Task<List<Country>> GetCountriesAsync()
    {
        // Since we don't have Country/City entities in the schema, we'll extract from Trips
        var countries = await _context.Trips
            .Where(t => !string.IsNullOrEmpty(t.Country1))
            .Select(t => t.Country1!)
            .Union(_context.Trips
                .Where(t => !string.IsNullOrEmpty(t.Country2))
                .Select(t => t.Country2!))
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        var result = new List<Country>();
        foreach (var countryName in countries)
        {
            var cities = await GetCitiesByCountryAsync(countryName);
            result.Add(new Country { Name = countryName, Cities = cities });
        }

        return result;
    }

    public async Task<List<City>> GetCitiesByCountryAsync(string countryName)
    {
        var cities = await _context.Trips
            .Where(t => t.Country1 == countryName && !string.IsNullOrEmpty(t.City1))
            .Select(t => t.City1!)
            .Union(_context.Trips
                .Where(t => t.Country2 == countryName && !string.IsNullOrEmpty(t.City2))
                .Select(t => t.City2!))
            .Distinct()
            .OrderBy(c => c)
            .Select(c => new City { Name = c, CountryName = countryName })
            .ToListAsync();

        return cities;
    }

    public async Task<Country> CreateCountryAsync(string name)
    {
        // For now, just return a new country object
        // In a full implementation, this would be stored in a dedicated table
        return await Task.FromResult(new Country { Name = name, Cities = new List<City>() });
    }

    public async Task<City> CreateCityAsync(string name, string countryName)
    {
        // For now, just return a new city object
        // In a full implementation, this would be stored in a dedicated table
        return await Task.FromResult(new City { Name = name, CountryName = countryName });
    }

    public async Task DeleteCountryAsync(string countryName)
    {
        // In a full implementation, this would delete from a dedicated table
        await Task.CompletedTask;
        _logger.LogInformation("Deleted country: {Name}", countryName);
    }

    public async Task DeleteCityAsync(string cityName, string countryName)
    {
        // In a full implementation, this would delete from a dedicated table
        await Task.CompletedTask;
        _logger.LogInformation("Deleted city: {CityName}, {CountryName}", cityName, countryName);
    }

    public async Task ImportCountriesAndCitiesAsync(Stream csvStream)
    {
        // Parse CSV and import countries/cities
        // For now, just log the operation
        _logger.LogInformation("Importing countries and cities from CSV");
        await Task.CompletedTask;
    }

    #endregion

    #region Tax Settings Management

    public async Task<List<Tax>> GetTaxSettingsAsync()
    {
        return await _context.TaxRules.OrderBy(t => t.FiscalYear).ThenBy(t => t.Country).ToListAsync();
    }

    public async Task<Tax> CreateTaxSettingAsync(int fiscalYear, string country, string subsidiary, decimal mealsCap, decimal lodgingCap, decimal taxShield)
    {
        var tax = new Tax
        {
            FiscalYear = fiscalYear,
            Country = country,
            Subsidiary = subsidiary,
            MealsCap = mealsCap,
            LodgingCap = lodgingCap,
            TaxShield = taxShield
        };
        _context.TaxRules.Add(tax);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created tax setting: {FiscalYear} {Country} {Subsidiary}", fiscalYear, country, subsidiary);
        return tax;
    }

    public async Task<Tax> UpdateTaxSettingAsync(int taxId, int fiscalYear, string country, string subsidiary, decimal mealsCap, decimal lodgingCap, decimal taxShield)
    {
        var tax = await _context.TaxRules.FindAsync(taxId);
        if (tax == null)
            throw new InvalidOperationException($"Tax setting with ID {taxId} not found");

        tax.FiscalYear = fiscalYear;
        tax.Country = country;
        tax.Subsidiary = subsidiary;
        tax.MealsCap = mealsCap;
        tax.LodgingCap = lodgingCap;
        tax.TaxShield = taxShield;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated tax setting: {TaxId} -> {FiscalYear} {Country} {Subsidiary}", taxId, fiscalYear, country, subsidiary);
        return tax;
    }

    public async Task DeleteTaxSettingAsync(int taxId)
    {
        var tax = await _context.TaxRules.FindAsync(taxId);
        if (tax == null)
            throw new InvalidOperationException($"Tax setting with ID {taxId} not found");

        _context.TaxRules.Remove(tax);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted tax setting: {FiscalYear} {Country} {Subsidiary}", tax.FiscalYear, tax.Country, tax.Subsidiary);
    }

    #endregion

    #region Headcount Management

    public async Task<List<Headcount>> GetHeadcountAsync()
    {
        return await _context.Headcount.OrderByDescending(h => h.Period).ThenBy(h => h.LastName).ToListAsync();
    }

    public async Task<Headcount?> GetHeadcountByEmailAsync(string email)
    {
        // Get the most recent headcount record for this email
        return await _context.Headcount
            .Where(h => h.Email.ToLower() == email.ToLower())
            .OrderByDescending(h => h.Period)
            .FirstOrDefaultAsync();
    }

    public async Task ImportHeadcountAsync(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream, Encoding.UTF8);
        var lines = new List<string>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lines.Add(line);
        }

        if (lines.Count == 0) return;

        // Assume first line is header
        var headerLine = lines[0];
        var headers = headerLine.Split(',');

        for (int i = 1; i < lines.Count; i++)
        {
            try
            {
                var values = lines[i].Split(',');
                if (values.Length >= headers.Length)
                {
                    var headcount = new Headcount
                    {
                        Period = DateTime.Parse(values[0]),
                        UserId = values[1],
                        Email = values[2],
                        FirstName = values[3],
                        LastName = values[4],
                        Subsidiary = values[5],
                        Site = values[6],
                        Department = values[7],
                        Domain = values[8],
                        CostCenter = values[9]
                    };

                    _context.Headcount.Add(headcount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing headcount line {LineNumber}: {Line}", i + 1, lines[i]);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Imported {Count} headcount records", lines.Count - 1);
    }

    public async Task<Headcount> CreateHeadcountAsync(DateTime period, string userId, string email, string firstName, string lastName, 
        string subsidiary, string site, string department, string domain, string costCenter)
    {
        var headcount = new Headcount
        {
            Period = period,
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Subsidiary = subsidiary,
            Site = site,
            Department = department,
            Domain = domain,
            CostCenter = costCenter
        };
        _context.Headcount.Add(headcount);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created headcount: {FirstName} {LastName} ({Email})", firstName, lastName, email);
        return headcount;
    }

    public async Task<Headcount> UpdateHeadcountAsync(int headcountId, DateTime period, string userId, string email, string firstName, string lastName, 
        string subsidiary, string site, string department, string domain, string costCenter)
    {
        var headcount = await _context.Headcount.FindAsync(headcountId);
        if (headcount == null)
            throw new InvalidOperationException($"Headcount with ID {headcountId} not found");

        headcount.Period = period;
        headcount.UserId = userId;
        headcount.Email = email;
        headcount.FirstName = firstName;
        headcount.LastName = lastName;
        headcount.Subsidiary = subsidiary;
        headcount.Site = site;
        headcount.Department = department;
        headcount.Domain = domain;
        headcount.CostCenter = costCenter;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated headcount: {HeadcountId} -> {FirstName} {LastName}", headcountId, firstName, lastName);
        return headcount;
    }

    public async Task DeleteHeadcountAsync(int headcountId)
    {
        var headcount = await _context.Headcount.FindAsync(headcountId);
        if (headcount == null)
            throw new InvalidOperationException($"Headcount with ID {headcountId} not found");

        _context.Headcount.Remove(headcount);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted headcount: {FirstName} {LastName}", headcount.FirstName, headcount.LastName);
    }

    #endregion

    #region Countries & Cities (Combined)

    public async Task<List<CountryCity>> GetCountriesCitiesAsync()
    {
        return await _context.CountriesCities.OrderBy(cc => cc.Country).ThenBy(cc => cc.City).ToListAsync();
    }

    public async Task<CountryCity> CreateCountryCityAsync(string country, string city)
    {
        var countryCity = new CountryCity
        {
            Country = country,
            City = city,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = "System"
        };

        _context.CountriesCities.Add(countryCity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created country/city: {Country} - {City}", country, city);
        return countryCity;
    }

    public async Task<CountryCity> UpdateCountryCityAsync(int countryCityId, string country, string city)
    {
        var countryCity = await _context.CountriesCities.FindAsync(countryCityId);
        if (countryCity == null)
            throw new InvalidOperationException($"CountryCity with ID {countryCityId} not found");

        countryCity.Country = country;
        countryCity.City = city;
        countryCity.ModifiedAt = DateTime.UtcNow;
        countryCity.ModifiedBy = "System";

        _context.CountriesCities.Update(countryCity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated country/city: {CountryCityId} -> {Country} - {City}", countryCityId, country, city);
        return countryCity;
    }

    public async Task DeleteCountryCityAsync(int countryCityId)
    {
        var countryCity = await _context.CountriesCities.FindAsync(countryCityId);
        if (countryCity == null)
            throw new InvalidOperationException($"CountryCity with ID {countryCityId} not found");

        _context.CountriesCities.Remove(countryCity);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted country/city: {Country} - {City}", countryCity.Country, countryCity.City);
    }

    #endregion

    #region System Settings

    public async Task<Dictionary<string, string>> GetSystemSettingsAsync()
    {
        // For now, return a static set of settings
        // In a full implementation, this would come from a Settings table
        return await Task.FromResult(new Dictionary<string, string>
        {
            ["DefaultCurrency"] = "USD",
            ["DateFormat"] = "dd/MM/yyyy",
            ["NumberFormat"] = "1,000.00",
            ["DefaultTimeZone"] = "Israel Standard Time",
            ["MaxFileUploadSize"] = "10485760", // 10MB
            ["EmailNotifications"] = "true",
            ["AutoBackup"] = "true",
            ["BackupRetentionDays"] = "30",
            ["AuditLogRetentionDays"] = "365"
        });
    }

    public async Task UpdateSystemSettingAsync(string key, string value)
    {
        // In a full implementation, this would update a Settings table
        _logger.LogInformation("Updated system setting: {Key} = {Value}", key, value);
        await Task.CompletedTask;
    }

    public async Task UpdateSystemSettingsAsync(Dictionary<string, string> settings)
    {
        // In a full implementation, this would update the SystemSettings table
        foreach (var setting in settings)
        {
            await UpdateSystemSettingAsync(setting.Key, setting.Value);
        }
        _logger.LogInformation("Updated {Count} system settings", settings.Count);
    }

    public async Task<string?> GetSystemSettingAsync(string key)
    {
        var settings = await GetSystemSettingsAsync();
        return settings.TryGetValue(key, out var value) ? value : null;
    }

    #endregion
    
    #region Additional Helper Methods
    
    public async Task<List<Headcount>> GetAllHeadcountAsync()
    {
        // Alias for consistency with other methods
        return await GetHeadcountAsync();
    }
    
    public async Task<Transaction?> GetTransactionByIdAsync(string transactionId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Source)
            .Include(t => t.CabinClass)
            .Include(t => t.BookingStatus)
            .Include(t => t.BookingType)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }
    
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        // Alias for consistency with other methods
        return await GetCategoriesAsync();
    }
    
    #endregion
}