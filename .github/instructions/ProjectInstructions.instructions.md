---
applyTo: '**'
---
Provide project context and coding guidelines that AI should follow when generating code, answering questions, or reviewing changes.
Travel Expense Management System - Instructions 
Overview
Build a comprehensive travel expense management system for tracking business travel transactions, trips, and ensuring tax compliance. The system manages employee travel expenses, links transactions to trips, validates spending against company policies, and generates reports.
________________________________________

1. FORMATTING STANDARDS
Date & Time
•	Dates: dd/MM/yyyy (e.g., 25/12/2024)
•	Timestamps: dd/MM/yyyy HH:mm:ss (Israel timezone)
Numbers
•	Amounts: 1,000.00 (with thousand separators, 2 decimals)
•	USD Amounts: $1,000.00 (with $ prefix)
Text
•	Headers: First letter capitalized, rest lowercase (e.g., "Transaction details")
•	Body font: Inter Tight (or similar sans-serif for WPF)
•	Header font: Sora (or similar geometric sans-serif for WPF)
Table Features (ALL TABLES MUST HAVE)
•	✅ Create and save custom views
•	✅ Change column layout (reorder columns)
•	✅ Resize column widths
•	✅ Sort by any column
•	✅ Export to CSV/Excel
________________________________________
2. DATABASE SCHEMA
2.1 Lookup Tables (Editable Lists)
Sources:
- Navan
- Agent
- Manual

Categories (with emoji):
- ✈ Airfare
- 🏨 Lodging
- 🚕 Transportation
- 📱 Communication
- 🍸 Client entertainment
- 🍽 Meals
- ❔ Other
- ❓ Non-travel

Purposes (with emoji):
- 💼 Business trip
- 🎓 Onboarding
- 🏖 Company trip
- 🛡 BCP

CabinClasses (with emoji):
- 💺 Economy
- 🛫 Premium economy
- 🧳 Business
- 👑 First

TripTypes (with emoji):
- 🏠 Domestic
- 🌍 International
- 📍 Local

Status (with emoji):
- 🔴 Canceled
- ⚪ Upcoming
- 🔵 Ongoing
- 🟢 Completed

ValidationStatus (with emoji):
- ⚪ Not ready to validate
- 🟡 Ready to validate
- 🟢 Validated

BookingTypes (with emoji):
- ✈ Flight
- 🏨 Hotel
- 🚗 Car
- 🚆 Train

BookingStatus (with emoji):
- 🔴 Canceled
- 🟢 Approved

