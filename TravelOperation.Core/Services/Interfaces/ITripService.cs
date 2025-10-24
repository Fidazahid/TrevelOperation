using TravelOperation.Core.Models;
using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

public interface ITripService
{
    // Existing methods (non-paginated for backward compatibility)
    Task<IEnumerable<Trip>> GetAllTripsAsync();
    
    // New paginated methods
    Task<PagedResult<Trip>> GetAllTripsPagedAsync(PaginationParams pagination);
    Task<PagedResult<Trip>> GetTripsByEmailPagedAsync(string email, PaginationParams pagination);
    Task<PagedResult<Trip>> GetTripsReadyForValidationPagedAsync(PaginationParams pagination);
    Task<Trip?> GetTripByIdAsync(int tripId);
    Task<IEnumerable<Trip>> GetTripsByEmailAsync(string email);
    Task<IEnumerable<Trip>> GetTripsByOwnerAsync(int ownerId);
    Task<IEnumerable<Trip>> GetTripsReadyForValidationAsync();
    Task<Trip> CreateTripAsync(Trip trip);
    Task UpdateTripAsync(Trip trip);
    Task DeleteTripAsync(int tripId);
    Task<IEnumerable<Trip>> SuggestTripsFromTransactionsAsync();
    Task ValidateTripAsync(int tripId);
    Task<decimal> CalculateTotalSpendAsync(int tripId);
    Task<Dictionary<string, decimal>> CalculateCategorySpendAsync(int tripId);
    Task<decimal> CalculateTaxExposureAsync(int tripId);
    Task<int> CalculateTripDurationAsync(DateTime startDate, DateTime endDate);
}