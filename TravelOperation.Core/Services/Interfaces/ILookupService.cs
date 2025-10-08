using TravelOperation.Core.Models.Lookup;
using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

public interface ILookupService
{
    Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
    Task<T?> GetByIdAsync<T>(int id) where T : class;
    Task<T> CreateAsync<T>(T entity) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task DeleteAsync<T>(int id) where T : class;
    
    Task<IEnumerable<Source>> GetSourcesAsync();
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<IEnumerable<Purpose>> GetPurposesAsync();
    Task<IEnumerable<CabinClass>> GetCabinClassesAsync();
    Task<IEnumerable<TripType>> GetTripTypesAsync();
    Task<IEnumerable<Status>> GetStatusesAsync();
    Task<IEnumerable<ValidationStatus>> GetValidationStatusesAsync();
    Task<IEnumerable<BookingType>> GetBookingTypesAsync();
    Task<IEnumerable<BookingStatus>> GetBookingStatusesAsync();
    Task<IEnumerable<Owner>> GetOwnersAsync();
}