Owners:
- Maayan Chesler
- Martina Poplinsk
All these lists must be editable - Add, Edit, Delete functionality in Settings.
2.2 Main Tables
Transactions Table
TransactionId (PRIMARY KEY, VARCHAR)
Source (FK to Sources)
Email (VARCHAR)
TransactionDate (DATE)
AuthorizationDate (DATE)
TransactionType (VARCHAR) - Purchase, Refund, Reimbursement
CategoryId (FK to Categories)
Vendor (VARCHAR)
MerchantCategory (VARCHAR)
Address (VARCHAR)
SourceTripId (VARCHAR) - from external system
BookingId (VARCHAR)
BookingStatusId (FK to BookingStatus)
BookingStartDate (DATE)
BookingEndDate (DATE)
BookingTypeId (FK to BookingTypes)
Policy (VARCHAR)
Currency (VARCHAR)
Amount (DECIMAL)
AmountUSD (DECIMAL)
ExchangeRate (DECIMAL)
Participants (TEXT) - comma-separated emails
DocumentUrl (TEXT)
Notes (TEXT)
TripId (FK to Trips) - link to internal trip
DataValidation (BOOLEAN) - flag for validation needed
ParticipantsValidated (BOOLEAN)
IsValid (BOOLEAN)
CreatedAt (DATETIME)
ModifiedAt (DATETIME)
ModifiedBy (VARCHAR)
Trips Table
TripId (INTEGER PRIMARY KEY)
TripName (VARCHAR)
Email (VARCHAR)
StartDate (DATE)
EndDate (DATE)
Duration (INTEGER) - calculated in days
Country1 (VARCHAR)
City1 (VARCHAR)
Country2 (VARCHAR) - optional
City2 (VARCHAR) - optional
PurposeId (FK to Purposes)
TripTypeId (FK to TripTypes)
StatusId (FK to Status)
ValidationStatusId (FK to ValidationStatus)
IsManual (BOOLEAN)
OwnerId (FK to Owners)
CreatedAt (DATETIME)
ModifiedAt (DATETIME)
ModifiedBy (VARCHAR)
Owners Table
OwnerId (INTEGER PRIMARY KEY)
Name (VARCHAR)
Email (VARCHAR)
CostCenter (VARCHAR)
Department (VARCHAR)
Domain (VARCHAR)
Headcount Table
Period (DATE)
UserId (VARCHAR)
Email (VARCHAR)
FirstName (VARCHAR)
LastName (VARCHAR)
Subsidiary (VARCHAR)
Site (VARCHAR)
Department (VARCHAR)
Domain (VARCHAR)
CostCenter (VARCHAR)
Tax Table
FiscalYear (INTEGER)
Country (VARCHAR)
Subsidiary (VARCHAR)
MealsCap (DECIMAL)
LodgingCap (DECIMAL)
TaxShield (DECIMAL)
Countries & Cities (CSV import)
Country (VARCHAR)
City (VARCHAR)
Allow manual entry if not in list
Audit Log Table
AuditId (INTEGER PRIMARY KEY)
Timestamp (DATETIME)
UserId (VARCHAR)
Action (VARCHAR) - Create, Edit, Delete, Link, Unlink, Split
LinkedTable (VARCHAR) - Transactions, Trips
LinkedRecordId (VARCHAR)
OldValue (TEXT) - JSON
NewValue (TEXT) - JSON
Comments (TEXT)
________________________________________
3. APPLICATION STRUCTURE
Main Navigation
├── Reports
│   ├── Transactions (view, edit, delete, link to trip, split)
│   ├── Trips (view, edit, delete, view linked transactions)
│   ├── Create Manual Trip
│   ├── Trip Suggestions
│   ├── Trip Validation
│   └── Travel Spend (aggregate report)
│
├── Data Integrity
│   ├── Controls
│   │   ├── Airfare (cabin class validation)
│   │   ├── Meals (high-value validation)
│   │   ├── Lodging (low-value validation)
│   │   ├── Client Entertainment (participants validation)
│   │   └── Other (categorization validation)
│   │   └── Missing Documentation
│   ├── Matching Engine
│   │   ├── Manual Matching
│   │   └── Matching Suggestions
│   └── Split Engine
│       └── Split Suggestions
│
└── Settings
    ├── Lists (manage all dropdown values)
    ├── Data Transformation (import CSV rules)
    ├── Countries and Cities
    ├── Manage Quick Rules
    ├── Tax Settings
    ├── Owners
    └── Audit Log
________________________________________
4. KEY FEATURES & BUSINESS LOGIC
4.1 Transaction Management
Transaction Import (CSV)
•	Import from Navan, Agent, or Manual entry
•	Apply categorization rules based on policy field
•	Calculate USD amounts using exchange rates
•	Auto-detect participants from transaction data
•	Link to external trip ID if available
Transaction Table View
•	Show all transactions with filters
•	Editable columns: Category, Cabin Class, Participants, Notes
•	Actions: Edit, Delete, Link to Trip, Split Transaction, Mark as Valid
•	Double-click to open detail view
Split Transaction
•	User can split one transaction into multiple
•	Maintain original transaction ID reference
•	Each split gets own amount and category
•	Update audit log
4.2 Trip Management
Trip Creation
1.	Manual Trip Form
o	Enter all trip details manually
o	Select dates, countries, cities, purpose
o	Assign owner
2.	Trip Suggestions (Auto-detect)
o	Algorithm: Group transactions by email + date proximity (±2 days)
o	Look for airfare/lodging to identify trips
o	Suggest trip name: "Trip to [City/Country]"
o	User reviews and approves/edits suggestions
3.	Link Transactions to Trip
o	View unlinked transactions for a trip period
o	Select multiple transactions and link to trip
o	Unlink transactions if needed
Trip Validation
•	Show trips with ValidationStatus = "Ready to validate"
•	Display calculated metrics (see Travel Spend section)
•	Check against tax rules
•	Validate documentation completeness
•	Change status to "Validated" when approved
4.3 Data Integrity Controls
Airfare Control
Purpose: Ensure all airfare transactions have cabin class assigned
Display Logic:
•	WHERE Category = 'Airfare' AND CabinClass IS NULL
Table Columns:
•	Document (hyperlink)
•	Transaction ID
•	Email
•	Date
•	Vendor
•	Address
•	Currency
•	Amount
•	Amount (USD)
•	Cabin Class (EDITABLE - highlight)
•	Category (EDITABLE - highlight)
Features:
•	Order by: Email, Date
•	Filter by: Owner
•	Editable: Cabin Class, Category only
________________________________________
Meals Control
Purpose: Review high-value meal transactions (≥$80) for compliance
Display Logic:
•	WHERE Category = 'Meals'
•	AND ABS(AmountUSD) >= $80
•	AND IsValid = FALSE
Table Columns:
•	Document (hyperlink)
•	Transaction ID
•	Email
•	Date
•	Vendor
•	Address
•	Currency
•	Amount
•	Amount (USD)
•	Participants (EDITABLE)
•	Category (EDITABLE - highlight)
Actions:
1.	Mark as Valid (sets IsValid = TRUE)
2.	Update Category
3.	Generate Message Template (copy to clipboard)
Features:
•	Order by: Email, Date
•	Filter by: Owner, Amount USD (default ≥$80)
Message Templates: (Generate based on participants)
If External Participants Detected:
Hi [FirstName],

