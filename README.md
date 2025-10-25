# Travel Expense Management System

A comprehensive WPF + Blazor Hybrid application for managing business travel expenses, trips, and ensuring tax compliance.

## 🚀 Features

### Core Functionality
- **Transaction Management**: Track business travel transactions from multiple sources (Navan, Agent, Manual)
- **Trip Management**: Create, edit, and validate trips with automatic suggestion engine
- **Data Integrity Controls**: Automated validation for airfare, meals, lodging, client entertainment, and documentation
- **Tax Compliance**: Calculate tax exposure based on configurable caps per country/subsidiary
- **Travel Spend Reporting**: Comprehensive aggregate reports with cost breakdowns
- **Split Engine**: Split transactions across multiple participants with automatic suggestions
- **Matching Engine**: Link transactions to trips with manual and automatic matching

### Technical Features
- **Server-Side Pagination**: Efficient data loading for all major tables with PagedResult<T>
- **Role-Based Access Control (RBAC)**: Finance and Owner roles with granular permissions
- **Theme Support**: Light/Dark mode with system preference detection and localStorage persistence
- **Caching Layer**: In-memory caching for lookup tables and settings (60-min/30-min TTL)
- **Audit Logging**: Complete change tracking with restore capability
- **CSV Import**: Bulk import with configurable transformation rules
- **Export Functionality**: CSV/Excel export for all major reports

## 🏗️ Architecture

### Technology Stack
- **.NET 9.0** (Preview RC1)
- **WPF** - Desktop application host
- **Blazor WebView** - Modern web UI components
- **Entity Framework Core** - ORM with SQLite database
- **Microsoft.Extensions.Caching.Memory** - In-memory caching
- **DaisyUI + Tailwind CSS** - UI styling framework

### Project Structure
```
TrevelOperation/                    # WPF host application
├── MainWindow.xaml                 # Main WPF window
├── Startup.cs                      # Dependency injection configuration
└── wwwroot/                        # Static assets

TrevelOperation.RazorLib/           # Blazor components
├── Pages/                          # Page components
│   ├── Reports/                    # Transactions, Trips, Travel Spend
│   ├── DataIntegrity/              # Airfare, Meals, Lodging, etc.
│   └── Settings/                   # Configuration pages
├── Components/                     # Reusable components
│   ├── DataTable.razor             # Advanced table with sorting, filtering
│   └── ThemeProvider.razor         # Theme management
└── Theme/                          # Theme configuration

TravelOperation.Core/               # Domain layer
├── Models/                         # Entity models
│   ├── Entities/                   # Transaction, Trip, etc.
│   └── Lookup/                     # Category, Source, Purpose, etc.
├── Services/                       # Business logic
│   ├── TransactionService.cs
│   ├── TripService.cs
│   ├── LookupService.cs
│   ├── CacheService.cs             # Caching implementation
│   └── AuditService.cs
├── Interfaces/                     # Service contracts
│   └── ICacheService.cs
└── Data/                           # Database context
    └── TravelDbContext.cs

TrevelOperation.Service/            # Application services
├── SettingsService.cs              # Tax and headcount management
├── CsvImportService.cs             # CSV import logic
├── TaxCalculationService.cs        # Tax exposure calculations
└── MessageTemplateService.cs       # Email template generation
```

## 💾 Database Schema

### Main Tables
- **Transactions**: Travel expense records with categorization, amounts, participants
- **Trips**: Travel trips with dates, locations, purpose, status, validation
- **Lookup Tables**: Categories, Sources, Purposes, CabinClasses, TripTypes, Status, etc.
- **Owners**: Employee information with cost center and department
- **Headcount**: Employee directory by period
- **Tax**: Tax caps and shields by fiscal year, country, and subsidiary
- **AuditLog**: Complete change history with old/new values

### Key Relationships
- Transactions → Trips (many-to-one)
- Transactions → Category, Source, BookingType, BookingStatus (foreign keys)
- Trips → Purpose, TripType, Status, ValidationStatus, Owner (foreign keys)
- Tax → Country, Subsidiary (indexed)

## ⚡ Caching Implementation

### Overview
The application uses an in-memory caching layer to improve performance for frequently accessed data. The cache-aside pattern is implemented with lazy loading and pattern-based invalidation.

### Cached Data

#### Lookup Tables (60-minute TTL)
- Categories (✈ Airfare, 🏨 Lodging, 🚕 Transportation, etc.)
- Sources (Navan, Agent, Manual)
- Purposes (💼 Business trip, 🎓 Onboarding, 🏖 Company trip, 🛡 BCP)
- CabinClasses (💺 Economy, 🛫 Premium economy, 🧳 Business, 👑 First)
- TripTypes (🏠 Domestic, 🌍 International, 📍 Local)
- Status (🔴 Canceled, ⚪ Upcoming, 🔵 Ongoing, 🟢 Completed)
- ValidationStatus (⚪ Not ready, 🟡 Ready, 🟢 Validated)
- BookingTypes (✈ Flight, 🏨 Hotel, 🚗 Car, 🚆 Train)
- BookingStatus (🔴 Canceled, 🟢 Approved)
- Owners (Employee list)
- Countries and Cities

