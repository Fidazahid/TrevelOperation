using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Text.Json;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;

namespace TrevelOperation.RazorLib.Pages;

public partial class AuditLog
{
    private Dictionary<string, int>? stats;
    private IEnumerable<TravelOperation.Core.Models.Entities.AuditLog>? auditLogs;
    private IEnumerable<TravelOperation.Core.Models.Entities.AuditLog> pagedAuditLogs => auditLogs?.Skip((currentPage - 1) * pageSize).Take(pageSize) ?? Enumerable.Empty<TravelOperation.Core.Models.Entities.AuditLog>();
    private IEnumerable<string>? distinctActions;
    private IEnumerable<string>? distinctTables;
    private IEnumerable<string>? distinctUsers;
    
    private bool loading = true;
    private bool showDetailsModal = false;
    private TravelOperation.Core.Models.Entities.AuditLog? selectedLog;
    
    private string searchTerm = string.Empty;
    private string selectedAction = string.Empty;
    private string selectedTable = string.Empty;
    private string selectedUser = string.Empty;
    private DateTime? startDate;
    private DateTime? endDate;
    
    private int currentPage = 1;
    private int pageSize = 20;
    private int totalPages => auditLogs?.Count() > 0 ? (int)Math.Ceiling((double)auditLogs.Count() / pageSize) : 1;
    
    private string sortField = "Timestamp";
    private bool sortAscending = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        loading = true;
        try
        {
            stats = await AuditService.GetAuditStatsAsync();
            auditLogs = await AuditService.GetAllAuditLogsAsync();
            distinctActions = await AuditService.GetDistinctActionsAsync();
            distinctTables = await AuditService.GetDistinctTablesAsync();
            distinctUsers = await AuditService.GetDistinctUsersAsync();
            
            ApplySorting();
        }
        finally
        {
            loading = false;
        }
    }

    private async Task RefreshData()
    {
        await LoadData();
    }

    private async Task ApplyFilters()
    {
        loading = true;
        currentPage = 1;
        
        try
        {
            auditLogs = await AuditService.SearchAuditLogsAsync(
                searchTerm: string.IsNullOrEmpty(searchTerm) ? null : searchTerm,
                action: string.IsNullOrEmpty(selectedAction) ? null : selectedAction,
                linkedTable: string.IsNullOrEmpty(selectedTable) ? null : selectedTable,
                userId: string.IsNullOrEmpty(selectedUser) ? null : selectedUser,
                startDate: startDate,
                endDate: endDate
            );
            
            ApplySorting();
        }
        finally
        {
            loading = false;
        }
    }

    private async Task OnSearchKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await ApplyFilters();
        }
    }

    private void SortBy(string field)
    {
        if (sortField == field)
        {
            sortAscending = !sortAscending;
        }
        else
        {
            sortField = field;
            sortAscending = true;
        }
        
        ApplySorting();
        currentPage = 1;
    }

    private void ApplySorting()
    {
        if (auditLogs == null) return;

        auditLogs = sortField switch
        {
            "Timestamp" => sortAscending 
                ? auditLogs.OrderBy(x => x.Timestamp) 
                : auditLogs.OrderByDescending(x => x.Timestamp),
            "UserId" => sortAscending 
                ? auditLogs.OrderBy(x => x.UserId) 
                : auditLogs.OrderByDescending(x => x.UserId),
            "Action" => sortAscending 
                ? auditLogs.OrderBy(x => x.Action) 
                : auditLogs.OrderByDescending(x => x.Action),
            "LinkedTable" => sortAscending 
                ? auditLogs.OrderBy(x => x.LinkedTable) 
                : auditLogs.OrderByDescending(x => x.LinkedTable),
            _ => auditLogs.OrderByDescending(x => x.Timestamp)
        };
    }

    private string GetSortIcon(string field)
    {
        if (sortField != field) return "";
        return sortAscending ? " ↑" : " ↓";
    }

    private void ChangePage(int page)
    {
        if (page >= 1 && page <= totalPages)
        {
            currentPage = page;
        }
    }

    private string GetActionBadgeClass(string action)
    {
        return action switch
        {
            "Added" or "Create" => "badge-success",
            "Modified" or "Edit" => "badge-warning",
            "Deleted" or "Delete" => "badge-error",
            "Restore" => "badge-info",
            _ => "badge-neutral"
        };
    }

    private void ViewDetails(TravelOperation.Core.Models.Entities.AuditLog log)
    {
        selectedLog = log;
        showDetailsModal = true;
    }

    private void CloseDetailsModal()
    {
        showDetailsModal = false;
        selectedLog = null;
    }

    private async Task RestoreRecord(TravelOperation.Core.Models.Entities.AuditLog log)
    {
        try
        {
            var canRestore = await AuditService.CanRestoreAsync(log.LinkedTable, log.LinkedRecordId);
            
            if (!canRestore)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Cannot restore: Record was involved in a split operation.");
                return;
            }

            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
                $"Are you sure you want to restore {log.LinkedTable} record {log.LinkedRecordId} to the state from {log.Timestamp:dd/MM/yyyy HH:mm:ss}?");
            
            if (confirmed)
            {
                await AuditService.RestoreFromAuditAsync(log.AuditId);
                CloseDetailsModal();
                await RefreshData();
                await JSRuntime.InvokeVoidAsync("alert", "Record restored successfully!");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error restoring record: {ex.Message}");
        }
    }

    private async Task ExportToCsv()
    {
        try
        {
            if (auditLogs == null || !auditLogs.Any())
            {
                await JSRuntime.InvokeVoidAsync("alert", "No data to export.");
                return;
            }

            var csvContent = GenerateCsvContent();
            var fileName = $"audit_log_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, csvContent, "text/csv");
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error exporting data: {ex.Message}");
        }
    }

    private string GenerateCsvContent()
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("AuditId,Timestamp,UserId,Action,LinkedTable,LinkedRecordId,Comments");
        
        foreach (var log in auditLogs!)
        {
            csv.AppendLine($"{log.AuditId},{log.Timestamp:dd/MM/yyyy HH:mm:ss},\"{log.UserId}\",\"{log.Action}\",\"{log.LinkedTable}\",\"{log.LinkedRecordId}\",\"{log.Comments?.Replace("\"", "\"\"")}\"");
        }
        
        return csv.ToString();
    }

    private string FormatJson(string json)
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch
        {
            return json;
        }
    }
}