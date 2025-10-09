# Phase 3 Testing Guide - Data Integrity Engines

**Date:** January 2025  
**Application Status:** ✅ Running  
**Features to Test:** Items 17, 18, 19, 26

---

## 🎯 Testing Objective

Verify that Phase 3 features (Matching Engine, Split Engine, Audit Log) work correctly with real data.

---

## 📋 Pre-Testing Setup

### Step 1: Import Sample Data

**Location:** Settings → CSV Import

Import these files in order:

1. **Navan_Sample.csv** (10 transactions)
   - Multiple currencies (USD, EUR, ILS, GBP)
   - Various categories (Airfare, Lodging, Meals, Transportation)
   - Contains trip linkage data
   - Participants included

2. **Agent_Sample.csv** (8 transactions)
   - European travel focused
   - Different date format (dd/MM/yyyy)
   - Includes high-value transactions

3. **Manual_Sample.csv** (6 transactions)
   - Minimal field data
   - Tests basic import functionality

**Expected Result:** Total of 24 transactions imported successfully

---

### Step 2: Create Test Trips

**Location:** Reports → Create Manual Trip

Create at least 2-3 trips matching the imported transaction dates:

**Suggested Trip 1:**
- Trip Name: "NYC Business Trip - John Smith"
- Email: john.smith@wsc.com
- Start Date: 15/01/2024
- End Date: 20/01/2024
- Country: United States
- City: New York
- Purpose: Business trip
- Trip Type: International

**Suggested Trip 2:**
- Trip Name: "London Conference - Sarah Johnson"
- Email: sarah.johnson@wsc.com
- Start Date: 05/02/2024
- End Date: 08/02/2024
- Country: United Kingdom
- City: London
- Purpose: Business trip
- Trip Type: International

**Suggested Trip 3:**
- Trip Name: "Tel Aviv Client Meeting"
- Email: david.cohen@wsc.com
- Start Date: 10/03/2024
- End Date: 12/03/2024
- Country: Israel
- City: Tel Aviv
- Purpose: Client entertainment
- Trip Type: International

---

## 🧪 Test Cases

### Test Case 1: Matching Engine - Automatic Suggestions

**Location:** Data Integrity → Matching → Automatic Suggestions Tab

**Steps:**
1. Click "Refresh" button
2. Observe statistics dashboard:
   - Total Transactions count
   - Unlinked Transactions count
   - Total Trips count
   - Pending Suggestions count

3. Review each suggestion card:
   - Check Trip ID and Name display
   - Verify Email, Start/End dates
   - Check Overall Confidence percentage and badge color
   - Check Total Amount calculation

4. Expand transaction details:
   - Verify Transaction ID, Date, Category
   - Check Vendor and Amount (USD)
   - Verify Confidence score per transaction
   - Read Matching Reason explanations

5. Test "Link All" button:
   - Click on a suggestion
   - Confirm the dialog
   - Verify success message shows correct count
   - Check that suggestion is removed after linking

6. Test individual transaction linking:
   - Click "Link" on a specific transaction
   - Confirm the dialog
   - Verify success message
   - Check transaction is marked as "Already Linked"

**Expected Results:**
- ✅ Suggestions displayed with confidence scores
- ✅ High confidence (80%+) shown in green badge
- ✅ Medium confidence (50-79%) shown in yellow badge
- ✅ Transactions link successfully
- ✅ Linked transactions show "Already Linked" badge
- ✅ Statistics update after linking

**Success Criteria:**
- At least 1 suggestion with confidence ≥ 60%
- All "Link All" operations complete without errors
- Audit log captures all linking actions

---

### Test Case 2: Matching Engine - Manual Search

**Location:** Data Integrity → Matching → Manual Search Tab

**Steps:**
1. Switch to "Manual Search" tab
2. Enter a valid Trip ID from your created trips
3. Set Days Tolerance to 5 (default)
4. Optional: Enter email filter
5. Click "Search" button

6. Review search results table:
   - Check "Found X Transactions" message
   - Verify Transaction ID, Email, Date
   - Check Category, Vendor, Amount (USD)
   - Check Current Trip status

7. Test linking unlinked transaction:
   - Click "Link to Trip" on unlinked transaction
   - Confirm dialog
   - Verify success message

8. Test unlinking transaction:
   - Find a linked transaction (Trip badge shown)
   - Click "Unlink" button
   - Confirm dialog
   - Verify success message
   - Check transaction now shows "Unlinked" badge

