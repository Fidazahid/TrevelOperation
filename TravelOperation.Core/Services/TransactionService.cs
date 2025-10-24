using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Extensions;
using TravelOperation.Core.Models;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Services;

public class TransactionService : ITransactionService
{
    private readonly TravelDbContext _context;
    private readonly IAuditService _auditService;
    private readonly IAuthenticationService _authService;

    public TransactionService(
        TravelDbContext context, 
        IAuditService auditService, 
        IAuthenticationService authService)
    {
        _context = context;
        _auditService = auditService;
        _authService = authService;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.Trip)
            .Include(t => t.CabinClass)
            .Include(t => t.BookingStatus)
            .Include(t => t.BookingType)
            .AsQueryable();

        // Apply role-based filtering
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser != null)
        {
            if (currentUser.Role == "Employee")
            {
                // Employees see only their own transactions
                query = query.Where(t => t.Email == currentUser.Email);
            }
            else if (currentUser.Role == "Owner")
            {
                // Owners see transactions for their department
                // This requires matching email domain or department lookup
                // For now, we'll filter by department from user records
                var departmentEmails = await GetDepartmentEmailsAsync(currentUser.Department);
                query = query.Where(t => departmentEmails.Contains(t.Email));
            }
            // Finance users see all transactions (no filter)
        }

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    // Paginated version with role-based filtering
    public async Task<PagedResult<Transaction>> GetAllTransactionsPagedAsync(PaginationParams pagination)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.Trip)
            .Include(t => t.CabinClass)
            .Include(t => t.BookingStatus)
            .Include(t => t.BookingType)
            .AsQueryable();

        // Apply role-based filtering
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser != null)
        {
            if (currentUser.Role == "Employee")
            {
                query = query.Where(t => t.Email == currentUser.Email);
            }
            else if (currentUser.Role == "Owner")
            {
                var departmentEmails = await GetDepartmentEmailsAsync(currentUser.Department);
                query = query.Where(t => departmentEmails.Contains(t.Email));
            }
        }

        query = query.OrderByDescending(t => t.TransactionDate);
        
        return await query.ToPagedResultAsync(pagination);
    }

    public async Task<PagedResult<Transaction>> GetTransactionsByEmailPagedAsync(string email, PaginationParams pagination)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.Trip)
            .Where(t => t.Email == email)
            .OrderByDescending(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
    }

    public async Task<PagedResult<Transaction>> GetUnlinkedTransactionsPagedAsync(PaginationParams pagination)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.TripId == null)
            .OrderByDescending(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
    }

    private async Task<List<string>> GetDepartmentEmailsAsync(string department)
    {
        // For now, just return empty list to avoid circular dependency
        // In production, this would query a separate users/headcount table
        // TODO: Implement proper department email lookup without circular dependency
        await Task.CompletedTask;
        return new List<string>();
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

    public async Task<Transaction?> GetByIdAsync(string transactionId)
    {
        return await GetTransactionByIdAsync(transactionId);
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

    public async Task<IEnumerable<Transaction>> GetTransactionsByEmailAndDateRangeAsync(string email, DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.Trip)
            .Include(t => t.CabinClass)
            .Include(t => t.BookingStatus)
            .Include(t => t.BookingType)
            .Where(t => t.Email == email && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .OrderBy(t => t.TransactionDate)
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
        // Validate transaction before creating
        ValidateTransaction(transaction);
        
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

        // Validate transaction before updating
        ValidateTransaction(transaction);
        
        transaction.ModifiedAt = DateTime.UtcNow;
        
        _context.Entry(existing).CurrentValues.SetValues(transaction);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Edit", "Transactions", transaction.TransactionId, existing, transaction);
    }

    /// <summary>
    /// Validates a transaction according to business rules
    /// </summary>
    /// <param name="transaction">The transaction to validate</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    private void ValidateTransaction(Transaction transaction)
    {
        var errors = new List<string>();

        // Amount must be numeric (C# type system enforces this, but check for valid range)
        if (transaction.Amount == 0)
        {
            errors.Add("Amount must not be zero.");
        }

        // Transaction date must be valid
        if (transaction.TransactionDate == default(DateTime))
        {
            errors.Add("Transaction date is required.");
        }

        if (transaction.TransactionDate > DateTime.Now.AddDays(1))
        {
            errors.Add("Transaction date cannot be in the future.");
        }

        // Email must be valid format
        if (string.IsNullOrWhiteSpace(transaction.Email))
        {
            errors.Add("Email is required.");
        }
        else if (!IsValidEmail(transaction.Email))
        {
            errors.Add("Email format is invalid.");
        }

        // Currency must be 3-letter code
        if (string.IsNullOrWhiteSpace(transaction.Currency))
        {
            errors.Add("Currency is required.");
        }
        else if (transaction.Currency.Length != 3)
        {
            errors.Add("Currency must be a 3-letter code (e.g., USD, EUR, ILS).");
        }

        // Document URL must be valid URL or empty
        if (!string.IsNullOrWhiteSpace(transaction.DocumentUrl))
        {
            if (!IsValidUrl(transaction.DocumentUrl))
            {
                errors.Add("Document URL format is invalid.");
            }
        }

        // Exchange rate should be positive if provided
        if (transaction.ExchangeRate.HasValue && transaction.ExchangeRate.Value <= 0)
        {
            errors.Add("Exchange rate must be a positive number.");
        }

        // If AmountUSD is provided, it should match Amount * ExchangeRate
        if (transaction.AmountUSD.HasValue && transaction.ExchangeRate.HasValue)
        {
            var calculatedUSD = transaction.Amount * transaction.ExchangeRate.Value;
            var difference = Math.Abs(transaction.AmountUSD.Value - calculatedUSD);
            
            // Allow for small rounding errors (within 0.01)
            if (difference > 0.01m)
            {
                errors.Add($"Amount USD ({transaction.AmountUSD:F2}) does not match Amount × Exchange Rate ({calculatedUSD:F2}).");
            }
        }

        if (errors.Any())
        {
            throw new ArgumentException($"Transaction validation failed:\n{string.Join("\n", errors)}");
        }
    }

    /// <summary>
    /// Validates email format
    /// </summary>
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates URL format
    /// </summary>
    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
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

    public async Task<PagedResult<Transaction>> GetAirfareWithoutCabinClassPagedAsync(PaginationParams pagination)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Include(t => t.CabinClass)
            .Where(t => t.Category.Name == "Airfare" && t.CabinClassId == null)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
    }

    public async Task<IEnumerable<Transaction>> GetHighValueMealsAsync(decimal threshold = 80)
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Meals" && 
                       ((t.AmountUSD ?? 0) >= threshold || (t.AmountUSD ?? 0) <= -threshold) &&
                       !t.IsValid)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<PagedResult<Transaction>> GetHighValueMealsPagedAsync(PaginationParams pagination, decimal threshold = 80)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Meals" && 
                       ((t.AmountUSD ?? 0) >= threshold || (t.AmountUSD ?? 0) <= -threshold) &&
                       !t.IsValid)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
    }

    public async Task<IEnumerable<Transaction>> GetLowValueLodgingAsync(decimal threshold = 100)
    {
        return await _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Lodging" && 
                       (t.AmountUSD ?? 0) <= threshold && 
                       (t.AmountUSD ?? 0) >= -threshold &&
                       !t.IsValid)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<PagedResult<Transaction>> GetLowValueLodgingPagedAsync(PaginationParams pagination, decimal threshold = 100)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Lodging" && 
                       (t.AmountUSD ?? 0) <= threshold && 
                       (t.AmountUSD ?? 0) >= -threshold &&
                       !t.IsValid)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
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

    public async Task<PagedResult<Transaction>> GetClientEntertainmentWithoutParticipantsPagedAsync(PaginationParams pagination)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Client entertainment" && 
                       !t.ParticipantsValidated)
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
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

    public async Task<PagedResult<Transaction>> GetOtherCategoryTransactionsPagedAsync(PaginationParams pagination)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => t.Category.Name == "Other")
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
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

    public async Task<PagedResult<Transaction>> GetTransactionsWithoutDocumentationPagedAsync(PaginationParams pagination)
    {
        var query = _context.Transactions
            .Include(t => t.Source)
            .Include(t => t.Category)
            .Where(t => string.IsNullOrEmpty(t.DocumentUrl))
            .OrderBy(t => t.Email)
            .ThenBy(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
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

    public async Task<PagedResult<Transaction>> SearchTransactionsPagedAsync(string searchQuery, PaginationParams pagination, bool includeAlreadySplit = true)
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

        query = query.OrderByDescending(t => t.TransactionDate);

        return await query.ToPagedResultAsync(pagination);
    }

    public async Task<List<Transaction>> GetAllTransactionsAsync(bool includeRelated = false)
    {
        var query = _context.Transactions.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(t => t.Source)
                .Include(t => t.Category)
                .Include(t => t.Trip)
                .Include(t => t.CabinClass)
                .Include(t => t.BookingStatus)
                .Include(t => t.BookingType);
        }

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task MarkAsValidAsync(string transactionId)
    {
        await MarkTransactionAsValidAsync(transactionId);
    }

    public async Task DeleteAsync(string transactionId)
    {
        await DeleteTransactionAsync(transactionId);
    }

    public async Task UnlinkFromTripAsync(string transactionId)
    {
        await UnlinkTransactionFromTripAsync(transactionId);
    }

    public async Task UpdateTransactionAsync(object updateDto)
    {
        // Use reflection to get properties from the DTO
        var properties = updateDto.GetType().GetProperties();
        var transactionIdProperty = properties.FirstOrDefault(p => p.Name == "TransactionId");
        
        if (transactionIdProperty?.GetValue(updateDto) is not string transactionId)
            throw new ArgumentException("TransactionId is required in update DTO");

        var transaction = await GetTransactionByIdAsync(transactionId);
        if (transaction == null)
            throw new ArgumentException($"Transaction with ID {transactionId} not found");

        var oldTransaction = new Transaction
        {
            TransactionId = transaction.TransactionId,
            CategoryId = transaction.CategoryId,
            CabinClassId = transaction.CabinClassId,
            Participants = transaction.Participants,
            Notes = transaction.Notes,
            IsValid = transaction.IsValid,
            DataValidation = transaction.DataValidation
        };

        // Update properties from DTO
        foreach (var property in properties)
        {
            if (property.Name == "TransactionId") continue;

            var transactionProperty = typeof(Transaction).GetProperty(property.Name);
            if (transactionProperty != null && transactionProperty.CanWrite)
            {
                var value = property.GetValue(updateDto);
                transactionProperty.SetValue(transaction, value);
            }
        }

        transaction.ModifiedAt = DateTime.UtcNow;
        transaction.ModifiedBy = "System"; // TODO: Get current user

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync("System", "Edit", "Transactions", transactionId, oldTransaction, transaction);
    }
}