#### Settings Data (30-minute TTL)
- Tax Settings (caps, shields by fiscal year and country)
- Headcount (employee directory)

### Cache Invalidation Strategy

**Pattern-Based Invalidation:**
- All lookup Create/Update/Delete operations clear `lookup_*` keys
- Tax setting mutations clear `settings_tax*` keys
- Headcount mutations clear `settings_headcount_all` key

**Automatic Eviction:**
- Absolute expiration based on TTL
- PostEvictionCallback removes keys from tracking dictionary

### Performance Benefits
- **First Load**: 20-50ms (database query)
- **Cached Load**: 1-5ms (memory access)
- **Improvement**: 80-95% reduction in response time

### Cache Keys Reference
```csharp
// Lookup keys (60-minute TTL)
CacheKeys.Categories            "lookup_categories"
CacheKeys.Sources              "lookup_sources"
CacheKeys.Purposes             "lookup_purposes"
CacheKeys.CabinClasses         "lookup_cabin_classes"
CacheKeys.TripTypes            "lookup_trip_types"
CacheKeys.Status               "lookup_status"
CacheKeys.ValidationStatus     "lookup_validation_status"
CacheKeys.BookingTypes         "lookup_booking_types"
CacheKeys.BookingStatus        "lookup_booking_status"
CacheKeys.Owners               "lookup_owners"
CacheKeys.CountriesAndCities   "lookup_countries_cities"

// Settings keys (30-minute TTL)
CacheKeys.TaxSettingsAll       "settings_tax_all"
CacheKeys.HeadcountAll         "settings_headcount_all"

// Pattern matchers
CacheKeys.LookupPattern        "lookup_*"
CacheKeys.SettingsPattern      "settings_*"
```

For detailed technical documentation, see [CACHING_IMPLEMENTATION.md](./CACHING_IMPLEMENTATION.md).

## 🔐 Security & Permissions

### Role-Based Access Control (RBAC)
- **Finance Role**: Full access to all features and data
- **Owner Role**: Limited access to own trips and transactions only

### Protected Features
- Data Integrity Controls (Finance only)
- Settings Management (Finance only)
- All CRUD operations filtered by user role
- Service-layer enforcement in addition to UI protection

## 📊 Data Integrity Controls

### Airfare Control
**Purpose**: Ensure cabin class is assigned to all airfare transactions  
**Filter**: `Category = 'Airfare' AND CabinClass IS NULL`  
**Actions**: Edit cabin class, recategorize

### Meals Control
**Purpose**: Review high-value meal transactions (≥$80)  
**Filter**: `Category = 'Meals' AND ABS(AmountUSD) >= 80 AND IsValid = FALSE`  
**Actions**: Mark as valid, update category, generate email template  
**Message Templates**: Detects internal/external participants

### Lodging Control
**Purpose**: Review unusually low lodging charges (≤$100)  
**Filter**: `Category = 'Lodging' AND ABS(AmountUSD) <= 100 AND IsValid = FALSE`  
**Actions**: Mark as valid, recategorize

### Client Entertainment Control
**Purpose**: Ensure participant information is complete  
**Filter**: `Category = 'Client entertainment' AND ParticipantsValidated = FALSE`  
**Actions**: Add participants, generate email template

### Other Control
**Purpose**: Categorize "Other" transactions properly  
**Filter**: `Category = 'Other'`  
**Actions**: Update category, generate email template

### Missing Documentation Control
**Purpose**: Flag transactions without receipts  
**Filter**: `DocumentUrl IS NULL OR DocumentUrl = ''`

## 🧮 Tax Calculations

### Tax Exposure Formula
```
Meals Exposure = IF (MealsPerDay > MealsCap):
    Duration × (MealsPerDay - MealsCap)

Lodging Exposure = IF (LodgingPerNight > LodgingCap):
    Duration × (LodgingPerNight - LodgingCap)

Airfare Exposure = IF CabinClass IN ('First', 'Business'):
    Flag = TRUE

Total Tax Exposure = Meals Exposure + Lodging Exposure
```

### Tax Settings Configuration
- Fiscal Year
- Country
- Subsidiary (e.g., "WSC IL")
- Meals Cap (per day)
- Lodging Cap (per night)
- Tax Shield percentage

## 📥 CSV Import

### Supported Sources
1. **Navan**: Corporate travel booking platform
2. **Agent**: Travel agent bookings
3. **Manual**: Manual entry template

### Import Process
1. Upload CSV file
2. Apply transformation rules (policy field → category mapping)
3. Calculate USD amounts using exchange rates
4. Auto-detect participants from transaction data
5. Link to external trip ID if available
6. Save to database with audit trail

