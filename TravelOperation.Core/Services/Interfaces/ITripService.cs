using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

public interface ITripService
{
    Task<IEnumerable<Trip>> GetAllTripsAsync();
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