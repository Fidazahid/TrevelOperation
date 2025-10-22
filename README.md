# ✈️ Travel Expense Management System

A comprehensive WPF application with Blazor WebView for managing business travel expenses, ensuring tax compliance, and generating detailed reports.

## 🎯 Overview

This system manages employee travel expenses, links transactions to trips, validates spending against company policies, and generates reports for tax compliance and financial analysis.

## ✨ Key Features

### 📊 **Transaction Management**
- **Import from multiple sources**: Navan, Agent, Manual entry
- **Automatic categorization**: Based on vendor and policy rules
- **Currency conversion**: Multi-currency support with exchange rates
- **Validation controls**: Ensure data integrity and compliance
- **Inline editing**: Direct table editing with highlighted editable fields

### 🧳 **Trip Management**
- **Manual trip creation**: Full trip details with validation
- **Automatic trip suggestions**: AI-powered grouping of related transactions
- **Trip validation**: Compliance checking and approval workflows
- **Tax exposure calculation**: Per-trip analysis with country-specific rules

### 🔍 **Data Integrity Controls**
- **Airfare Control**: Cabin class validation and compliance
- **Meals Control**: High-value transaction review (≥$80)
- **Lodging Control**: Low-value transaction validation (≤$100)
- **Client Entertainment**: Participant validation and documentation
- **Other Categorization**: Proper classification of miscellaneous expenses
- **Missing Documentation**: Receipt and documentation tracking

### 📈 **Advanced Analytics**
- **Travel Spend Reports**: Comprehensive cost breakdowns per trip
- **Tax Compliance Reports**: Exposure analysis and regulatory compliance
- **Monthly Reports**: Executive summaries for meetings
- **Audit Trail**: Complete change history with restore capabilities

### 🚀 **Professional Table Features**
- **✅ Column sorting** - Click any header to sort
- **✅ Column resizing** - Drag column borders to resize
- **✅ Column reordering** - Drag & drop to reorder
- **✅ Column visibility** - Show/hide columns as needed
- **✅ Search & filtering** - Real-time search across all data
- **✅ Pagination** - Handle large datasets efficiently
- **✅ Custom views** - Save and load personalized table layouts
- **✅ Export capabilities** - CSV, Excel, PDF formats
- **✅ Keyboard navigation** - Arrow keys, Tab, Escape support
- **✅ Responsive design** - Works on different screen sizes

## 🏗️ Architecture

### **Technology Stack**
- **.NET 9.0** - Latest framework with performance improvements
- **WPF + Blazor WebView** - Modern desktop application with web UI
- **Entity Framework Core 9.0** - Database ORM with SQLite
- **ClosedXML** - Excel generation and manipulation
- **iText7** - PDF generation and reporting
- **Tailwind CSS** - Modern utility-first styling
- **xUnit** - Comprehensive testing framework

### **Project Structure**
```
TrevelOperation/
├── TrevelOperation/              # Main WPF application
├── TrevelOperation.RazorLib/     # Blazor UI components
├── TravelOperation.Core/         # Business logic and services
├── TravelOperation.Tests/        # Comprehensive test suite
└── docs/                        # Documentation
```

## 📋 Business Requirements Compliance

### **✅ Formatting Standards**
- **Dates**: dd/MM/yyyy format (25/12/2024)
- **Currency**: 1,000.00 with thousand separators
- **Timestamps**: dd/MM/yyyy HH:mm:ss (Israel timezone)
- **Headers**: Professional capitalization
- **Fonts**: Inter Tight body, Sora headers

### **✅ Data Management**
- **Editable lookup lists**: Categories, Sources, Purposes, etc.
- **Comprehensive audit trail**: Track all changes with timestamps
- **Message templates**: Automated communication generation
- **Tax calculations**: Country-specific rules and exposure analysis
- **Exchange rate handling**: Multi-currency support

### **✅ Validation Rules**
- **Transaction validation**: Amount, date, email format checks
- **Trip validation**: Date ranges, duration calculations
- **Policy compliance**: Meal caps, lodging limits, cabin class rules
- **Documentation requirements**: Receipt and approval tracking

## 🚀 Quick Start

### Prerequisites
- Windows 10/11
- .NET 9.0 Runtime
- Visual Studio 2022 or VS Code

### Installation
```bash
# Clone repository
git clone https://github.com/your-org/TrevelOperation.git
cd TrevelOperation

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run application
dotnet run --project TrevelOperation
```

### First Run Setup
1. **Database initialization**: Automatic SQLite database creation
2. **Import lookup data**: Categories, countries, tax rules
3. **Configure owners**: Set up company owners and roles
4. **Import historical data**: Upload existing transaction files
5. **Validate and review**: Check data integrity and accuracy

