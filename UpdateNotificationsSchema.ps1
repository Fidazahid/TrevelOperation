Write-Host "🔧 Updating Notifications table schema..." -ForegroundColor Cyan

# Load SQLite assembly
Add-Type -Path "$env:USERPROFILE\.nuget\packages\microsoft.data.sqlite.core\8.0.0\lib\net8.0\Microsoft.Data.Sqlite.dll" -ErrorAction SilentlyContinue

# If that fails, try to use System.Data.SQLite or download the assembly
if (-not ([System.Management.Automation.PSTypeName]'Microsoft.Data.Sqlite.SqliteConnection').Type) {
    Write-Host "⚠️ SQLite library not found. Trying alternative method..." -ForegroundColor Yellow
    
    # Try using sqlite3.exe if available
    $sqlite3Path = Get-Command sqlite3.exe -ErrorAction SilentlyContinue
    
    if ($null -eq $sqlite3Path) {
        Write-Host "❌ SQLite tools not available. Installing via dotnet tool..." -ForegroundColor Red
        Write-Host ""
        Write-Host "Please run the SQL script manually on your database:" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "ALTER TABLE Notifications ADD COLUMN CreatedByEmail TEXT;" -ForegroundColor Green
        Write-Host "ALTER TABLE Notifications ADD COLUMN CreatedByName TEXT;" -ForegroundColor Green
        Write-Host ""
        Write-Host "You can use DB Browser for SQLite or any SQLite management tool." -ForegroundColor Yellow
        exit 1
    }
}

# Database paths to update
$dbPaths = @(
    "c:\Users\imran\source\repos\dawloom\TrevelOperation\TrevelOperation\TravelOperations.db",
    "c:\Users\imran\source\repos\dawloom\TrevelOperation\TravelOperation.Core\TravelOperations.db",
    "c:\Users\imran\source\repos\dawloom\TrevelOperation\TrevelOperation\TravelExpense.db",
    "c:\Users\imran\source\repos\dawloom\TrevelOperation\TravelOperation.Core\TravelExpense.db"
)

foreach ($dbPath in $dbPaths) {
    if (Test-Path $dbPath) {
        try {
            Write-Host "📁 Processing: $dbPath" -ForegroundColor White
            
            $connectionString = "Data Source=$dbPath"
            $connection = New-Object Microsoft.Data.Sqlite.SqliteConnection($connectionString)
            $connection.Open()
            
            # Check if columns exist
            $checkCmd = $connection.CreateCommand()
            $checkCmd.CommandText = "PRAGMA table_info(Notifications)"
            $reader = $checkCmd.ExecuteReader()
            
            $hasCreatedByEmail = $false
            $hasCreatedByName = $false
            
            while ($reader.Read()) {
                $columnName = $reader.GetString(1)
                if ($columnName -eq "CreatedByEmail") { $hasCreatedByEmail = $true }
                if ($columnName -eq "CreatedByName") { $hasCreatedByName = $true }
            }
            $reader.Close()
            
            # Add CreatedByEmail column if it doesn't exist
            if (-not $hasCreatedByEmail) {
                $cmd1 = $connection.CreateCommand()
                $cmd1.CommandText = "ALTER TABLE Notifications ADD COLUMN CreatedByEmail TEXT;"
                $cmd1.ExecuteNonQuery() | Out-Null
                Write-Host "  ✓ Added CreatedByEmail column" -ForegroundColor Green
            } else {
                Write-Host "  ℹ️ CreatedByEmail column already exists" -ForegroundColor Gray
            }
            
            # Add CreatedByName column if it doesn't exist
            if (-not $hasCreatedByName) {
                $cmd2 = $connection.CreateCommand()
                $cmd2.CommandText = "ALTER TABLE Notifications ADD COLUMN CreatedByName TEXT;"
                $cmd2.ExecuteNonQuery() | Out-Null
                Write-Host "  ✓ Added CreatedByName column" -ForegroundColor Green
            } else {
                Write-Host "  ℹ️ CreatedByName column already exists" -ForegroundColor Gray
            }
            
            $connection.Close()
            Write-Host "  ✅ Database updated successfully" -ForegroundColor Green
            Write-Host ""
            
        } catch {
            Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
            Write-Host ""
        }
    } else {
        Write-Host "⚠️ Not found: $dbPath" -ForegroundColor Yellow
    }
}

Write-Host "✅ Schema update complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
