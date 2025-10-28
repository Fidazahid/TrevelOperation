# PowerShell script to create Notifications table in SQLite database
# Run this script from the TrevelOperation root directory

$ErrorActionPreference = "Stop"

Write-Host "=== Creating Notifications Table ===" -ForegroundColor Cyan

# Database paths to check
$dbPaths = @(
    ".\TrevelOperation\TravelOperations.db",
    ".\TrevelOperation\bin\Debug\net9.0-windows7.0\TravelOperations.db"
)

# Find the database
$dbPath = $null
foreach ($path in $dbPaths) {
    if (Test-Path $path) {
        $dbPath = Resolve-Path $path
        Write-Host "Found database at: $dbPath" -ForegroundColor Green
        break
    }
}

if (-not $dbPath) {
    Write-Host "ERROR: Could not find TravelOperations.db" -ForegroundColor Red
    Write-Host "Searched in:" -ForegroundColor Yellow
    foreach ($path in $dbPaths) {
        Write-Host "  - $path" -ForegroundColor Yellow
    }
    exit 1
}

# SQL to create the Notifications table
$sql = @"
CREATE TABLE IF NOT EXISTS "Notifications" (
    "NotificationId" INTEGER NOT NULL CONSTRAINT "PK_Notifications" PRIMARY KEY AUTOINCREMENT,
    "RecipientEmail" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "Priority" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "Message" TEXT NOT NULL,
    "ActionUrl" TEXT NULL,
    "ActionLabel" TEXT NULL,
    "RelatedEntityType" TEXT NULL,
    "RelatedEntityId" TEXT NULL,
    "Icon" TEXT NULL,
    "IsRead" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TEXT NOT NULL,
    "ReadAt" TEXT NULL,
    "ExpiresAt" TEXT NULL,
    "Metadata" TEXT NULL
);

CREATE INDEX IF NOT EXISTS "IX_Notifications_RecipientEmail_IsRead" ON "Notifications" ("RecipientEmail", "IsRead");
CREATE INDEX IF NOT EXISTS "IX_Notifications_CreatedAt" ON "Notifications" ("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Notifications_ExpiresAt" ON "Notifications" ("ExpiresAt");
"@

try {
    # Load System.Data.SQLite
    Write-Host "`nLoading SQLite library..." -ForegroundColor Yellow
    
    # Try to use Microsoft.Data.Sqlite via .NET
    Add-Type -AssemblyName System.Data
    
    $connectionString = "Data Source=$dbPath;Version=3;"
    
    # Use ADO.NET directly with SQLite
    Write-Host "Opening database connection..." -ForegroundColor Yellow
    
    # Create connection string for Microsoft.Data.Sqlite
    $assembly = [System.Reflection.Assembly]::LoadFrom("$PSScriptRoot\TrevelOperation\bin\Debug\net9.0-windows7.0\Microsoft.Data.Sqlite.dll")
    
    $connectionType = $assembly.GetType("Microsoft.Data.Sqlite.SqliteConnection")
    $connection = [Activator]::CreateInstance($connectionType, $connectionString)
    $connection.Open()
    
    Write-Host "Executing SQL to create Notifications table..." -ForegroundColor Yellow
    
    $commandType = $assembly.GetType("Microsoft.Data.Sqlite.SqliteCommand")
    $command = [Activator]::CreateInstance($commandType, $sql, $connection)
    $command.ExecuteNonQuery() | Out-Null
    
    Write-Host "`n✅ SUCCESS! Notifications table created successfully!" -ForegroundColor Green
    
    # Verify table exists
    $verifyCommand = [Activator]::CreateInstance($commandType, "SELECT name FROM sqlite_master WHERE type='table' AND name='Notifications';", $connection)
    $reader = $verifyCommand.ExecuteReader()
    
    if ($reader.Read()) {
        Write-Host "✅ Verification: Notifications table exists in database" -ForegroundColor Green
    } else {
        Write-Host "⚠ Warning: Could not verify table creation" -ForegroundColor Yellow
    }
    
    $reader.Close()
    $connection.Close()
    
    Write-Host "`nNext steps:" -ForegroundColor Cyan
    Write-Host "1. Close your application if it's running" -ForegroundColor White
    Write-Host "2. Restart the application" -ForegroundColor White
    Write-Host "3. Login as Employee and create a transaction (e.g., $1234 meal)" -ForegroundColor White
    Write-Host "4. Check console for debug messages" -ForegroundColor White
    Write-Host "5. Login as Finance (martina.popinsk@wsc.com) and check Notifications page" -ForegroundColor White
    
} catch {
    Write-Host "`n❌ ERROR: Failed to create Notifications table" -ForegroundColor Red
    Write-Host "Error details: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`nTrying alternative method using EF Core migration..." -ForegroundColor Yellow
    
    # Alternative: Use EF Core to update database
    Write-Host "`nExecuting: dotnet ef database update" -ForegroundColor Yellow
    Set-Location -Path ".\TrevelOperation"
    $result = & dotnet ef database update --project ..\TravelOperation.Core --verbose
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Database updated successfully via EF Core!" -ForegroundColor Green
    } else {
        Write-Host "❌ EF Core migration also failed" -ForegroundColor Red
        Write-Host "`nPlease check the error messages above and try:" -ForegroundColor Yellow
        Write-Host "1. Close all applications using the database" -ForegroundColor White
        Write-Host "2. Delete the database file and run migrations again" -ForegroundColor White
        Write-Host "3. Contact support if issue persists" -ForegroundColor White
    }
}

Write-Host "`n=== Script Complete ===" -ForegroundColor Cyan
