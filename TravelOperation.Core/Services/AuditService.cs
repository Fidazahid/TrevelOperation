using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TravelOperation.Core.Data;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Services;

public class AuditService : IAuditService
{
    private readonly TravelDbContext _context;

    public AuditService(TravelDbContext context)
    {
        _context = context;
    }

    public async Task LogActionAsync(string userId, string action, string linkedTable, string linkedRecordId, 
        object? oldValue = null, object? newValue = null, string? comments = null)
    {
        var auditLog = new AuditLog
        {
            Timestamp = DateTime.UtcNow,
            UserId = userId,
            Action = action,
            LinkedTable = linkedTable,
            LinkedRecordId = linkedRecordId,
            OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
            NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null,
            Comments = comments
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAuditHistoryAsync(string linkedTable, string linkedRecordId)
    {
        return await _context.AuditLogs
            .Where(a => a.LinkedTable == linkedTable && a.LinkedRecordId == linkedRecordId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(string userId)
    {
        return await _context.AuditLogs
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.AuditLogs
            .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync()
    {
        return await _context.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> SearchAuditLogsAsync(string? searchTerm = null, string? action = null, 
        string? linkedTable = null, string? userId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(a => a.Comments!.Contains(searchTerm) || 
                                   a.LinkedRecordId.Contains(searchTerm) ||
                                   a.UserId.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (!string.IsNullOrEmpty(linkedTable))
        {
            query = query.Where(a => a.LinkedTable == linkedTable);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(a => a.UserId == userId);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= endDate.Value);
        }

        return await query.OrderByDescending(a => a.Timestamp).ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetAuditStatsAsync()
    {
        var stats = new Dictionary<string, int>();

        stats["TotalEntries"] = await _context.AuditLogs.CountAsync();
        stats["TodayEntries"] = await _context.AuditLogs
            .Where(a => a.Timestamp.Date == DateTime.Today)
            .CountAsync();
        stats["WeekEntries"] = await _context.AuditLogs
            .Where(a => a.Timestamp >= DateTime.Today.AddDays(-7))
            .CountAsync();
        stats["MonthEntries"] = await _context.AuditLogs
            .Where(a => a.Timestamp >= DateTime.Today.AddDays(-30))
            .CountAsync();

        return stats;
    }

    public async Task<IEnumerable<string>> GetDistinctActionsAsync()
    {
        return await _context.AuditLogs
            .Select(a => a.Action)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetDistinctTablesAsync()
    {
        return await _context.AuditLogs
            .Select(a => a.LinkedTable)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetDistinctUsersAsync()
    {
        return await _context.AuditLogs
            .Select(a => a.UserId)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    public async Task<bool> CanRestoreAsync(string linkedTable, string linkedRecordId)
    {
        var lastSplit = await _context.AuditLogs
            .Where(a => a.LinkedTable == linkedTable && a.LinkedRecordId == linkedRecordId && a.Action == "Split")
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();

        return lastSplit == null;
    }

    public async Task RestoreFromAuditAsync(int auditId)
    {
        var auditLog = await _context.AuditLogs.FindAsync(auditId);
        if (auditLog?.OldValue == null)
            throw new ArgumentException("Cannot restore: no old value found");

        if (!await CanRestoreAsync(auditLog.LinkedTable, auditLog.LinkedRecordId))
            throw new InvalidOperationException("Cannot restore: transaction was split");

        switch (auditLog.LinkedTable.ToLower())
        {
            case "transactions":
                var transactionData = JsonSerializer.Deserialize<Transaction>(auditLog.OldValue);
                if (transactionData != null)
                {
                    var existingTransaction = await _context.Transactions.FindAsync(auditLog.LinkedRecordId);
                    if (existingTransaction != null)
                    {
                        _context.Entry(existingTransaction).CurrentValues.SetValues(transactionData);
                        await _context.SaveChangesAsync();
                        
                        await LogActionAsync("System", "Restore", "Transactions", auditLog.LinkedRecordId,
                            null, transactionData, $"Restored from audit ID {auditId}");
                    }
                }
                break;

            case "trips":
                var tripData = JsonSerializer.Deserialize<Trip>(auditLog.OldValue);
                if (tripData != null)
                {
                    var existingTrip = await _context.Trips.FindAsync(int.Parse(auditLog.LinkedRecordId));
                    if (existingTrip != null)
                    {
                        _context.Entry(existingTrip).CurrentValues.SetValues(tripData);
                        await _context.SaveChangesAsync();
                        
                        await LogActionAsync("System", "Restore", "Trips", auditLog.LinkedRecordId,
                            null, tripData, $"Restored from audit ID {auditId}");
                    }
                }
                break;
        }
    }
}