### Transformation Rules
Configurable mapping from policy field to category:
- `'tripactions_fees'` → Trip fee
- `'Airalo'` → Communication
- `'public transport, tolls & parking'` → Transportation
- `'entertaining clients'` → Client entertainment
- `'Meals for myself'` → Meals
- `'Airfare'` → Airfare
- `'Lodging'` → Lodging
- etc.

Managed in **Settings → Data Transformation**

## 📤 Export Functionality

All major reports support export to:
- **CSV**: Plain text format
- **Excel**: Formatted with headers and styling

Exported data includes:
- Current filters and sorting
- Visible columns only
- Formatted dates and numbers

## 🎨 Theme System

### Features
- **Light/Dark Mode**: Toggle between themes
- **System Preference Detection**: Automatically detects OS theme preference
- **Persistent**: Theme choice saved in localStorage
- **Smooth Transitions**: CSS transitions for theme changes
- **100% Coverage**: All pages support theming

### Theme Classes (DaisyUI)
- Light: `light` theme with professional color scheme
- Dark: `dark` theme with comfortable contrast

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK (Preview RC1 or later)
- Visual Studio 2022 (v17.12+) or VS Code
- Node.js (for Vite dev server in RazorLib)

### Setup
1. **Clone repository**
   ```bash
   git clone https://github.com/Fidazahid/TrevelOperation.git
   cd TrevelOperation
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Build solution**
   ```bash
   dotnet build TrevelOperation.sln
   ```

4. **Initialize database**
   ```bash
   cd TravelOperation.Core
   dotnet ef database update
   ```

5. **Import initial data**
   - Countries & Cities CSV
   - Tax rates CSV
   - Headcount data
   - Configure Owners in Settings

6. **Run application**
   ```bash
   cd ../TrevelOperation
   dotnet run
   ```

### First-Time Configuration
1. Navigate to **Settings → Manage Lists** to verify lookup tables
2. Go to **Settings → Owners** to add employee list
3. Configure **Settings → Tax Settings** for each subsidiary/country
4. Import **Settings → Headcount** for employee directory
5. Set up **Settings → Data Transformation** rules for CSV imports

## 🧪 Testing

### Unit Tests
```bash
dotnet test TravelOperation.Tests
```

### Integration Tests
```bash
dotnet test TravelOperation.IntegrationTests
```

### Test Coverage
- Transaction CRUD operations
- Trip management and validation
- Tax calculation logic
- Split transaction functionality
- Message template generation
- Cache invalidation

## 📝 Audit Logging

All data modifications are logged:
- **Who**: User email
- **What**: Action type (Create, Edit, Delete, Link, Unlink, Split)
- **When**: Timestamp (dd/MM/yyyy HH:mm:ss)
- **Where**: Table and record ID
- **Old Value**: JSON snapshot before change
- **New Value**: JSON snapshot after change

View audit history: **Settings → Audit Log**

Restore capability available for non-split records.

## 🗓️ Date & Number Formatting

### Standards
- **Dates**: dd/MM/yyyy (e.g., 25/10/2025)
- **Timestamps**: dd/MM/yyyy HH:mm:ss (Israel timezone)
- **Amounts**: 1,000.00 (with thousand separators, 2 decimals)
- **USD Amounts**: $1,000.00 (with $ prefix)

### Fonts
- **Headers**: Sora (geometric sans-serif)
- **Body**: Inter Tight (sans-serif)

## 📊 Table Features

All data tables include:
- ✅ Create and save custom views
- ✅ Change column layout (reorder columns)
- ✅ Resize column widths
- ✅ Sort by any column
- ✅ Filter by multiple criteria
- ✅ Export to CSV/Excel
- ✅ Server-side pagination for large datasets

## 🔧 Configuration

### Application Settings
- **Database**: SQLite with WAL mode (`TravelOperations.db`)
- **Cache TTL**: 60 min (lookups), 30 min (settings)
- **Page Size**: 100 rows (configurable per user)
- **Timezone**: Israel (GMT+2/+3)

### Environment Variables
None required - all configuration in `appsettings.json`

## 📚 Documentation

- **[PROJECT_TASKS.md](./PROJECT_TASKS.md)**: Complete feature checklist
- **[CACHING_IMPLEMENTATION.md](./CACHING_IMPLEMENTATION.md)**: Technical caching documentation
- **[THEME_VERIFICATION_REPORT.md](./THEME_VERIFICATION_REPORT.md)**: Theme system details
- **[RBAC_VERIFICATION_REPORT.md](./RBAC_VERIFICATION_REPORT.md)**: Security implementation
- **[TEST_SUMMARY.md](./TEST_SUMMARY.md)**: Test results and coverage

## 🤝 Contributing

1. Create feature branch from `version3`
2. Implement changes with tests
3. Update documentation
4. Submit pull request

## 📄 License

Proprietary - All rights reserved

## 👥 Authors

- Development Team: WSC Travel Operations

## 📞 Support

For issues or questions, please contact the development team.

---

**Current Version**: 3.0  
**Last Updated**: October 2025  
**Build Status**: ✅ Passing (160 warnings, 0 errors)