I have a question about the following travel-related transaction:

Transaction ID: [TransactionId]
Date: [Date in dd/MM/yyyy]
Category: Meals
Vendor: [Vendor]
Address: [Address]
Amount: [Currency] [Amount]
Participants: [comma-separated list]
Documentation: [DocumentUrl]

The system identified the following external participants: [external emails], while this transaction was marked as "Meals". If this was a meeting with external participants, please share all the participants in the meeting and the purpose.

Thank you!
If Internal Participants Only:
Hi [FirstName],

I have a question about the following travel-related transaction:

Transaction ID: [TransactionId]
Date: [Date in dd/MM/yyyy]
Category: Meals
Vendor: [Vendor]
Address: [Address]
Amount: [Currency] [Amount]
Participants: [comma-separated list]
Documentation: [DocumentUrl]

The system identified the following participants: [internal emails]. Can you please confirm the transaction was shared with them and you paid on their behalf as well?

Based on company policy, shared spend among WSC employees is not allowed. Each employee should pay for their own expenses.

Thank you!
If No Participants:
Hi [FirstName],

I have a question about the following travel-related transaction:

Transaction ID: [TransactionId]
Date: [Date in dd/MM/yyyy]
Category: Meals
Vendor: [Vendor]
Address: [Address]
Amount: [Currency] [Amount]
Documentation: [DocumentUrl]

Were there other participants (internal or external) in this meal? If so, could you please provide their email addresses?

Based on company policy, shared spend among WSC employees is not allowed. Each employee should pay for their own expenses.

Thank you!
________________________________________
Lodging Control
Purpose: Review unusually low lodging charges (≤$100)
Display Logic:
•	WHERE Category = 'Lodging'
•	AND ABS(AmountUSD) <= $100
•	AND IsValid = FALSE
Table Columns:
•	Document (hyperlink)
•	Transaction ID
•	Email
•	Date
•	Vendor
•	Address
•	Currency
•	Amount
•	Amount (USD)
•	Participants
•	Category (EDITABLE - highlight)
Actions:
1.	Mark as Valid (sets IsValid = TRUE)
2.	Update Category
Features:
•	Order by: Email, Date
•	Filter by: Owner, Amount USD (default ≤$100)
________________________________________
Client Entertainment Control
Purpose: Ensure client entertainment has participant information
Display Logic:
•	WHERE Category = 'Client entertainment'
•	AND ParticipantsValidated = FALSE
Table Columns:
•	Document (hyperlink)
•	Transaction ID
•	Email
•	Date
•	Vendor
•	Address
•	Currency
•	Amount
•	Amount (USD)
•	Participants (EDITABLE - highlight)
•	Category (EDITABLE)
Actions:
1.	Add Participants: 
o	Select internal employees from dropdown (Headcount table)
o	Add external participants as free text (email format)
o	Concatenate with commas
o	Set ParticipantsValidated = TRUE
2.	Update Category
3.	Generate Message Template
Message Template:
Hi [FirstName],