9. Test different tolerances:
   - Try Days Tolerance = 0 (exact match)
   - Try Days Tolerance = 10 (wider range)
   - Compare result counts

10. Test email filter:
    - Enter partial email
    - Verify results filtered correctly

11. Test "Clear" button:
    - Click "Clear"
    - Verify all fields reset
    - Verify results table cleared

**Expected Results:**
- ✅ Search returns transactions within date range
- ✅ Link/Unlink operations work correctly
- ✅ Filters work as expected
- ✅ Clear button resets form

**Success Criteria:**
- Search returns expected transaction count based on tolerance
- All link/unlink operations complete without errors
- Audit log captures all operations

---

### Test Case 3: Split Engine

**Location:** Data Integrity → Split Engine

**Steps:**
1. Observe statistics dashboard:
   - Split Suggestions count
   - Total Amount
   - Total Participants
   - High Confidence count

2. Test filters:
   - Category dropdown (select "Client entertainment")
   - Confidence dropdown (select "High (80%+)")
   - Amount Range dropdown (select "High ($200+)")
   - Search textbox (enter vendor name)
   - Click filter and verify results update

3. Review suggestion table:
   - Check Transaction ID and Date display
   - Verify Vendor and Address
   - Check Category badge with emoji
   - Verify Amount display
   - Check Participant badges (first 3 + count)
   - Verify Split Suggestion preview
   - Check Confidence radial progress (color-coded)

4. Test multi-select:
   - Click "Select All" checkbox
   - Verify all visible items selected
   - Uncheck "Select All"
   - Select 2-3 individual suggestions
   - Verify selected count shown

5. Test per-row actions:
   - Click ⋮ (actions menu)
   - Try "View Details" (should show navigation message)
   - Try "Edit Split" (should open modal)
   - Try "Accept Split" (confirm dialog)
   - Try "Reject" (confirm removal)

6. Test bulk actions (with selections):
   - Select multiple suggestions
   - Click "Accept Selected"
   - Verify confirmation dialog shows correct count
   - Confirm and check success message
   - Try "Reject Selected"
   - Try "Clear Selection"

7. Test "Process All":
   - Click "Process All" button
   - Verify confirmation shows total count
   - Confirm and wait for processing
   - Check success/error summary

8. Test pagination:
   - Navigate to page 2 (if available)
   - Verify items displayed correctly
   - Check page numbers highlight correctly

**Expected Results:**
- ✅ Filters work correctly
- ✅ Multi-select and bulk actions function
- ✅ Split processing creates new transaction records
- ✅ Confidence scores display correctly
- ✅ Pagination works smoothly

**Success Criteria:**
- At least 1 split suggestion with multiple participants
- Accept operations create split transactions
- Audit log captures all split operations
- Original transaction marked as IsSplit = true

---

### Test Case 4: Audit Log

**Location:** Audit Log (main menu)

**Steps:**
1. Observe statistics:
   - Total Entries
   - Today's entries
   - This Week's entries
   - This Month's entries

2. Test filters:
   - Enter search term in textbox
   - Press Enter or click "Filter"
   - Select Action dropdown (e.g., "Link")
   - Select Table dropdown (e.g., "Transactions")
   - Select User dropdown
   - Set Start Date and End Date
   - Click "Filter" button

3. Test sorting:
   - Click "Timestamp" column header
   - Verify sort indicator (↑ or ↓)
   - Click again to reverse sort
   - Try sorting by User, Action, Table

4. Review table data:
   - Check Timestamp format (dd/MM/yyyy HH:mm:ss)
   - Verify User badge display
   - Check Action badge colors:
     - Green: Create/Added
     - Yellow: Edit/Modified
     - Red: Delete/Deleted
     - Blue: Restore
   - Check Table badge (accent color)
   - Verify Record ID display
   - Read Comments

5. Test "View" button:
   - Click "View" on an entry
   - Verify modal opens with full details
   - Check JSON formatting for Old Value
   - Check JSON formatting for New Value
   - Verify syntax highlighting

6. Test "Restore" button:
   - Find entry with "Restore" button visible
   - Click "Restore"
   - Check validation message (if applicable)
   - Confirm restoration dialog
   - Verify success message
   - Check data restored correctly

7. Test Export to CSV:
   - Click "Export CSV" button
   - Verify file downloads
   - Open CSV and check:
     - Headers: AuditId, Timestamp, UserId, Action, LinkedTable, LinkedRecordId, Comments
     - Date format: dd/MM/yyyy HH:mm:ss
     - Proper CSV escaping

