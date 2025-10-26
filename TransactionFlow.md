# Transaction Flow - Component Documentation

## Overview
The Transactions page is a comprehensive interface for managing travel expense transactions. It provides viewing, filtering, sorting, editing, and bulk operations on transaction data with integrated validation and reporting features.

---

## Architecture

### Page Route
- **URL**: `/transactions`
- **Component**: `Transactions.razor`
- **Location**: `TrevelOperation.RazorLib/Pages/Transactions.razor`

---

## Component Breakdown

### 1. **Page Header Section**
```html
<div class="flex justify-between items-center">
```

**Purpose**: Displays page title and action buttons

**Components**:
- **Page Title**: "💳 Transactions"
- **Description**: "View, edit, and manage travel expense transactions"
- **Trip Filter Alert** (conditional): Shows when filtering by specific Trip ID
- **Action Buttons**:
  - 📥 Import CSV - Navigate to CSV import page
  - 📤 Export - Export transactions (coming soon)
  - ➕ Add Transaction - Opens create modal

**Functions Used**:
- `ImportTransactions()` - Navigates to `/settings/csv-import`
- `ExportTransactions()` - Shows alert (not implemented)
- `CreateTransaction()` - Opens `TransactionCreateModal`
- `ClearTripFilter()` - Removes trip filter and navigates to `/transactions`

---

### 2. **Summary Stats Cards**
```html
<div class="grid grid-cols-1 md:grid-cols-4 gap-4">
```

**Purpose**: Display key metrics at a glance

**Four Cards**:

#### Card 1: Total Transactions (Primary - Blue)
- Icon: 💳
- Value: `@totalTransactions`
- Description: `@validTransactions validated`

#### Card 2: Linked to Trips (Secondary - Purple)
- Icon: 🔗
- Value: `@linkedTransactions`
- Description: `@unlinkedTransactions unlinked`

#### Card 3: Total Amount (Accent - Cyan)
- Icon: 💰
- Value: `$@totalAmount` (formatted with thousand separators)
- Description: "This period"

#### Card 4: Need Attention (Warning - Yellow)
- Icon: ⚠️
- Value: `@flaggedTransactions`
- Description: "Validation required"

**Data Source**: Calculated in `UpdateStatistics()` from all transactions

---

### 3. **Filter Section**
```html
<div class="card bg-base-200 shadow-lg">
```

**Purpose**: Allows users to filter transactions by multiple criteria

**Filter Fields**:

#### Source Filter
- Type: Dropdown
- Options: All sources, Navan, Agent, Manual
- Binding: `@bind="filterSource"`

#### Category Filter
- Type: Dropdown
- Options: All categories, Airfare, Lodging, Meals, etc.
- Binding: `@bind="filterCategory"`

#### Date From
- Type: Date picker
- Binding: `@bind="filterDateFrom"`

#### Date To
- Type: Date picker
- Binding: `@bind="filterDateTo"`

#### Status Filter
- Type: Dropdown
- Options:
  - All
  - ✅ Valid
  - ❌ Invalid
  - 🔗 Linked
  - 🔓 Unlinked
- Binding: `@bind="filterStatus"`

#### Filter Button
- Icon: 🔍 Filter
- Function: `ApplyFilters()`

**Functions**:
- `ApplyFilters()` → `ApplyFiltersAndSort()`
  - Resets to page 1
  - Clears selection
  - Calls `LoadTransactions()`

---

### 4. **Transaction Table**
```html
<div class="card bg-base-100 shadow-lg" style="min-height: 600px;">
```

**Purpose**: Displays paginated transaction data in a scrollable table

**Table Features**:
- Fixed column widths for consistent layout
- Sticky header that stays visible while scrolling
- Row hover effects
- Selected row highlighting
- Sortable columns
- Scrollable content area (max-height: 600px)

#### Table Columns:

| Column | Width | Sortable | Features |
|--------|-------|----------|----------|
| Checkbox | 30px | No | Select individual transactions |
| Transaction ID | 100px | Yes | Monospaced font |
| Date | 120px | Yes | Format: dd/MM/yyyy |
| Source | 80px | No | Color-coded badges |
| Email | 140px | No | User email |
| Vendor | 120px | No | Merchant name |
| Category | 110px | Yes | Color-coded badges |
| Amount | 95px | Yes | USD with color (red/green) |
| Cabin | 95px | No | For airfare only |
| Trip | 110px | No | Clickable link or "Unlinked" |
| Status | 70px | No | Valid/Invalid badges |
| Actions | 100px | No | Dropdown menu |