The following transaction was categorized as Client entertainment:

Transaction ID: [TransactionId]
Date: [Date in dd/MM/yyyy]
Category: Client entertainment
Vendor: [Vendor]
Address: [Address]
Amount: [Currency] [Amount]
Participants: [comma-separated list]
Documentation: [DocumentUrl]

The system identified the following external participants: [external emails].

To comply with tax reporting requirements, could you please provide the names and email addresses of all participants (both internal WSC employees and external customers/prospects)?

This information is required for proper documentation and may be needed in the event of a future tax audit.

Thank you!
Features:
•	Order by: Email, Date
•	Filter by: Owner
________________________________________
Other Control
Purpose: Categorize "Other" transactions properly
Display Logic:
•	WHERE Category = 'Other'
Table Columns:
•	Document (hyperlink)
•	Transaction ID
•	Email
•	Date
•	Vendor
•	Address
•	Currency
•	Amount
•	Amount (USD)
•	Category (EDITABLE - highlight)
Actions:
1.	Update Category
2.	Generate Message Template
Message Template:
Hi [FirstName],

I have a question about the following travel-related transaction:

Transaction ID: [TransactionId]
Date: [Date in dd/MM/yyyy]
Trip: [TripName], [StartDate] - [EndDate]
Category: Other
Vendor: [Vendor]
Address: [Address]
Amount: [Currency] [Amount]
Documentation: [DocumentUrl]

The system wasn't able to categorize this transaction and we need to select a proper category for tax purposes.

What is the nature of this transaction?

Thank you!
Features:
•	Order by: Email, Date
•	Filter by: Owner
________________________________________
Missing Documentation Control
Purpose: Flag transactions without receipts
Display Logic:
•	WHERE DocumentUrl IS NULL OR DocumentUrl = ''
Similar table structure to other controls
________________________________________
4.4 Tax Calculations
Tax Exposure Calculation (per trip):
Join Logic:
1.	Match TaxTable WHERE: 
o	YEAR(Trip.StartDate) = Tax.FiscalYear
o	Trip.Country1 = Tax.Country
o	Trip.Subsidiary = Tax.Subsidiary (e.g., "WSC IL")
Calculate Tax Exposure:
1.	Meals Exposure:
2.	IF (MealsPerDay > Tax.MealsCap):
3.	    Exposure = Duration × (MealsPerDay - MealsCap)
4.	ELSE:
5.	    Exposure = 0
6.	Lodging Exposure:
7.	IF (LodgingPerNight > Tax.LodgingCap):
8.	    Exposure = Duration × (LodgingPerNight - LodgingCap)
9.	ELSE:
10.	    Exposure = 0
11.	Airfare Exposure:
12.	IF CabinClass IN ('First', 'Business'):
13.	    Flag = TRUE
Total Tax Exposure = Meals Exposure + Lodging Exposure
________________________________________
4.5 Travel Spend Report
Purpose: Aggregate view of all trips with spending breakdown
Calculation Logic (for each trip):
# of Transactions = COUNT(linked transactions)
Total Amount ($) = SUM(AmountUSD for all linked transactions)
Cost per Day ($) = Total Amount / Duration

Airfare ($) = SUM(AmountUSD WHERE Category = 'Airfare')
Cabin Classes = DISTINCT(CabinClass for airfare transactions)

Lodging ($) = SUM(AmountUSD WHERE Category = 'Lodging')
Lodging per Night ($) = Lodging / Duration

Meals ($) = SUM(AmountUSD WHERE Category = 'Meals')
Meals per Day ($) = Meals / Duration

Transportation ($) = SUM(AmountUSD WHERE Category = 'Transportation')
Transportation per Day ($) = Transportation / Duration

Client Entertainment ($) = SUM(AmountUSD WHERE Category = 'Client entertainment')
Communication ($) = SUM(AmountUSD WHERE Category = 'Communication')
Other ($) = SUM(AmountUSD WHERE Category = 'Other')

