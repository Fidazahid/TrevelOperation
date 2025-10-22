using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace TrevelOperation.RazorLib.Components;

public partial class DataTable<TItem> : ComponentBase where TItem : class
{
    [Parameter] public IEnumerable<TItem>? Items { get; set; }
    [Parameter] public List<TableColumn> Columns { get; set; } = new();
    [Parameter] public RenderFragment? AdditionalActions { get; set; }
    [Parameter] public EventCallback<TItem> OnRowDoubleClick { get; set; }
    [Parameter] public EventCallback<TItem> OnRowEdit { get; set; }
    [Parameter] public EventCallback<TItem> OnRowDelete { get; set; }
    [Parameter] public EventCallback<List<TItem>> OnExportCsv { get; set; }
    [Parameter] public EventCallback<List<TItem>> OnExportExcel { get; set; }
    [Parameter] public bool ShowPagination { get; set; } = true;
    [Parameter] public int PageSize { get; set; } = 50;
    [Parameter] public string TableId { get; set; } = string.Empty;
    [Parameter] public Func<TItem, string>? RowClassProvider { get; set; }

    private ElementReference tableElement;
    private string SearchTerm = string.Empty;
    private string SortColumn = string.Empty;
    private bool SortAscending = true;
    private int CurrentPage = 1;
    private TItem? EditingRow;
    private bool ShowColumnManager;
    private List<TableView> SavedViews = new();
    private Dictionary<string, int> ColumnWidths = new();
    private bool IsResizing;
    private string ResizingColumn = string.Empty;