#### Column Details:

**Transaction ID**
- Font: Monospace
- Format: Full ID string

**Date**
- Format: `dd/MM/yyyy`
- Sortable via `SortBy(nameof(Transaction.TransactionDate))`

**Source**
- Color coded:
  - Navan: Blue (`text-blue-600`)
  - Agent: Purple (`text-purple-600`)
  - Manual: Orange (`text-orange-600`)

**Category**
- Color coded by type:
  - Airfare: Blue
  - Lodging: Purple
  - Meals: Green
  - Transportation: Yellow
  - Communication: Cyan
  - Client entertainment: Pink
  - Other: Gray
  - Non-travel: Red
  - Uncategorized: Orange

**Amount**
- Shows USD if available
- Red for negative (refunds)
- Green for positive (purchases)
- Falls back to original currency if no USD

**Cabin Class**
- Only shown for Airfare transactions
- Shows "Missing" in red if airfare has no cabin class
- Color coded:
  - Economy: Green
  - Premium economy: Blue
  - Business: Purple
  - First: Orange

**Trip**
- Clickable link to trip detail page if linked
- Shows "🔓 Unlinked" in yellow if not linked

**Status**
- ✅ Green badge if valid
- ❌ Red badge if invalid
- ⚠️ Yellow badge if needs validation

**Actions Dropdown (⋮)**
- ✏️ Edit - Opens edit modal
- 🔗 Link to Trip - Link transaction to trip
- ✂️ Split - Split transaction
- 💬 Generate Message - Copy email template
- 📄 Documents - View attached documents
- ✅ Mark Valid - Set IsValid = true
- 🗑️ Delete - Remove transaction (with confirmation)

---

### 5. **Pagination Controls**
```html
<div class="flex justify-between items-center p-4 border-t">
```

**Purpose**: Navigate through large datasets

**Components**:

#### Left Side - Info Display
- Shows: "Showing X to Y of Z transactions"
- Per page selector: 25, 50, 100
- Binding: `@bind="pageSize"`
- Triggers: `OnPageSizeChanged()`

#### Right Side - Page Navigation
- « Previous button
- Page number buttons (current ±2)
- Next » button
- Current page highlighted in primary color

**Functions**:
- `PreviousPage()` - Go to previous page if exists
- `NextPage()` - Go to next page if exists
- `GoToPage(int page)` - Jump to specific page
- `OnPageSizeChanged()` - Reset to page 1, reload data

---

### 6. **Empty State**
```html
<div class="flex flex-col items-center justify-center h-64">
```

**Purpose**: Shown when no transactions match filters

**Display**:
- Icon: 📭 (large)
- Title: "No transactions found"
- Message: "Try adjusting your filters or import some transaction data."
- Button: 📥 Import Transactions

---

### 7. **Bulk Actions Bar**
```html
<div class="fixed bottom-6 left-1/2 transform -translate-x-1/2 z-50">
```

**Purpose**: Perform actions on multiple selected transactions

**Visibility**: Only shown when `selectedTransactions.Any()` is true

**Components**:
- Count display: "X selected"
- 🔗 Link to Trip - Bulk link transactions
- ✅ Mark Valid - Bulk validate
- 📤 Export - Export selected
- ✕ Clear - Clear selection

**Functions**:
- `BulkLinkToTrip()` - Link multiple transactions to one trip
- `BulkMarkValid()` - Mark all selected as valid
- `BulkExport()` - Export selected (not implemented)
- `ClearSelection()` - Deselect all

**Styling**:
- Fixed position at bottom center
- Primary colored card
- High z-index (50) to stay on top

---

## Modals

### 1. **TransactionEditModal**
```html
<TransactionEditModal @ref="editModal" OnTransactionUpdated="RefreshTransactions" />
```

**Purpose**: Edit existing transaction details

**Trigger**: Click "Edit" in actions dropdown

**Callback**: `RefreshTransactions()` on save

