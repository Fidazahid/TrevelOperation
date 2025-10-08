using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Lookup;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Services;

public class LookupService : ILookupService
{
    private readonly TravelDbContext _context;

    public LookupService(TravelDbContext context)
    {
        _context = context;
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
        return entity;
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync<T>(int id) where T : class
    {
        var entity = await GetByIdAsync<T>(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Source>> GetSourcesAsync()
    {
        return await _context.Sources
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Purpose>> GetPurposesAsync()
    {
        return await _context.Purposes
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CabinClass>> GetCabinClassesAsync()
    {
        return await _context.CabinClasses
            .OrderBy(cc => cc.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<TripType>> GetTripTypesAsync()
    {
        return await _context.TripTypes
            .OrderBy(tt => tt.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Status>> GetStatusesAsync()
    {
        return await _context.Statuses
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<ValidationStatus>> GetValidationStatusesAsync()
    {
        return await _context.ValidationStatuses
            .OrderBy(vs => vs.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingType>> GetBookingTypesAsync()
    {
        return await _context.BookingTypes
            .OrderBy(bt => bt.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingStatus>> GetBookingStatusesAsync()
    {
        return await _context.BookingStatuses
            .OrderBy(bs => bs.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Owner>> GetOwnersAsync()
    {
        return await _context.Owners
            .OrderBy(o => o.Name)
            .ToListAsync();
    }
}