    protected List<TableColumn> AllColumns => Columns;
    protected IEnumerable<TableColumn> VisibleColumns => AllColumns.Where(c => c.IsVisible);
    protected IEnumerable<TItem> FilteredItems => GetFilteredAndSortedItems();
    protected int TotalPages => (int)Math.Ceiling((FilteredItems?.Count() ?? 0) / (double)PageSize);

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(TableId))
            TableId = typeof(TItem).Name;

        await LoadSavedViews();
        await LoadColumnWidths();
        
        if (!AllColumns.Any())
        {
            GenerateDefaultColumns();
        }
    }

    private void GenerateDefaultColumns()
    {
        var properties = typeof(TItem).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        int order = 0;

        foreach (var prop in properties)
        {
            if (ShouldIncludeProperty(prop))
            {
                var column = new TableColumn
                {
                    PropertyName = prop.Name,
                    DisplayName = FormatDisplayName(prop.Name),
                    DataType = GetDataType(prop.PropertyType),
                    IsVisible = true,
                    IsEditable = IsEditableProperty(prop),
                    Order = order++,
                    Width = GetDefaultWidth(prop.PropertyType)
                };

                AllColumns.Add(column);
            }
        }
    }

    private bool ShouldIncludeProperty(PropertyInfo prop)
    {
        var excludedTypes = new[] { typeof(byte[]), typeof(Stream) };
        return !excludedTypes.Contains(prop.PropertyType) && 
               !prop.PropertyType.IsClass || 
               prop.PropertyType == typeof(string);
    }

    private string FormatDisplayName(string propertyName)
    {
        return System.Text.RegularExpressions.Regex.Replace(propertyName, "([a-z])([A-Z])", "$1 $2");
    }

    private TableDataType GetDataType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        
        if (underlyingType == typeof(DateTime))
            return TableDataType.Date;
        if (underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float))
            return TableDataType.Currency;
        if (underlyingType == typeof(int) || underlyingType == typeof(long))
            return TableDataType.Number;
        if (underlyingType == typeof(bool))
            return TableDataType.Boolean;
        
        return TableDataType.Text;
    }

    private bool IsEditableProperty(PropertyInfo prop)
    {
        return prop.CanWrite && 
               prop.SetMethod?.IsPublic == true &&
               !prop.Name.EndsWith("Id") &&
               prop.Name != "CreatedAt" &&
               prop.Name != "ModifiedAt";
    }

    private int GetDefaultWidth(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        
        if (underlyingType == typeof(DateTime))
            return 120;
        if (underlyingType == typeof(decimal) || underlyingType == typeof(double))
            return 100;
        if (underlyingType == typeof(bool))
            return 80;
        if (underlyingType == typeof(int))
            return 80;
        
        return 150;
    }

    private IEnumerable<TItem> GetFilteredAndSortedItems()
    {
        if (Items == null)
            return Enumerable.Empty<TItem>();

        var filtered = Items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            filtered = filtered.Where(item => MatchesSearchTerm(item, SearchTerm));
        }

        if (!string.IsNullOrEmpty(SortColumn))
        {
            filtered = ApplySort(filtered, SortColumn, SortAscending);
        }

        if (ShowPagination)
        {
            filtered = filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }

        return filtered;
    }

    private bool MatchesSearchTerm(TItem item, string searchTerm)
    {
        var searchLower = searchTerm.ToLowerInvariant();
        var properties = typeof(TItem).GetProperties();

        return properties.Any(prop =>
        {
            try
            {
                var value = prop.GetValue(item)?.ToString();
                return !string.IsNullOrEmpty(value) && value.ToLowerInvariant().Contains(searchLower);
            }
            catch
            {
                return false;
            }
        });
    }

    private IEnumerable<TItem> ApplySort(IEnumerable<TItem> items, string propertyName, bool ascending)
    {
        try
        {
            var property = typeof(TItem).GetProperty(propertyName);
            if (property == null)
                return items;

            var parameter = Expression.Parameter(typeof(TItem), "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(TItem), property.PropertyType },
                items.AsQueryable().Expression,
                Expression.Quote(orderByExpression));

            return items.AsQueryable().Provider.CreateQuery<TItem>(resultExpression);
        }
        catch
        {
            return items;
        }
    }

    private void SortBy(string columnName)
    {
        if (SortColumn == columnName)
        {
            SortAscending = !SortAscending;
        }
        else
        {
            SortColumn = columnName;
            SortAscending = true;
        }

        CurrentPage = 1;
        StateHasChanged();
    }

    private void OnSearchChanged()
    {
        CurrentPage = 1;
        StateHasChanged();
    }

    private void GoToPage(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            StateHasChanged();
        }
    }

    private string GetHeaderClass(TableColumn column)
    {
        var classes = new List<string> { "sortable-header" };
        
        if (SortColumn == column.PropertyName)
            classes.Add("sorted");

        return string.Join(" ", classes);
    }

    private string GetColumnStyle(TableColumn column)
    {
        var width = ColumnWidths.ContainsKey(column.PropertyName) 
            ? ColumnWidths[column.PropertyName] 
            : column.Width;

        return $"width: {width}px; min-width: {Math.Max(50, width)}px;";
    }

    private string GetRowClass(TItem item)
    {
        var classes = new List<string>();

        if (EditingRow == item)
            classes.Add("row-editing");

        if (RowClassProvider != null)
            classes.Add(RowClassProvider(item));

        return string.Join(" ", classes);
    }

    private string GetCellClass(TableColumn column, TItem item)
    {
        var classes = new List<string>();

        if (column.IsEditable && EditingRow == item)
            classes.Add("editable-cell");

        switch (column.DataType)
        {
            case TableDataType.Currency:
                classes.Add("monetary-cell");
                break;
            case TableDataType.Date:
                classes.Add("date-cell");
                break;
            case TableDataType.Boolean:
                classes.Add("status-cell");
                break;
        }

        return string.Join(" ", classes);
    }

    private RenderFragment RenderCell(TableColumn column, TItem item)
    {
        return builder =>
        {
            var value = GetPropertyValue(item, column.PropertyName);
            var formattedValue = FormatCellValue(value, column.DataType);
            
            builder.AddContent(0, formattedValue);
        };
    }

    private RenderFragment RenderEditableCell(TableColumn column, TItem item)
    {
        return builder =>
        {
            var value = GetPropertyValue(item, column.PropertyName);
            
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "type", GetInputType(column.DataType));
            builder.AddAttribute(2, "class", "form-control form-control-sm");
            builder.AddAttribute(3, "value", FormatInputValue(value, column.DataType));
            builder.AddAttribute(4, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, 
                args => UpdatePropertyValue(item, column.PropertyName, args.Value)));
            builder.CloseElement();
        };
    }

    private object? GetPropertyValue(TItem item, string propertyName)
    {
        try
        {
            var property = typeof(TItem).GetProperty(propertyName);
            return property?.GetValue(item);
        }
        catch
        {
            return null;
        }
    }

    private void UpdatePropertyValue(TItem item, string propertyName, object? value)
    {
        try
        {
            var property = typeof(TItem).GetProperty(propertyName);
            if (property?.CanWrite == true)
            {
                var convertedValue = ConvertValue(value, property.PropertyType);
                property.SetValue(item, convertedValue);
                OnRowEdit.InvokeAsync(item);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating property {propertyName}: {ex.Message}");
        }
    }

    private object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
            return null;

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        
        if (underlyingType == typeof(string))
            return value.ToString();
        
        if (underlyingType == typeof(DateTime))
            return DateTime.TryParse(value.ToString(), out var date) ? date : null;
        
        if (underlyingType == typeof(decimal))
            return decimal.TryParse(value.ToString(), out var dec) ? dec : 0m;
        
        if (underlyingType == typeof(int))
            return int.TryParse(value.ToString(), out var intVal) ? intVal : 0;
        
        if (underlyingType == typeof(bool))
            return bool.TryParse(value.ToString(), out var boolVal) ? boolVal : false;

        return Convert.ChangeType(value, underlyingType);
    }

    private string FormatCellValue(object? value, TableDataType dataType)
    {
        if (value == null)
            return string.Empty;

        return dataType switch
        {
            TableDataType.Date => value is DateTime dt ? dt.ToString("dd/MM/yyyy") : value.ToString() ?? "",
            TableDataType.Currency => value is decimal dec ? dec.ToString("N2") : 
                                     value is double dbl ? dbl.ToString("N2") : 
                                     value.ToString() ?? "",
            TableDataType.Boolean => value is bool b ? (b ? "Yes" : "No") : value.ToString() ?? "",
            _ => value.ToString() ?? ""
        };
    }

    private string FormatInputValue(object? value, TableDataType dataType)
    {
        if (value == null)
            return string.Empty;

        return dataType switch
        {
            TableDataType.Date => value is DateTime dt ? dt.ToString("yyyy-MM-dd") : "",
            _ => value.ToString() ?? ""
        };
    }

    private string GetInputType(TableDataType dataType)
    {
        return dataType switch
        {
            TableDataType.Date => "date",
            TableDataType.Number => "number",
            TableDataType.Currency => "number",
            _ => "text"
        };
    }

    private async Task ExportToCsv()
    {
        if (OnExportCsv.HasDelegate)
        {
            await OnExportCsv.InvokeAsync(FilteredItems.ToList());
        }
    }

    private async Task ExportToExcel()
    {
        if (OnExportExcel.HasDelegate)
        {
            await OnExportExcel.InvokeAsync(FilteredItems.ToList());
        }
    }

    private void ToggleColumnManager()
    {
        ShowColumnManager = true;
        StateHasChanged();
    }

    private void HideColumnManager()
    {
        ShowColumnManager = false;
        StateHasChanged();
    }

    private void ApplyColumnChanges()
    {
        SaveColumnWidths();
        HideColumnManager();
        StateHasChanged();
    }

    private void ResetColumns()
    {
        foreach (var column in AllColumns)
        {
            column.IsVisible = true;
            column.Width = GetDefaultWidth(typeof(string)); // Default width
            column.Order = AllColumns.IndexOf(column);
        }
        
        ColumnWidths.Clear();
        StateHasChanged();
    }

    private async Task SaveView()
    {
        var viewName = $"View {DateTime.Now:yyyy-MM-dd HH:mm}";
        var view = new TableView
        {
            Name = viewName,
            TableId = TableId,
            ColumnSettings = AllColumns.Select(c => new ColumnSetting
            {
                PropertyName = c.PropertyName,
                IsVisible = c.IsVisible,
                Width = c.Width,
                Order = c.Order
            }).ToList()
        };

        SavedViews.Add(view);
        await SaveViewsToStorage();
        StateHasChanged();
    }

    private async Task LoadView(TableView view)
    {
        foreach (var setting in view.ColumnSettings)
        {
            var column = AllColumns.FirstOrDefault(c => c.PropertyName == setting.PropertyName);
            if (column != null)
            {
                column.IsVisible = setting.IsVisible;
                column.Width = setting.Width;
                column.Order = setting.Order;
            }
        }

        HideColumnManager();
        StateHasChanged();
    }

    private async Task DeleteView(TableView view)
    {
        SavedViews.Remove(view);
        await SaveViewsToStorage();
        StateHasChanged();
    }

    private async Task LoadSavedViews()
    {
        try
        {
            var json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", $"table_views_{TableId}");
            if (!string.IsNullOrEmpty(json))
            {
                SavedViews = JsonSerializer.Deserialize<List<TableView>>(json) ?? new List<TableView>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading saved views: {ex.Message}");
        }
    }

    private async Task SaveViewsToStorage()
    {
        try
        {
            var json = JsonSerializer.Serialize(SavedViews);
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", $"table_views_{TableId}", json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving views: {ex.Message}");
        }
    }

    private async Task LoadColumnWidths()
    {
        try
        {
            var json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", $"column_widths_{TableId}");
            if (!string.IsNullOrEmpty(json))
            {
                ColumnWidths = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading column widths: {ex.Message}");
        }
    }

    private async Task SaveColumnWidths()
    {
        try
        {
            var json = JsonSerializer.Serialize(ColumnWidths);
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", $"column_widths_{TableId}", json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving column widths: {ex.Message}");
        }
    }

    private async Task StartResize(Microsoft.AspNetCore.Components.Web.MouseEventArgs e, string columnName)
    {
        IsResizing = true;
        ResizingColumn = columnName;
        
        await JSRuntime.InvokeVoidAsync("dataTable.startResize", tableElement, columnName, DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public void UpdateColumnWidth(string columnName, int newWidth)
    {
        ColumnWidths[columnName] = Math.Max(50, newWidth);
        StateHasChanged();
    }

    [JSInvokable]
    public void EndResize()
    {
        IsResizing = false;
        ResizingColumn = string.Empty;
        SaveColumnWidths();
    }

    private async Task HandleRowDoubleClick(TItem item)
    {
        if (OnRowDoubleClick.HasDelegate)
        {
            await OnRowDoubleClick.InvokeAsync(item);
        }
    }
}

public class TableColumn
{
    public string PropertyName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public TableDataType DataType { get; set; } = TableDataType.Text;
    public bool IsVisible { get; set; } = true;
    public bool IsEditable { get; set; } = false;
    public int Width { get; set; } = 150;
    public int Order { get; set; } = 0;
}

public class TableView
{
    public string Name { get; set; } = string.Empty;
    public string TableId { get; set; } = string.Empty;
    public List<ColumnSetting> ColumnSettings { get; set; } = new();
}

public class ColumnSetting
{
    public string PropertyName { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int Width { get; set; } = 150;
    public int Order { get; set; } = 0;
}

public enum TableDataType
{
    Text,
    Number,
    Currency,
    Date,
    Boolean
}