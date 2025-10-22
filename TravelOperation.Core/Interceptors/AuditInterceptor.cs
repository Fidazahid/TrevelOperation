using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TravelOperation.Core.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<AuditEntry> _auditEntries = new();

    public AuditInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        OnBeforeSaving(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        OnBeforeSaving(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        OnAfterSaving();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await OnAfterSavingAsync();
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void OnBeforeSaving(DbContext? context)
    {
        if (context == null) return;

        _auditEntries.Clear();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry
            {
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                EntityId = GetEntityId(entry),
                OldValues = GetOldValues(entry),
                NewValues = GetNewValues(entry)
            };

            _auditEntries.Add(auditEntry);
        }
    }

    private void OnAfterSaving()
    {
        if (_auditEntries.Count == 0) return;

        using var scope = _serviceProvider.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

        foreach (var auditEntry in _auditEntries)
        {
            auditService.LogActionAsync(
                "System", 
                auditEntry.Action, 
                auditEntry.EntityName, 
                auditEntry.EntityId,
                auditEntry.OldValues,
                auditEntry.NewValues
            ).GetAwaiter().GetResult();
        }
    }

    private async Task OnAfterSavingAsync()
    {
        if (_auditEntries.Count == 0) return;

        using var scope = _serviceProvider.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

        foreach (var auditEntry in _auditEntries)
        {
            await auditService.LogActionAsync(
                "System", 
                auditEntry.Action, 
                auditEntry.EntityName, 
                auditEntry.EntityId,
                auditEntry.OldValues,
                auditEntry.NewValues
            );
        }
    }

    private string GetEntityId(EntityEntry entry)
    {
        var keyProperties = entry.Properties
            .Where(p => p.Metadata.IsPrimaryKey())
            .ToList();

        if (keyProperties.Count == 1)
        {
            return keyProperties[0].CurrentValue?.ToString() ?? "";
        }

        var compositeKey = keyProperties.ToDictionary(
            p => p.Metadata.Name,
            p => p.CurrentValue?.ToString() ?? ""
        );

        return JsonSerializer.Serialize(compositeKey);
    }

    private object? GetOldValues(EntityEntry entry)
    {
        if (entry.State == EntityState.Added) return null;

        var oldValues = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            if (property.IsModified || entry.State == EntityState.Deleted)
            {
                oldValues[property.Metadata.Name] = property.OriginalValue;
            }
        }

        return oldValues.Count > 0 ? oldValues : null;
    }

    private object? GetNewValues(EntityEntry entry)
    {
        if (entry.State == EntityState.Deleted) return null;

        var newValues = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            if (entry.State == EntityState.Added || property.IsModified)
            {
                newValues[property.Metadata.Name] = property.CurrentValue;
            }
        }

        return newValues.Count > 0 ? newValues : null;
    }

    private class AuditEntry
    {
        public string EntityName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public object? OldValues { get; set; }
        public object? NewValues { get; set; }
    }
}