### 2. **TransactionCreateModal**
```html
<TransactionCreateModal @ref="createModal" OnTransactionCreated="RefreshTransactions" />
```

**Purpose**: Create new transaction manually

**Trigger**: Click "➕ Add Transaction" button

**Callback**: `RefreshTransactions()` on create

---

## Code-Behind Logic

### State Variables

#### Component State
```csharp
private bool isLoading = true;
private List<Transaction> transactions = new();
private IEnumerable<Transaction> filteredTransactions = new List<Transaction>();
private PagedResult<Transaction> pagedResult = new();
private HashSet<string> selectedTransactions = new();
```

#### Lookup Data
```csharp
private List<Source>? sources;
private List<Category>? categories;
private List<CabinClass>? cabinClasses;
```

#### Filters
```csharp
private string filterSource = "";
private string filterCategory = "";
private DateTime? filterDateFrom;
private DateTime? filterDateTo;
private string filterStatus = "";
```

#### Pagination & Sorting
```csharp
private int currentPage = 1;
private int pageSize = 50;
private string sortField = nameof(Transaction.TransactionDate);
private bool sortAscending = false;
```

#### Statistics
```csharp
private int totalTransactions = 0;
private int validTransactions = 0;
private int linkedTransactions = 0;
private int unlinkedTransactions = 0;
private decimal totalAmount = 0;
private int flaggedTransactions = 0;
```

---

## Key Functions

### Initialization

#### `OnInitializedAsync()`
**Flow**:
1. Load lookup data (sources, categories, cabin classes)
2. Check for TripIdFilter query parameter
3. Load transactions
4. Set `isLoading = false`

#### `LoadLookupData()`
**Purpose**: Load dropdown options from database
**Services**:
- `LookupService.GetSourcesAsync()`
- `LookupService.GetCategoriesAsync()`
- `LookupService.GetCabinClassesAsync()`

---

### Data Loading

#### `LoadTransactions()`
**Flow**:
1. Set `isLoading = true`
2. Create pagination parameters
3. Load all transactions for statistics
4. Apply trip filter if `TripIdFilter.HasValue`
5. Update statistics
6. Load paged transactions with filters
7. Apply trip filter to paged results
8. Set `isLoading = false`

**Error Handling**: Console log on failure

#### `LoadPagedTransactionsAsync(PaginationParams paginationParams)`
**Purpose**: Get paginated data from service
**Logic**:
- If filterStatus = "unlinked" → `GetUnlinkedTransactionsPagedAsync()`
- Otherwise → `GetAllTransactionsPagedAsync()`

#### `UpdateStatistics()`
**Calculates**:
- `totalTransactions` = count of all
- `validTransactions` = count where IsValid = true
- `linkedTransactions` = count where TripId has value
- `unlinkedTransactions` = total - linked
- `totalAmount` = sum of all AmountUSD values
- `flaggedTransactions` = count where DataValidation = true OR IsValid = false

---

### Filtering & Sorting

#### `ApplyFilters()`
Calls `ApplyFiltersAndSort()`

#### `ApplyFiltersAndSort()`
**Flow**:
1. Reset `currentPage = 1`
2. Clear `selectedTransactions`
3. Call `LoadTransactions()`
4. Call `StateHasChanged()`

#### `SortBy(string field)`
**Logic**:
- If clicking same field → toggle `sortAscending`
- If different field → set field, `sortAscending = true`
- Reload transactions
- Update UI

#### `GetSortIcon(string field)`
**Returns**:
- Empty string if not sorted by this field
- "↑" if ascending
- "↓" if descending

---

### Selection

#### `ToggleSelectAll(ChangeEventArgs e)`
**Logic**:
- If checked → select all filtered transactions
- If unchecked → clear selection

#### `ToggleSelectTransaction(string transactionId, bool isSelected)`
**Logic**:
- If selected → add to `selectedTransactions`
- If deselected → remove from `selectedTransactions`

#### `ClearSelection()`
Clears `selectedTransactions` HashSet

---

### Actions

#### `EditTransaction(Transaction transaction)`
Opens `editModal.ShowAsync(transaction)`

#### `LinkToTrip(Transaction transaction)`
Shows alert (feature not fully implemented)

#### `SplitTransaction(Transaction transaction)`
Shows alert with instructions to use Split Engine

