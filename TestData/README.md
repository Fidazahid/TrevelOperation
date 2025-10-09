# Test Data for CSV Import

This folder contains sample CSV files to test the import functionality.

## 📁 Files

### 1. Navan_Sample.csv
- **Format:** Navan expense system export
- **Records:** 10 transactions
- **Features tested:**
  - Multiple currencies (USD, GBP, EUR)
  - Various categories (Airfare, Lodging, Meals, Transportation, etc.)
  - Trip linkage (Trip IDs and Booking IDs)
  - Participant tracking (single and multiple)
  - Date format: yyyy-MM-dd
  - Exchange rate conversion

**Expected Transformation Results:**
- TXN001: Airfare → ✈ Airfare
- TXN002: Lodging → 🏨 Lodging
- TXN003: Taxi & rideshare → 🚕 Transportation
- TXN004: entertaining clients → 🍸 Client entertainment
- TXN005: Airfare → ✈ Airfare
- TXN006: Meals for myself → 🍽 Meals
- TXN007: telecommunication_services → 📱 Communication
- TXN008: Rental cars → 🚕 Transportation
- TXN009: Lodging → 🏨 Lodging
- TXN010: Conference attendance → ❔ Other

### 2. Agent_Sample.csv
- **Format:** Travel agent export
- **Records:** 8 transactions
- **Features tested:**
  - Simplified field mapping
  - Date format: dd/MM/yyyy
  - European currencies (EUR)
  - Public transport categories

**Expected Transformation Results:**
- AGT001: Airfare → ✈ Airfare
- AGT002: Lodging → 🏨 Lodging
- AGT003: Taxi & rideshare → 🚕 Transportation
- AGT004: Meals for myself → 🍽 Meals
- AGT005: Airfare → ✈ Airfare
- AGT006: public transport, tolls & parking → 🚕 Transportation
- AGT007: Internet access → 📱 Communication
- AGT008: Fuel → 🚕 Transportation

### 3. Manual_Sample.csv
- **Format:** Manual entry template
- **Records:** 6 transactions
- **Features tested:**
  - Minimal required fields
  - User-friendly column names
  - Date format: dd/MM/yyyy

**Expected Transformation Results:**
- MAN001: Airfare → ✈ Airfare
- MAN002: Lodging → 🏨 Lodging
- MAN003: Taxi & rideshare → 🚕 Transportation
- MAN004: entertaining clients → 🍸 Client entertainment
- MAN005: telecommunication_services → 📱 Communication
- MAN006: Rental cars → 🚕 Transportation

## 🧪 Testing Steps

### 1. Access CSV Import Page
- Navigate to **Settings → CSV Import** or go to `/settings/csv-import`

### 2. Test Navan Import
1. Click on **Navan Import** card
2. Upload `Navan_Sample.csv`
3. Click **Import** button
4. Verify results:
   - ✅ Success: 10 transactions imported
   - ✅ Categories assigned correctly via transformation rules
   - ✅ USD amounts calculated (GBP: ×1.25, EUR: needs exchange rate)
   - ✅ Participants parsed correctly
   - ✅ Trip IDs linked (TRIP001, TRIP002, TRIP003)

### 3. Test Agent Import
1. Click on **Agent Import** card
2. Upload `Agent_Sample.csv`
3. Click **Import** button
4. Verify results:
   - ✅ Success: 8 transactions imported
   - ✅ Date format dd/MM/yyyy parsed correctly
   - ✅ Categories assigned via transformation rules
   - ✅ EUR amounts converted to USD

### 4. Test Manual Import
1. Click on **Manual Import** card
2. Upload `Manual_Sample.csv`
3. Click **Import** button
4. Verify results:
   - ✅ Success: 6 transactions imported
   - ✅ Minimal fields handled correctly
   - ✅ Categories assigned via transformation rules

### 5. Verify in Transactions Page
- Navigate to **Reports → Transactions**
- Search for imported transactions by Transaction ID
- Verify all data is correct:
  - Categories have emojis
  - Amounts are formatted correctly
  - Dates display as dd/MM/yyyy
  - Participants are listed

## 🎯 Expected Transformation Rules Applied

Based on the default 16 transformation rules:

| Policy Pattern | Category | Priority |
|---------------|----------|----------|
| Airfare | ✈ Airfare | 80 |
| Lodging | 🏨 Lodging | 80 |
| Taxi & rideshare | 🚕 Transportation | 80 |
| Rental cars | 🚕 Transportation | 80 |
| Fuel | 🚕 Transportation | 80 |
| public transport, tolls & parking | 🚕 Transportation | 80 |
| Train travel | 🚕 Transportation | 80 |
| Meals for myself | 🍽 Meals | 70 |
| team events & meals | 🍽 Meals | 70 |
| entertaining clients | 🍸 Client entertainment | 70 |
| Internet access | 📱 Communication | 60 |
| telecommunication_services | 📱 Communication | 60 |
| Airalo | 📱 Communication | 90 |
| Conference attendance | ❔ Other | 50 |
| Software | ❔ Other | 50 |

## ❌ Testing Error Handling

Create invalid CSV files to test error handling:
1. Missing required columns
2. Invalid date formats
3. Invalid email formats
4. Malformed CSV (wrong delimiter, missing quotes)

## 📊 Export Testing

After importing:
1. Navigate to **Reports → Transactions**
2. Filter imported transactions
3. Click **Export CSV** - verify formatting
4. Click **Export Excel** - verify formatting and styling
5. Check date format: dd/MM/yyyy
6. Check amount format: 1,000.00

## 🔍 Validation Checks

Post-import, verify these appear in Data Integrity controls:
- **Airfare Control:** Any airfare without cabin class
- **Meals Control:** Meals ≥$80 (e.g., TXN004: $425.00)
- **Lodging Control:** Lodging ≤$100 (e.g., TXN009: $75.00)
- **Client Entertainment Control:** Transactions missing participants
- **Other Control:** Any transactions categorized as "Other"

## 📝 Notes

- All sample data uses WSC email domain (@wsc.com)
- Dates range from August to October 2024
- Mix of domestic (USD) and international (GBP, EUR) transactions
- Various transaction types to test all transformation rules
