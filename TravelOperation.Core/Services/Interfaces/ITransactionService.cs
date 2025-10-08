using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<Transaction?> GetTransactionByIdAsync(string transactionId);
    Task<Transaction?> GetByIdAsync(string transactionId);
    Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email);
    Task<IEnumerable<Transaction>> GetTransactionsByEmailAndDateRangeAsync(string email, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Transaction>> GetUnlinkedTransactionsAsync();
    Task<IEnumerable<Transaction>> GetTransactionsByTripIdAsync(int tripId);
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task UpdateTransactionAsync(Transaction transaction);
    Task DeleteTransactionAsync(string transactionId);
    Task LinkTransactionToTripAsync(string transactionId, int tripId);
    Task UnlinkTransactionFromTripAsync(string transactionId);
    Task<IEnumerable<Transaction>> SplitTransactionAsync(string transactionId, List<Transaction> splitTransactions);
    Task MarkTransactionAsValidAsync(string transactionId);
    Task<IEnumerable<Transaction>> GetAirfareWithoutCabinClassAsync();
    Task<IEnumerable<Transaction>> GetHighValueMealsAsync(decimal threshold = 80);
    Task<IEnumerable<Transaction>> GetLowValueLodgingAsync(decimal threshold = 100);
    Task<IEnumerable<Transaction>> GetClientEntertainmentWithoutParticipantsAsync();
    Task<IEnumerable<Transaction>> GetOtherCategoryTransactionsAsync();
    Task<IEnumerable<Transaction>> GetTransactionsWithoutDocumentationAsync();
    Task<IEnumerable<Transaction>> SearchTransactionsAsync(string searchQuery, int pageSize = 50, bool includeAlreadySplit = true);
    Task<List<Transaction>> GetAllTransactionsAsync(bool includeRelated = false);
    Task MarkAsValidAsync(string transactionId);
    Task DeleteAsync(string transactionId);
    Task UnlinkFromTripAsync(string transactionId);
    Task UpdateTransactionAsync(object updateDto);
}