#### `GenerateMessage(Transaction transaction)`
**Logic**:
1. Determine message type based on:
   - Meals category + amount ≥ $80
   - Client entertainment category
   - Other/uncategorized
   - Missing documentation
2. Generate appropriate email template using `MessageTemplateService`
3. Copy to clipboard
4. Show success alert

**Message Types**:
- Meals with external participants
- Meals with internal participants only
- Meals with no participants
- Client entertainment
- Other category
- Missing documentation

#### `ViewDocuments(Transaction transaction)`
Opens `DocumentUrl` in new tab or shows "no documents" alert

#### `MarkAsValid(Transaction transaction)`
Calls `TransactionService.MarkAsValidAsync(transactionId)` and refreshes

#### `DeleteTransaction(Transaction transaction)`
Shows confirmation dialog, deletes if confirmed, refreshes

#### `CreateTransaction()`
Opens `createModal.Show()`

#### `ImportTransactions()`
Navigates to `/settings/csv-import`

#### `ExportTransactions()`
Shows "coming soon" alert

---

### Bulk Actions

#### `BulkLinkToTrip()`
Shows alert (not fully implemented)

#### `BulkMarkValid()`
**Flow**:
1. Loop through `selectedTransactions`
2. Call `MarkAsValidAsync()` for each
3. Refresh transactions
4. Clear selection

#### `BulkExport()`
Shows count alert (not implemented)

---

### Helper Methods

#### `CloseDropdown()`
Removes focus from active element to close dropdown menu

#### `GetSourceColor(string sourceName)`
Returns Tailwind CSS class based on source:
- Navan → `text-blue-600`
- Agent → `text-purple-600`
- Manual → `text-orange-600`
- Default → `text-gray-600`

#### `GetCategoryColor(string categoryName)`
Returns color class for each category type

#### `GetCabinClassColor(string cabinClassName)`
Returns color class for each cabin class

---

## Styling

### Layout Classes
- Page container: `flex flex-col gap-4`
- Cards: `card bg-base-100 shadow-lg`
- Grids: `grid grid-cols-1 md:grid-cols-4 gap-4`

### Table Styling
- Zebra striping: `table-zebra`
- Pinned header: `table-pin-rows`
- Fixed layout: `table-layout: fixed`
- Sticky header: `sticky top-0 z-10`

### Responsive Design
- Mobile: Single column grids
- Tablet (md): 2-4 column grids
- Desktop (lg): Full layout

### Color Scheme
- Primary: Blue (transactions)
- Secondary: Purple (linked trips)
- Accent: Cyan (amounts)
- Warning: Yellow (attention needed)
- Success: Green (valid)
- Error: Red (invalid)

---

## Services Used

### ITransactionService
- `GetAllTransactionsAsync(bool includeRelated)`
- `GetAllTransactionsPagedAsync(PaginationParams)`
- `GetUnlinkedTransactionsPagedAsync(PaginationParams)`
- `MarkAsValidAsync(string transactionId)`
- `DeleteAsync(string transactionId)`

### ILookupService
- `GetSourcesAsync()`
- `GetCategoriesAsync()`
- `GetCabinClassesAsync()`

### IMessageTemplateService
- `GenerateMealsMessage(Transaction, List<string>?)`
- `GenerateClientEntertainmentMessage(Transaction, List<string>)`
- `GenerateOtherCategoryMessage(Transaction, Trip?)`
- `GenerateDocumentationMessage(Transaction)`

### NavigationManager
- `NavigateTo(string uri)`

### IJSRuntime
- `InvokeVoidAsync()` - For alerts, console logs, clipboard
- `InvokeAsync<bool>()` - For confirm dialogs

---

## Query Parameters

### TripIdFilter
**Type**: `int?`
**Attribute**: `[SupplyParameterFromQuery(Name = "tripId")]`
**Purpose**: Filter transactions by specific trip
**Behavior**: 
- Sets `filterStatus = "linked"` on init
- Shows info alert at top
- Filters both all transactions and paged results

---

## Performance Optimizations

### Data Loading
- Loads all transactions once for statistics (client-side)
- Uses paged service calls for display (server-side)
- Only reloads when filters/sorting changes

### Pagination
- Default page size: 50
- Options: 25, 50, 100
- Maintains current page during sort
- Resets to page 1 on filter change

