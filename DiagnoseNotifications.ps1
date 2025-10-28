# Notification System Diagnostic Script
# Run this to diagnose why notifications aren't working

Write-Host "🔔 NOTIFICATION SYSTEM DIAGNOSTICS" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

$dbPath = ".\TravelOperations.db"

if (-not (Test-Path $dbPath)) {
    Write-Host "❌ ERROR: Database file not found at: $dbPath" -ForegroundColor Red
    Write-Host "   Please ensure you're running this from the project root directory" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Database found: $dbPath" -ForegroundColor Green
Write-Host ""

# Check 1: Notifications Table Exists
Write-Host "📊 CHECK 1: Notifications Table" -ForegroundColor Yellow
Write-Host "================================" -ForegroundColor Yellow
$notifTableCheck = sqlite3 $dbPath "SELECT name FROM sqlite_master WHERE type='table' AND name='Notifications';"
if ($notifTableCheck -eq "Notifications") {
    Write-Host "✅ Notifications table EXISTS" -ForegroundColor Green
    
    # Count notifications
    $notifCount = sqlite3 $dbPath "SELECT COUNT(*) FROM Notifications;"
    Write-Host "   Total notifications in database: $notifCount" -ForegroundColor Cyan
    
    if ($notifCount -gt 0) {
        Write-Host ""
        Write-Host "   Recent notifications:" -ForegroundColor Cyan
        sqlite3 $dbPath -header -column "SELECT NotificationId, RecipientEmail, Title, IsRead, CreatedAt FROM Notifications ORDER BY CreatedAt DESC LIMIT 5;"
    }
} else {
    Write-Host "❌ Notifications table DOES NOT EXIST" -ForegroundColor Red
    Write-Host "   Fix: Run the application once to auto-create the table" -ForegroundColor Yellow
}
Write-Host ""

# Check 2: Finance Users in Headcount
Write-Host "👥 CHECK 2: Finance Users in Headcount Table" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Yellow
$headcountExists = sqlite3 $dbPath "SELECT name FROM sqlite_master WHERE type='table' AND name='Headcount';"
if ($headcountExists -eq "Headcount") {
    $financeUsers = sqlite3 $dbPath "SELECT Email, Department FROM Headcount WHERE Department LIKE '%Finance%' OR Department LIKE '%Accounting%';"
    if ($financeUsers) {
        Write-Host "✅ Finance users found in Headcount:" -ForegroundColor Green
        sqlite3 $dbPath -header -column "SELECT Email, FirstName, LastName, Department FROM Headcount WHERE Department LIKE '%Finance%' OR Department LIKE '%Accounting%';"
    } else {
        Write-Host "⚠️  NO Finance users found in Headcount table" -ForegroundColor Yellow
        Write-Host "   Checking Employees table..." -ForegroundColor Cyan
        
        $employeesExists = sqlite3 $dbPath "SELECT name FROM sqlite_master WHERE type='table' AND name='Employees';"
        if ($employeesExists -eq "Employees") {
            $financeEmps = sqlite3 $dbPath "SELECT Email FROM Employees WHERE Department = 'Finance' AND IsActive = 1;"
            if ($financeEmps) {
                Write-Host "✅ Finance users found in Employees table:" -ForegroundColor Green
                sqlite3 $dbPath -header -column "SELECT Email, FirstName, LastName, Department FROM Employees WHERE Department = 'Finance' AND IsActive = 1;"
            } else {
                Write-Host "⚠️  NO Finance users found in Employees table either" -ForegroundColor Yellow
                Write-Host "   Will use hardcoded fallback emails:" -ForegroundColor Cyan
                Write-Host "   - martina.popinsk@wsc.com" -ForegroundColor Gray
                Write-Host "   - maayan.chesler@wsc.com" -ForegroundColor Gray
            }
        }
    }
} else {
    Write-Host "⚠️  Headcount table DOES NOT EXIST" -ForegroundColor Yellow
}
Write-Host ""

# Check 3: Users Table
Write-Host "🔐 CHECK 3: Users Table" -ForegroundColor Yellow
Write-Host "=======================" -ForegroundColor Yellow
$usersExists = sqlite3 $dbPath "SELECT name FROM sqlite_master WHERE type='table' AND name='Users';"
if ($usersExists -eq "Users") {
    Write-Host "✅ Users table exists" -ForegroundColor Green
    $userCount = sqlite3 $dbPath "SELECT COUNT(*) FROM Users;"
    Write-Host "   Total users: $userCount" -ForegroundColor Cyan
    
    $financeRoleUsers = sqlite3 $dbPath "SELECT Email, Role FROM Users WHERE Role = 'Finance';"
    if ($financeRoleUsers) {
        Write-Host ""
        Write-Host "   Finance role users:" -ForegroundColor Cyan
        sqlite3 $dbPath -header -column "SELECT Email, FirstName, LastName, Role FROM Users WHERE Role = 'Finance';"
    } else {
        Write-Host "   ⚠️  NO users with 'Finance' role found" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ Users table DOES NOT EXIST" -ForegroundColor Red
}
Write-Host ""

# Check 4: Transactions Table
Write-Host "💳 CHECK 4: Transactions Table" -ForegroundColor Yellow
Write-Host "==============================" -ForegroundColor Yellow
$transExists = sqlite3 $dbPath "SELECT name FROM sqlite_master WHERE type='table' AND name='Transactions';"
if ($transExists -eq "Transactions") {
    Write-Host "✅ Transactions table exists" -ForegroundColor Green
    $transCount = sqlite3 $dbPath "SELECT COUNT(*) FROM Transactions;"
    Write-Host "   Total transactions: $transCount" -ForegroundColor Cyan
    
    if ($transCount -gt 0) {
        Write-Host ""
        Write-Host "   Recent transactions:" -ForegroundColor Cyan
        sqlite3 $dbPath -header -column "SELECT TransactionId, Email, TransactionDate, CategoryId, AmountUSD FROM Transactions ORDER BY CreatedAt DESC LIMIT 5;"
    }
} else {
    Write-Host "❌ Transactions table DOES NOT EXIST" -ForegroundColor Red
}
Write-Host ""

# Check 5: Categories Table
Write-Host "📂 CHECK 5: Categories Table" -ForegroundColor Yellow
Write-Host "============================" -ForegroundColor Yellow
$catExists = sqlite3 $dbPath "SELECT name FROM sqlite_master WHERE type='table' AND name='Categories';"
if ($catExists -eq "Categories") {
    Write-Host "✅ Categories table exists" -ForegroundColor Green
    Write-Host ""
    Write-Host "   Available categories:" -ForegroundColor Cyan
    sqlite3 $dbPath -header -column "SELECT CategoryId, Name FROM Categories ORDER BY Name;"
} else {
    Write-Host "❌ Categories table DOES NOT EXIST" -ForegroundColor Red
}
Write-Host ""

# Summary
Write-Host "📋 SUMMARY & RECOMMENDATIONS" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan
Write-Host ""

# Issue 1: No Notifications Table
if ($notifTableCheck -ne "Notifications") {
    Write-Host "❌ ISSUE 1: Notifications table missing" -ForegroundColor Red
    Write-Host "   FIX: Run the application once. It will auto-create the table." -ForegroundColor Yellow
    Write-Host ""
}

# Issue 2: No Finance Users
if (-not $financeUsers -and -not $financeEmps) {
    Write-Host "⚠️  ISSUE 2: No Finance users in database" -ForegroundColor Yellow
    Write-Host "   FIX: Add Finance users to Headcount or Employees table" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "   SQL to add Finance users:" -ForegroundColor Cyan
    Write-Host @"
   
   INSERT INTO Headcount (Period, UserId, Email, FirstName, LastName, Department, Domain, CostCenter)
   VALUES 
   ('2024-01-01', 'user1', 'martina.popinsk@wsc.com', 'Martina', 'Popinsk', 'Finance', 'wsc.com', 'FIN001'),
   ('2024-01-01', 'user2', 'maayan.chesler@wsc.com', 'Maayan', 'Chesler', 'Finance', 'wsc.com', 'FIN001');
   
"@ -ForegroundColor Gray
    Write-Host "   OR the system will use hardcoded fallback emails." -ForegroundColor Green
    Write-Host ""
}

# Issue 3: Transaction Thresholds
Write-Host "ℹ️  INFO: Transaction Notification Thresholds" -ForegroundColor Cyan
Write-Host "   Finance team is notified ONLY if transaction amount exceeds:" -ForegroundColor White
Write-Host "   - Meals: ≥ `$80 USD" -ForegroundColor Gray
Write-Host "   - Lodging: ≥ `$100 USD" -ForegroundColor Gray
Write-Host "   - Client Entertainment: ≥ `$50 USD" -ForegroundColor Gray
Write-Host "   - Other categories: ≥ `$200 USD" -ForegroundColor Gray
Write-Host ""
Write-Host "   If you want ALL transactions to notify Finance, edit:" -ForegroundColor Yellow
Write-Host "   TravelOperation.Core\Services\TransactionService.cs (Line ~270)" -ForegroundColor Gray
Write-Host ""

# Test recommendation
Write-Host "🧪 RECOMMENDED TEST:" -ForegroundColor Green
Write-Host "===================" -ForegroundColor Green
Write-Host "1. Login to the application" -ForegroundColor White
Write-Host "2. Create a new Meals transaction with amount ≥ `$80 USD" -ForegroundColor White
Write-Host "3. Check console output for:" -ForegroundColor White
Write-Host "   [NotificationService] ===== CREATE NOTIFICATION =====" -ForegroundColor Gray
Write-Host "   [NotifyFinanceTeamAsync] Found X Finance users" -ForegroundColor Gray
Write-Host "4. Login as Finance user and navigate to /notifications" -ForegroundColor White
Write-Host "5. Notification should appear" -ForegroundColor White
Write-Host ""

Write-Host "✅ Diagnostics Complete!" -ForegroundColor Green
Write-Host ""