Tax Exposure = Calculate using tax logic above
Table Columns:
•	All Trip table columns
•	of Transactions
•	Total Amount ($)
•	Cost per Day ($)
•	Airfare ($)
•	Cabin Classes
•	Lodging ($)
•	Lodging per Night ($)
•	Meals ($)
•	Meals per Day ($)
•	Transportation ($)
•	Transportation per Day ($)
•	Client Entertainment ($)
•	Communication ($)
•	Other ($)
•	Tax Exposure
•	Owner
•	Last Modified Date
•	Last Modified By
Interactions:
•	Double-click Trip Row → Show all linked transactions in detail view
•	Double-click Tax Exposure → Show calculation breakdown with tax caps
________________________________________
4.6 Matching Engine
Automatic Matching Suggestions:
•	Find transactions with SourceTripId
•	Find existing trips with matching external IDs
•	Suggest links with confidence score
•	User reviews and approves/rejects
Manual Matching:
•	User selects trip
•	System shows all transactions in date range (±5 days)
•	User selects transactions to link
________________________________________
4.7 Split Engine
Automatic Split Suggestions:
•	Detect transactions with multiple participants
•	Calculate split amount per person
•	Suggest creating split records
•	User reviews and approves
Manual Split:
•	User selects transaction
•	Enters number of splits
•	Assigns amounts and categories to each split
•	System creates new transaction records with reference to original
________________________________________
5. IMPORT & DATA TRANSFORMATION
CSV Import Features
1.	Navan Import
o	Parse CSV with specific column mapping
o	Apply transformation rules (see lines 201-282 in original doc)
o	Category mapping based on policy field
o	Exchange rate conversion
2.	Agent Import
o	Similar structure to Navan
o	Different column names
3.	Manual Import
o	Simple CSV template
o	Minimal required fields
Transformation Rules (Category Mapping)
Based on "Policy" field from CSV:

Policy Contains                          → Category
'tripactions_fees'                       → 'Trip fee'
'Airalo'                                 → 'Communication'
'public transport, tolls & parking'      → 'Transportation'
'Taxi & rideshare'                       → 'Transportation'
'Rental cars'                            → 'Transportation'
'Train travel'                           → 'Transportation'
'Fuel'                                   → 'Transportation'
'entertaining clients'                   → 'Client entertainment'
'team events & meals'                    → 'Meals'
'Meals for myself'                       → 'Meals'
'Airfare'                                → 'Airfare'
'Internet access'                        → 'Communication'
'telecommunication_services'             → 'Communication'
'Lodging'                                → 'Lodging'
'Software'                               → 'Other'
'Conference attendance'                  → 'Other'
Default                                  → 'Uncategorized'
This should be manageable from Settings → Quick Rules
________________________________________
6. UI/UX REQUIREMENTS
General UI Rules
1.	Editable fields must be visually highlighted (different background color)
2.	Tables must support: 
o	Column reordering (drag & drop)
o	Column resizing
o	Sorting (click header)
o	Filtering (dropdown or search box at top)
o	Save custom views
o	Export to CSV/Excel
3.	Forms should have clear Save/Cancel buttons
4.	Confirmations required for Delete operations
5.	Toast notifications for success/error messages
6.	Loading indicators during data operations
Specific Views
Transaction Detail View (Modal/Popup)
[Document Preview if available]

Transaction ID: [value]
Email: [value]
Date: [value]
Vendor: [value]
...

[Edit] [Delete] [Link to Trip] [Split] [Generate Message]
Trip Detail View
Trip Information Section:
- Name, Dates, Countries, Purpose, etc.

Linked Transactions Section:
[Table of all linked transactions]
[Link More Transactions] button

Tax Calculation Section:
- Meals Cap: [value] | Actual: [value] | Exposure: [value]
- Lodging Cap: [value] | Actual: [value] | Exposure: [value]
Total Tax Exposure: [value]
Settings → Lists Manager
Select List: [Dropdown: Sources, Categories, Purposes, ...]

[Table showing list items]
ID | Emoji | Name | [Edit] [Delete]