### State Management
- Uses `HashSet<string>` for O(1) selection lookups
- Caches lookup data (sources, categories, cabin classes)
- Only calls `StateHasChanged()` when needed

---

## User Experience Features

### Visual Feedback
- Loading spinner during data fetch
- Row hover highlighting
- Selected row color change
- Disabled buttons when not applicable
- Color-coded status badges

### Keyboard & Mouse
- Click headers to sort
- Checkbox selection
- Dropdown menus
- Sticky header while scrolling

### Scrolling Behavior
- Main page scrolls normally
- Table has independent scroll (max 600px)
- Table minimum height: 600px
- Sticky header stays visible when scrolling table

### Error Handling
- Console logging for debugging
- User-friendly alerts
- Confirmation dialogs for destructive actions
- Graceful fallbacks for missing data

---

## Data Flow Diagram

```
User Action
    ↓
Component Method
    ↓
Service Call
    ↓
Database Query
    ↓
Service Response
    ↓
Update State Variables
    ↓
StateHasChanged()
    ↓
UI Re-renders
```

---

## Common Use Cases

### 1. View All Transactions
1. Navigate to `/transactions`
2. Page loads with default filters (none)
3. Shows first 50 transactions
4. Statistics calculated and displayed

### 2. Filter by Category
1. Select category from dropdown
2. Click "🔍 Filter" button
3. `ApplyFilters()` called
4. Data reloaded with filter
5. Page resets to 1
6. Selection cleared

### 3. Sort by Date
1. Click "Date" column header
2. `SortBy(nameof(Transaction.TransactionDate))` called
3. First click: Ascending
4. Second click: Descending
5. Data reloaded with sort
6. Current page maintained

### 4. Edit Transaction
1. Click ⋮ in actions column
2. Select "✏️ Edit"
3. Modal opens with transaction data
4. User makes changes
5. Modal saves and triggers `RefreshTransactions()`
6. Table reloads with updated data

### 5. Mark Multiple as Valid
1. Check boxes for transactions
2. Bulk actions bar appears
3. Click "✅ Mark Valid"
4. Loop through selected IDs
5. Each marked valid via service
6. Table refreshed
7. Selection cleared

### 6. View Trip Transactions
1. Navigate from trip detail page with `?tripId=123`
2. `TripIdFilter` set to 123
3. Alert shown at top
4. Filters applied automatically
5. Only shows transactions for that trip

---

## Future Enhancements

### Planned Features
- ✅ Full export functionality (CSV/Excel)
- ✅ Complete link to trip implementation
- ✅ Split transaction workflow
- ✅ Advanced filtering (amount range, vendor search)
- ✅ Column customization
- ✅ Save custom views
- ✅ Real-time validation feedback

### Performance Improvements
- Virtual scrolling for very large datasets
- Debounced search inputs
- Lazy loading of documents
- Caching strategy for frequently accessed data

---

## Troubleshooting

### Common Issues

**Problem**: Statistics don't match filtered results
**Cause**: Statistics calculated from ALL transactions, not filtered
**Solution**: This is by design - stats show overall system status

**Problem**: Selection lost after filter
**Cause**: `ApplyFiltersAndSort()` clears selection
**Solution**: By design - prevents selecting items no longer visible

**Problem**: Modal doesn't open
**Cause**: Modal reference is null
**Solution**: Check `@ref` binding and modal component initialization

**Problem**: Dropdown stays open after action
**Cause**: Blazor doesn't auto-close dropdowns
**Solution**: Call `CloseDropdown()` before action

---

## Dependencies

### NuGet Packages
- Microsoft.AspNetCore.Components
- Microsoft.JSInterop

### Project References
- TravelOperation.Core (Services, Models, Entities)
- TrevelOperation.RazorLib (Components)

### External Libraries
- TailwindCSS (styling)
- DaisyUI (components)

---

## Related Documentation
- [Data Integrity Controls](./DATA_INTEGRITY.md)
- [Trip Management](./TRIP_MANAGEMENT.md)
- [Message Templates](./MESSAGE_TEMPLATES.md)
- [CSV Import](./CSV_IMPORT.md)

---

**Last Updated**: January 10, 2025
**Version**: 3.0
**Maintained By**: Development Team