8. Test pagination:
   - Navigate pages (20 items per page default)
   - Verify "Showing X to Y of Z entries" message
   - Test Previous/Next buttons

9. Test "Refresh" button:
   - Make a change elsewhere (link transaction, etc.)
   - Come back to Audit Log
   - Click "Refresh"
   - Verify new entry appears

**Expected Results:**
- ✅ All filters work correctly
- ✅ Sorting functions properly
- ✅ Details modal shows formatted JSON
- ✅ Restore functionality works (with validation)
- ✅ CSV export generates valid file
- ✅ Real-time updates with refresh

**Success Criteria:**
- Every action from previous tests appears in audit log
- JSON values are properly formatted and readable
- Restore works for eligible records
- Cannot restore split transactions
- CSV export contains all expected data

---

## 📊 Overall Success Metrics

After completing all tests, verify:

### Data Integrity:
- ✅ All transactions have audit trail
- ✅ No orphaned records
- ✅ Split transactions reference original
- ✅ Linked transactions show correct TripId

### Performance:
- ✅ Page loads under 2 seconds
- ✅ Filters/sorts respond instantly
- ✅ Bulk operations complete in reasonable time
- ✅ No UI freezing during operations

### User Experience:
- ✅ Clear success/error messages
- ✅ Confirmation dialogs for destructive actions
- ✅ Loading indicators during operations
- ✅ Statistics update after changes

### Code Quality:
- ✅ No console errors (check browser F12)
- ✅ No unhandled exceptions
- ✅ Proper null handling
- ✅ Clean audit log entries

---

## 🐛 Known Issues to Watch For

1. **Matching Engine:**
   - Confidence calculation edge cases
   - Date timezone issues
   - Null category names

2. **Split Engine:**
   - Participant email validation
   - Amount distribution rounding
   - Modal not opening

3. **Audit Log:**
   - JSON parsing errors
   - Restore validation false positives
   - CSV export encoding issues

---

## 📝 Testing Checklist

Use this checklist while testing:

### Matching Engine - Automatic
- [ ] Statistics display correctly
- [ ] Suggestions load without errors
- [ ] Confidence scores calculated correctly
- [ ] Transaction details expand/collapse
- [ ] Link All button works
- [ ] Individual link buttons work
- [ ] Already linked transactions marked
- [ ] Refresh updates suggestions

### Matching Engine - Manual
- [ ] Trip ID dropdown/input works
- [ ] Email filter works
- [ ] Days tolerance slider works
- [ ] Search button returns results
- [ ] Results show correct transactions
- [ ] Link to Trip button works
- [ ] Unlink button works
- [ ] Clear button resets form

### Split Engine
- [ ] Statistics display correctly
- [ ] All filters work
- [ ] Search filter works
- [ ] Suggestions display with all fields
- [ ] Confidence radial progress shows
- [ ] Participant badges display
- [ ] Select All checkbox works
- [ ] Individual checkboxes work
- [ ] Actions menu opens
- [ ] Edit Split modal opens
- [ ] Accept Split works
- [ ] Reject works
- [ ] Accept Selected works
- [ ] Reject Selected works
- [ ] Clear Selection works
- [ ] Process All works
- [ ] Pagination works

### Audit Log
- [ ] Statistics display correctly
- [ ] Search filter works
- [ ] Action filter works
- [ ] Table filter works
- [ ] User filter works
- [ ] Date range filter works
- [ ] Apply Filters button works
- [ ] Column sorting works
- [ ] Sort indicators display
- [ ] View button opens modal
- [ ] JSON formatted correctly
- [ ] Restore button works
- [ ] Restore validation works
- [ ] Export CSV works
- [ ] CSV format correct
- [ ] Pagination works
- [ ] Refresh button works

---

## 🎉 Completion Criteria

Testing is complete when:
1. ✅ All 24 sample transactions imported
2. ✅ At least 3 trips created
3. ✅ At least 5 transactions linked to trips
4. ✅ At least 1 split transaction processed
5. ✅ Audit log contains 10+ entries
6. ✅ All checkboxes above marked
7. ✅ No critical bugs found
8. ✅ User experience is smooth

---

**Next Steps After Testing:**
- Document any bugs found
- Update PROJECT_TASKS.md if features incomplete
- Proceed to Phase 4: Dashboard & Reporting
- Or continue with remaining Priority items
