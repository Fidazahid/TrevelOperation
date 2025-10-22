using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services.Interfaces;

public interface IAuditService
{
    Task LogActionAsync(string userId, string action, string linkedTable, string linkedRecordId, object? oldValue = null, object? newValue = null, string? comments = null);
    Task<IEnumerable<AuditLog>> GetAuditHistoryAsync(string linkedTable, string linkedRecordId);
    Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(string userId);
    Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync();
    Task<IEnumerable<AuditLog>> SearchAuditLogsAsync(string? searchTerm = null, string? action = null, string? linkedTable = null, string? userId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> CanRestoreAsync(string linkedTable, string linkedRecordId);
    Task RestoreFromAuditAsync(int auditId);
    Task<Dictionary<string, int>> GetAuditStatsAsync();
    Task<IEnumerable<string>> GetDistinctActionsAsync();
    Task<IEnumerable<string>> GetDistinctTablesAsync();
    Task<IEnumerable<string>> GetDistinctUsersAsync();
}