## 📊 Key Reports

### **Travel Spend Report**
- Trip-by-trip cost analysis
- Category breakdowns (Airfare, Lodging, Meals, etc.)
- Per-day and per-night calculations
- Tax exposure highlighting
- Owner and department analysis

### **Tax Compliance Report**
- Country-specific tax rule application
- Meals and lodging cap analysis
- Business class travel flagging
- Risk assessment and exposure calculation
- Audit-ready documentation

### **Monthly Executive Summary**
- Total spending by category
- Top travelers and departments
- Policy compliance metrics
- Year-over-year comparisons
- Key performance indicators

## 🛠️ Advanced Features

### **Matching Engine**
- **Automatic suggestions**: Link transactions to trips based on dates/locations
- **Manual matching**: User-controlled transaction-trip associations
- **Confidence scoring**: AI-powered suggestion ranking
- **Bulk operations**: Process multiple matches simultaneously

### **Split Engine**
- **Automatic detection**: Identify shared expenses
- **Participant management**: Track who attended meals/events
- **Fair splitting**: Calculate per-person amounts
- **Audit compliance**: Maintain detailed split records

### **Message Templates**
Generate professional communications for:
- High-value meal validation
- Client entertainment documentation
- Missing receipt requests
- Policy violation notifications
- Approval requirement alerts

## 🧪 Testing & Quality

### **Comprehensive Test Suite**
- **Unit tests**: Business logic validation
- **Integration tests**: Database and service testing
- **UI tests**: Component interaction testing
- **Performance tests**: Large dataset handling
- **Security tests**: Input validation and injection prevention

### **Quality Metrics**
- Code coverage > 80%
- All business rules tested
- Performance benchmarks
- Security vulnerability scans
- User acceptance testing

## 📈 Performance Features

### **Optimized Data Handling**
- **Lazy loading**: Load data on demand
- **Pagination**: Handle large datasets efficiently
- **Indexes**: Optimized database queries
- **Caching**: Reduce repeated calculations
- **Virtual scrolling**: Smooth large table rendering

### **Scalability**
- **Central package management**: Consistent versioning
- **Modular architecture**: Easy feature additions
- **Database optimization**: Efficient queries and indexes
- **Memory management**: Proper disposal patterns
- **Background processing**: Non-blocking operations

## 🔒 Security & Compliance

### **Data Protection**
- **Input validation**: Prevent injection attacks
- **Audit logging**: Track all data changes
- **Access control**: Role-based permissions
- **Data encryption**: Secure sensitive information
- **Backup procedures**: Regular data protection

### **Compliance Features**
- **Tax regulation support**: Country-specific rules
- **Corporate policy enforcement**: Automated validation
- **Documentation requirements**: Receipt tracking
- **Approval workflows**: Multi-level authorization
- **Audit trail**: Complete change history

## 📚 Documentation

- **[Deployment Guide](DEPLOYMENT.md)** - Complete setup instructions
- **[User Manual](docs/UserManual.md)** - End-user documentation
- **[API Documentation](docs/API.md)** - Developer reference
- **[Database Schema](docs/DatabaseSchema.md)** - Data structure details

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup
```bash
# Install dependencies
dotnet restore

# Run tests
dotnet test

# Start development server
dotnet run --project TrevelOperation
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Documentation**: Check our comprehensive guides
- **Issues**: Report bugs on GitHub Issues
- **Email**: support@company.com
- **Phone**: +1-xxx-xxx-xxxx (Emergency only)

---

## 🏆 Achievement Summary

### ✅ **Completed Features (18/18)**
1. ✅ Database Schema - Comprehensive SQLite database
2. ✅ Entity Framework Setup - Full ORM configuration  
3. ✅ Core Business Services - All CRUD operations
4. ✅ Data Import Services - Multi-source CSV import
5. ✅ Lookup Data Management - Dynamic list management
6. ✅ Main Navigation Structure - Professional UI layout
7. ✅ Transaction Management UI - Advanced table features
8. ✅ Trip Management UI - Complete workflow support
9. ✅ Data Integrity Controls - All validation rules
10. ✅ Tax Calculation Engine - Multi-country support
11. ✅ Travel Spend Reporting - Comprehensive analytics
12. ✅ Audit Logging System - Complete change tracking
13. ✅ Matching Engine - AI-powered suggestions
14. ✅ Split Engine - Shared expense management
15. ✅ Settings Management - Full configuration system
16. ✅ Message Template System - Automated communications
17. ✅ Export and Reporting - Multi-format support
18. ✅ UI Polish and Testing - Professional table features

**Built with ❤️ for efficient travel expense management**