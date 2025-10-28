# PowerShell script to create Notifications table
# Run this from the TrevelOperation project directory

$dbPath = ".\TravelOperations.db"

# Check if database exists
if (-not (Test-Path $dbPath)) {
    Write-Host "Database not found at: $dbPath" -ForegroundColor Red
    Write-Host "Please update the `$dbPath variable with the correct path to your database file" -ForegroundColor Yellow
    exit
}

Write-Host "Creating Notifications table in: $dbPath" -ForegroundColor Green

# SQL to create the Notifications table
$sql = @"
CREATE TABLE IF NOT EXISTS Notifications (
    NotificationId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    RecipientEmail TEXT NOT NULL,
    Type TEXT NOT NULL,
    Category TEXT NOT NULL,
    Title TEXT NOT NULL,
    Message TEXT NOT NULL,
    ActionUrl TEXT,
    ActionLabel TEXT,
    RelatedEntityId TEXT,
    RelatedEntityType TEXT,
    Priority TEXT NOT NULL,
    IsRead INTEGER NOT NULL DEFAULT 0,
    ReadAt TEXT,
    EmailSent INTEGER NOT NULL DEFAULT 0,
    EmailSentAt TEXT,
    ExpiresAt TEXT,
    CreatedAt TEXT NOT NULL,
    Icon TEXT
);

CREATE INDEX IF NOT EXISTS IX_Notifications_RecipientEmail_IsRead 
ON Notifications (RecipientEmail, IsRead);

CREATE INDEX IF NOT EXISTS IX_Notifications_CreatedAt 
ON Notifications (CreatedAt);

CREATE INDEX IF NOT EXISTS IX_Notifications_ExpiresAt 
ON Notifications (ExpiresAt);
"@

try {
    # Load SQLite assembly
    Add-Type -AssemblyName System.Data
    
    # Create connection
    $connectionString = "Data Source=$dbPath;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    # Create command
    $command = $connection.CreateCommand()
    $command.CommandText = $sql
    
    # Execute
    $command.ExecuteNonQuery() | Out-Null
    
    Write-Host "✅ Notifications table created successfully!" -ForegroundColor Green
    
    # Verify table exists
    $command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Notifications';"
    $reader = $command.ExecuteReader()
    
    if ($reader.Read()) {
        Write-Host "✅ Verified: Notifications table exists" -ForegroundColor Green
    } else {
        Write-Host "❌ Warning: Could not verify table creation" -ForegroundColor Yellow
    }
    
    $reader.Close()
    $connection.Close()
    
    Write-Host ""
    Write-Host "🎉 Done! You can now restart your application and create transactions." -ForegroundColor Cyan
    Write-Host "The notifications should work now!" -ForegroundColor Cyan
}
catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: Please use DB Browser for SQLite or another SQLite tool" -ForegroundColor Yellow
    Write-Host "and run the SQL from CreateNotificationsTable.sql" -ForegroundColor Yellow
}
"@