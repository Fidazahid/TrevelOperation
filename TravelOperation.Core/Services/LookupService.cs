using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Lookup;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;
using TravelOperation.Core.Interfaces;
using TravelOperation.Core.Models;

namespace TravelOperation.Core.Services;

public class LookupService : ILookupService
{
    private readonly TravelDbContext _context;
    private readonly ICacheService _cacheService;

    public LookupService(TravelDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync<T>(int id) where T : class
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T> CreateAsync<T>(T entity) where T : class
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        
        // Invalidate lookup cache after creating
        _cacheService.RemoveByPattern(CacheKeys.LookupPattern);
        
        return entity;
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        
        // Invalidate lookup cache after updating
        _cacheService.RemoveByPattern(CacheKeys.LookupPattern);
    }

    public async Task DeleteAsync<T>(int id) where T : class
    {
        var entity = await GetByIdAsync<T>(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            
            // Invalidate lookup cache after deleting
            _cacheService.RemoveByPattern(CacheKeys.LookupPattern);
        }
    }

    public async Task<IEnumerable<Source>> GetSourcesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.Sources,
            async () => await _context.Sources.OrderBy(s => s.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.Categories,
            async () => await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<Purpose>> GetPurposesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.Purposes,
            async () => await _context.Purposes.OrderBy(p => p.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<CabinClass>> GetCabinClassesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.CabinClasses,
            async () => await _context.CabinClasses.OrderBy(cc => cc.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<TripType>> GetTripTypesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.TripTypes,
            async () => await _context.TripTypes.OrderBy(tt => tt.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<Status>> GetStatusesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.Status,
            async () => await _context.Statuses.OrderBy(s => s.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<ValidationStatus>> GetValidationStatusesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.ValidationStatus,
            async () => await _context.ValidationStatuses.OrderBy(vs => vs.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<BookingType>> GetBookingTypesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.BookingTypes,
            async () => await _context.BookingTypes.OrderBy(bt => bt.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<BookingStatus>> GetBookingStatusesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.BookingStatus,
            async () => await _context.BookingStatuses.OrderBy(bs => bs.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<Owner>> GetOwnersAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.Owners,
            async () => await _context.Owners.OrderBy(o => o.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<Country>> GetCountriesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.CountriesAndCities,
            async () => await _context.Countries.OrderBy(c => c.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _cacheService.GetOrCreateAsync(
            CacheKeys.CountriesAndCities + "_cities",
            async () => await _context.Cities.OrderBy(c => c.Name).ToListAsync(),
            60 // 1 hour TTL
        );
    }
}