# Travel Expense Management System - Deployment Guide

## Prerequisites
- .NET 9.0 Runtime
- Visual Studio 2022 or VS Code
- SQLite support
- Windows 10/11 (for WPF application)

## Installation Steps

### 1. Clone Repository
```bash
git clone https://github.com/your-org/TrevelOperation.git
cd TrevelOperation
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Database Setup
```bash
# Create initial migration (if not exists)
dotnet ef migrations add InitialCreate --project TravelOperation.Core

# Update database
dotnet ef database update --project TravelOperation.Core
```

### 4. Build Solution
```bash
dotnet build --configuration Release
```

### 5. Run Tests
```bash
dotnet test TravelOperation.Tests
```

### 6. Publish Application
```bash
dotnet publish TrevelOperation --configuration Release --output ./publish
```

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TravelExpense.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "TravelExpenseSettings": {
    "DefaultCurrency": "USD",
    "MaxFileUploadSize": "10MB",
    "DateFormat": "dd/MM/yyyy",
    "NumberFormat": "N2"
  }
}
```

### Environment Variables
```
ASPNETCORE_ENVIRONMENT=Production
TRAVEL_EXPENSE_DB_PATH=C:\Data\TravelExpense.db
TRAVEL_EXPENSE_LOG_PATH=C:\Logs\TravelExpense
```

## Initial Data Setup

### 1. Import Lookup Data
- Navigate to Settings > Manage Lists
- Add default categories, sources, purposes, etc.
- Import countries and cities CSV

### 2. Configure Tax Settings
- Go to Settings > Tax Settings
- Add tax rules for each country/subsidiary
- Set meal and lodging caps

### 3. Setup Users and Owners
- Add company owners in Settings > Owners
- Configure user access roles

### 4. Import Historical Data
- Use Settings > CSV Import
- Upload transaction data from Navan/Agent
- Review and validate imported data

## Features Verification Checklist

### ✅ Core Functionality
- [ ] Transaction management (CRUD)
- [ ] Trip management (create, edit, link)
- [ ] CSV import (Navan, Agent, Manual)
- [ ] Data validation and controls
- [ ] Tax calculation engine
- [ ] Export functionality (CSV, Excel, PDF)

### ✅ Table Features
- [ ] Column sorting (click headers)
- [ ] Column resizing (drag handles)
- [ ] Column reordering (drag & drop)
- [ ] Column show/hide
- [ ] Search and filtering
- [ ] Pagination
- [ ] Custom views (save/load)
- [ ] Inline editing
- [ ] Keyboard navigation

### ✅ Data Integrity Controls
- [ ] Airfare control (cabin class validation)
- [ ] Meals control (high-value validation)
- [ ] Lodging control (low-value validation)
- [ ] Client entertainment (participants)
- [ ] Other categorization
- [ ] Missing documentation

### ✅ Business Rules
- [ ] Date formatting (dd/MM/yyyy)
- [ ] Currency formatting (1,000.00)
- [ ] Tax exposure calculations
- [ ] Message template generation
- [ ] Audit logging
- [ ] Matching engine
- [ ] Split engine

## Performance Optimization

### Database Indexes
```sql
CREATE INDEX IX_Transactions_Email ON Transactions(Email);
CREATE INDEX IX_Transactions_TransactionDate ON Transactions(TransactionDate);
CREATE INDEX IX_Transactions_CategoryId ON Transactions(CategoryId);
CREATE INDEX IX_Transactions_TripId ON Transactions(TripId);
CREATE INDEX IX_Trips_Email ON Trips(Email);
CREATE INDEX IX_Trips_StartDate ON Trips(StartDate);
CREATE INDEX IX_AuditLog_Timestamp ON AuditLog(Timestamp);
```

### Caching Configuration
- Enable response caching for lookup data
- Use in-memory caching for frequently accessed data
- Configure distributed caching for multi-instance deployments

## Security Considerations

### Data Protection
- Database file encryption at rest
- Secure connection strings
- Input validation and sanitization
- SQL injection prevention (EF Core)

### Access Control
- Role-based access control
- Audit logging for all changes
- Secure file uploads
- XSS protection

## Monitoring and Logging

### Application Logs
- Transaction operations
- Data import/export activities
- Validation errors
- Performance metrics

### Error Handling
- Global exception handling
- User-friendly error messages
- Detailed logging for debugging
- Graceful degradation

## Backup and Recovery

### Database Backup
```bash
# Daily backup
copy "TravelExpense.db" "Backups\TravelExpense_$(Get-Date -Format 'yyyyMMdd').db"
```

### Configuration Backup
- Export saved table views
- Backup transformation rules
- Export tax settings
- Save custom categories

## Troubleshooting

### Common Issues

#### Database Connection Issues
```
Error: Unable to open database file
Solution: Check file permissions and path
```

#### Package Restore Issues
```
Error: Package restore failed
Solution: Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore
```

#### Export Functionality Issues
```
Error: Excel export fails
Solution: Verify ClosedXML package installation
Check file permissions for temp directory
```

#### Table Performance Issues
```
Issue: Slow table rendering with large datasets
Solution: Enable pagination, reduce page size
Add database indexes for filtered columns
```

## Maintenance

### Regular Tasks
- Database optimization and cleanup
- Log file rotation
- Performance monitoring
- Security updates
- User training and documentation updates

### Monthly Tasks
- Review audit logs
- Validate tax calculation accuracy
- Update exchange rates
- Backup verification

### Quarterly Tasks
- Review and update transformation rules
- Tax setting updates
- Performance analysis
- User feedback incorporation

## Support

### Documentation
- User Manual: `/docs/UserManual.md`
- API Documentation: `/docs/API.md`
- Database Schema: `/docs/DatabaseSchema.md`

### Contact Information
- Technical Support: support@company.com
- Business Questions: business@company.com
- Emergency: +1-xxx-xxx-xxxx

---

## Version Information
- Application Version: 1.0.0
- .NET Version: 9.0
- Entity Framework Version: 9.0.10
- Last Updated: October 2025