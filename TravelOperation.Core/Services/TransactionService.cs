using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Services;

public class TransactionService : ITransactionService
{
    private readonly TravelDbContext _context;
    private readonly IAuditService _auditService;

    public TransactionService(TravelDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.Trip)
            .Include(t => t.CabinClass)
            .Include(t => t.BookingStatus)
            .Include(t => t.BookingType)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(string transactionId)
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.Trip)
            .Include(t => t.CabinClass)
            .Include(t => t.BookingStatus)
            .Include(t => t.BookingType)
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByEmailAsync(string email)
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.Trip)
            .Where(t => t.Email == email)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetUnlinkedTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.TripId == null)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByTripIdAsync(int tripId)
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.CabinClass)
            .Where(t => t.TripId == tripId)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.ModifiedAt = DateTime.UtcNow;
        
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Create", "Transactions", transaction.TransactionId, null, transaction);
        
        return transaction;
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        var existing = await GetTransactionByIdAsync(transaction.TransactionId);
        if (existing == null)
            throw new ArgumentException($"Transaction with ID {transaction.TransactionId} not found");

        transaction.ModifiedAt = DateTime.UtcNow;
        
        _context.Entry(existing).CurrentValues.SetValues(transaction);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Edit", "Transactions", transaction.TransactionId, existing, transaction);
    }

    public async Task DeleteTransactionAsync(string transactionId)
    {
        var transaction = await GetTransactionByIdAsync(transactionId);
        if (transaction == null)
            throw new ArgumentException($"Transaction with ID {transactionId} not found");

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Delete", "Transactions", transactionId, transaction, null);
    }

    public async Task LinkTransactionToTripAsync(string transactionId, int tripId)
    {
        var transaction = await GetTransactionByIdAsync(transactionId);
        if (transaction == null)
            throw new ArgumentException($"Transaction with ID {transactionId} not found");

        var oldTripId = transaction.TripId;
        transaction.TripId = tripId;
        transaction.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Link", "Transactions", transactionId, 
            new { TripId = oldTripId }, new { TripId = tripId });
    }

    public async Task UnlinkTransactionFromTripAsync(string transactionId)
    {
        var transaction = await GetTransactionByIdAsync(transactionId);
        if (transaction == null)
            throw new ArgumentException($"Transaction with ID {transactionId} not found");

        var oldTripId = transaction.TripId;
        transaction.TripId = null;
        transaction.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Unlink", "Transactions", transactionId,
            new { TripId = oldTripId }, new { TripId = (int?)null });
    }

    public async Task<IEnumerable<Transaction>> SplitTransactionAsync(string transactionId, List<Transaction> splitTransactions)
    {
        var originalTransaction = await GetTransactionByIdAsync(transactionId);
        if (originalTransaction == null)
            throw new ArgumentException($"Transaction with ID {transactionId} not found");

        foreach (var split in splitTransactions)
        {
            split.TransactionId = $"{transactionId}_split_{Guid.NewGuid().ToString("N")[..8]}";
            split.CreatedAt = DateTime.UtcNow;
            split.ModifiedAt = DateTime.UtcNow;
            split.Notes = $"Split from {transactionId}. {split.Notes}";
            
            _context.Transactions.Add(split);
        }

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Split", "Transactions", transactionId, 
            originalTransaction, splitTransactions);

        return splitTransactions;
    }

    public async Task MarkTransactionAsValidAsync(string transactionId)
    {
        var transaction = await GetTransactionByIdAsync(transactionId);
        if (transaction == null)
            throw new ArgumentException($"Transaction with ID {transactionId} not found");

        transaction.IsValid = true;
        transaction.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Validate", "Transactions", transactionId, 
            new { IsValid = false }, new { IsValid = true });
    }

    public async Task<IEnumerable<Transaction>> GetAirfareWithoutCabinClassAsync()
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Airfare" && t.CabinClassId == null)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetHighValueMealsAsync(decimal threshold = 80)
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Meals" && 
                       Math.Abs(t.AmountUSD ?? 0) >= threshold && 
                       !t.IsValid)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetLowValueLodgingAsync(decimal threshold = 100)
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Lodging" && 
                       Math.Abs(t.AmountUSD ?? 0) <= threshold && 
                       !t.IsValid)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetClientEntertainmentWithoutParticipantsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Client entertainment" && 
                       !t.ParticipantsValidated)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetOtherCategoryTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Other")
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsWithoutDocumentationAsync()
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => string.IsNullOrEmpty(t.DocumentUrl))
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> SearchTransactionsAsync(string searchQuery, int pageSize = 50, bool includeAlreadySplit = true)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.CabinClass)
            .AsQueryable();

        if (!includeAlreadySplit)
        {
            query = query.Where(t => !t.IsSplit && string.IsNullOrEmpty(t.OriginalTransactionId));
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var searchTerm = searchQuery.ToLower();
            query = query.Where(t => 
                t.TransactionId.ToLower().Contains(searchTerm) ||
                t.Email.ToLower().Contains(searchTerm) ||
                (t.Vendor != null && t.Vendor.ToLower().Contains(searchTerm)) ||
                (t.Address != null && t.Address.ToLower().Contains(searchTerm)) ||
                (t.Notes != null && t.Notes.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .Take(pageSize)
            .ToListAsync();
    }
}