[Add New Item] button
________________________________________
7. SECURITY & AUDIT
Audit Log Requirements
Log every action:
•	Who: UserId/Email
•	What: Action type (Create, Edit, Delete, Link, Unlink, Split)
•	When: Timestamp
•	Where: Which table and record
•	Old Value: JSON snapshot before change
•	New Value: JSON snapshot after change
Restore Feature
•	View audit history for any record
•	Click "Restore" to revert to previous version
•	Exception: Cannot restore if transaction was split (data integrity)
________________________________________
8. VALIDATION RULES
Transaction Validation
•	Amount must be numeric
•	Date must be valid date
•	Email must be valid format
•	Currency must be 3-letter code
•	Document URL must be valid URL or empty
Trip Validation
•	Start Date must be before or equal to End Date
•	Duration = EndDate - StartDate + 1 (inclusive days)
•	At least one country required
•	Owner must be assigned
Tax Validation
•	Fiscal Year must be 4-digit year
•	All cap amounts must be positive numbers
•	Country must match Countries list
________________________________________
9. PERFORMANCE CONSIDERATIONS
Indexing
Create indexes on:
•	Transactions: Email, TransactionDate, CategoryId, TripId
•	Trips: Email, StartDate, EndDate, StatusId
•	AuditLog: Timestamp, LinkedTable, LinkedRecordId
Lazy Loading
•	Load large tables in pages (e.g., 100 rows at a time)
•	Implement virtual scrolling for very large datasets
Caching
•	Cache lookup tables (Categories, Purposes, etc.) in memory
•	Refresh only when Settings are modified
________________________________________
10. EXPORT & REPORTING
Export Options
1.	CSV Export - All tables exportable to CSV
2.	Excel Export - With formatting preserved
3.	PDF Report - For Travel Spend summary
Report Formats
•	Travel Spend: Aggregate table with charts
•	Audit Log: Chronological list with filters
•	Tax Compliance: Summary by employee/trip
________________________________________
11. ERROR HANDLING
Required Error Handling
•	Database connection failures
•	CSV import errors (malformed data)
•	File not found (documents)
•	Network errors (if fetching exchange rates)
•	Duplicate transaction IDs
•	Invalid date ranges
User-Friendly Messages
•	Don't show technical error details
•	Provide actionable steps
•	Log errors to file for debugging
________________________________________
12. TESTING CHECKLIST
Unit Tests
•	[ ] Tax calculation logic
•	[ ] Date formatting functions
•	[ ] Category mapping rules
•	[ ] Split transaction logic
•	[ ] Amount calculations
Integration Tests
•	[ ] CSV import with sample data
•	[ ] Link transaction to trip
•	[ ] Split transaction creates correct records
•	[ ] Audit log captures all changes
•	[ ] Filter and sort work on all tables
UI Tests
•	[ ] All forms validate correctly
•	[ ] Tables resize/reorder/sort
•	[ ] Message templates generate correctly
•	[ ] Confirmations work for deletes
•	[ ] Export functions produce valid files
________________________________________
13. DEPLOYMENT
Database Initialization
1.	Create all tables with schema
2.	Insert default lookup values
3.	Create indexes
4.	Set up audit triggers (optional)
First Run
1.	Import Countries & Cities CSV
2.	Import Tax rates CSV
3.	Import Headcount data
4.	Set up Owners
5.	Configure transformation rules
6.	Import historical transactions/trips
________________________________________
14. FUTURE ENHANCEMENTS
•	OCR for receipt scanning
•	Email integration for automatic message sending
•	Mobile app for expense submission
•	Real-time exchange rate API
•	Dashboard with charts and KPIs
•	Approval workflow
•	Budget tracking
•	Notifications and reminders
________________________________________
SUMMARY FOR COPILOT
Your Task: Build this system step by step
Start with:
1.	Database schema creation (SQLite)
2.	Entity Framework models
3.	Basic CRUD operations
4.	Main navigation structure
5.	Transaction table view
6.	Trip table view
7.	Then implement each control (Airfare, Meals, Lodging, etc.)
8.	Add Tax calculations
9.	Add Import functionality
10.	Add Audit logging
11.	Add Settings management
12.	Polish UI and add validations
Remember:
•	All dates display as dd/MM/yyyy
•	All amounts with thousand separators
•	Highlight editable fields
•	All tables must support: resize, reorder, sort, filter, save views
•	Log everything in Audit table
•	Generate correct message templates
•	Apply tax calculation logic correctly
Good luck! Ask me if you need clarification on any specific feature.