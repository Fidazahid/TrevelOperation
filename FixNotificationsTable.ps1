# Simple script to create Notifications table directly in SQLite database
$ErrorActionPreference = "Stop"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Creating Notifications Table" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Find database file
$dbPath = ".\TrevelOperation\TravelOperations.db"

if (-not (Test-Path $dbPath)) {
    Write-Host "ERROR: Database not found at: $dbPath" -ForegroundColor Red
    exit 1
}

Write-Host "Found database: $dbPath" -ForegroundColor Green

# SQL to create Notifications table
$createTableSql = @"
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
    "EmailSent" INTEGER NOT NULL DEFAULT 0,
    "EmailSentAt" TEXT NULL
);
"@

$createIndex1Sql = "CREATE INDEX IF NOT EXISTS `"IX_Notifications_RecipientEmail_IsRead`" ON `"Notifications`" (`"RecipientEmail`", `"IsRead`");"
$createIndex2Sql = "CREATE INDEX IF NOT EXISTS `"IX_Notifications_CreatedAt`" ON `"Notifications`" (`"CreatedAt`");"
$createIndex3Sql = "CREATE INDEX IF NOT EXISTS `"IX_Notifications_ExpiresAt`" ON `"Notifications`" (`"ExpiresAt`");"

try {
    Write-Host "`nAttempting to create Notifications table..." -ForegroundColor Yellow
    
    # Use dotnet ef dbcontext scaffold to execute SQL (hacky but works)
    # Actually, let's use System.Data.SQLite via PowerShell's Add-Type
    
    # Try loading SQLite assembly from project output
    $sqliteAssembly = ".\TrevelOperation\bin\Debug\net9.0-windows7.0\Microsoft.Data.Sqlite.dll"
    
    if (Test-Path $sqliteAssembly) {
        Write-Host "Loading SQLite assembly from: $sqliteAssembly" -ForegroundColor Yellow
        Add-Type -Path $sqliteAssembly
        
        $connectionString = "Data Source=$dbPath"
        $connection = New-Object Microsoft.Data.Sqlite.SqliteConnection($connectionString)
        $connection.Open()
        
        # Create table
        $command = $connection.CreateCommand()
        $command.CommandText = $createTableSql
        $command.ExecuteNonQuery() | Out-Null
        Write-Host "✓ Created Notifications table" -ForegroundColor Green
        
        # Create indexes
        $command.CommandText = $createIndex1Sql
        $command.ExecuteNonQuery() | Out-Null
        Write-Host "✓ Created index: IX_Notifications_RecipientEmail_IsRead" -ForegroundColor Green
        
        $command.CommandText = $createIndex2Sql
        $command.ExecuteNonQuery() | Out-Null
        Write-Host "✓ Created index: IX_Notifications_CreatedAt" -ForegroundColor Green
        
        $command.CommandText = $createIndex3Sql
        $command.ExecuteNonQuery() | Out-Null
        Write-Host "✓ Created index: IX_Notifications_ExpiresAt" -ForegroundColor Green
        
        # Verify
        $command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Notifications';"
        $reader = $command.ExecuteReader()
        
        if ($reader.Read()) {
            Write-Host "`n✅ SUCCESS! Notifications table exists in database!" -ForegroundColor Green
        } else {
            Write-Host "`n⚠ WARNING: Could not verify table creation" -ForegroundColor Yellow
        }
        
        $reader.Close()
        $connection.Close()
        
    } else {
        Write-Host "SQLite assembly not found. Using alternative method..." -ForegroundColor Yellow
        
        # Alternative: Use EF Core migrations (create a proper migration)
        Write-Host "`nCreating EF Core migration..." -ForegroundColor Yellow
        
        # Create a migration that only adds Notifications table
        $migrationContent = @"
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelOperation.Core.Migrations
{
    public partial class AddNotificationsTableOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@`"
                CREATE TABLE IF NOT EXISTS \`"Notifications\`" (
                    \`"NotificationId\`" INTEGER NOT NULL CONSTRAINT \`"PK_Notifications\`" PRIMARY KEY AUTOINCREMENT,
                    \`"RecipientEmail\`" TEXT NOT NULL,
                    \`"Type\`" TEXT NOT NULL,
                    \`"Category\`" TEXT NOT NULL,
                    \`"Priority\`" TEXT NOT NULL,
                    \`"Title\`" TEXT NOT NULL,
                    \`"Message\`" TEXT NOT NULL,
                    \`"ActionUrl\`" TEXT NULL,
                    \`"ActionLabel\`" TEXT NULL,
                    \`"RelatedEntityType\`" TEXT NULL,
                    \`"RelatedEntityId\`" TEXT NULL,
                    \`"Icon\`" TEXT NULL,
                    \`"IsRead\`" INTEGER NOT NULL DEFAULT 0,
                    \`"CreatedAt\`" TEXT NOT NULL,
                    \`"ReadAt\`" TEXT NULL,
                    \`"ExpiresAt\`" TEXT NULL,
                    \`"EmailSent\`" INTEGER NOT NULL DEFAULT 0,
                    \`"EmailSentAt\`" TEXT NULL
                );
                
                CREATE INDEX IF NOT EXISTS \`"IX_Notifications_RecipientEmail_IsRead\`" ON \`"Notifications\`" (\`"RecipientEmail\`", \`"IsRead\`");
                CREATE INDEX IF NOT EXISTS \`"IX_Notifications_CreatedAt\`" ON \`"Notifications\`" (\`"CreatedAt\`");
                CREATE INDEX IF NOT EXISTS \`"IX_Notifications_ExpiresAt\`" ON \`"Notifications\`" (\`"ExpiresAt\`");
            `");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Notifications");
        }
    }
}
"@
        
        $timestamp = Get-Date -Format "yyyyMMddHHmmss"
        $migrationFile = ".\TravelOperation.Core\Migrations\${timestamp}_AddNotificationsTableOnly.cs"
        
        Set-Content -Path $migrationFile -Value $migrationContent
        Write-Host "Created migration file: $migrationFile" -ForegroundColor Green
        
        Write-Host "`nApplying migration..." -ForegroundColor Yellow
        cd TrevelOperation
        & dotnet ef database update --project ..\TravelOperation.Core --verbose
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Migration applied successfully!" -ForegroundColor Green
        } else {
            Write-Host "❌ Migration failed" -ForegroundColor Red
            cd ..
            exit 1
        }
        
        cd ..
    }
    
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "✅ NOTIFICATIONS TABLE READY" -ForegroundColor Green
    Write-Host "========================================`n" -ForegroundColor Cyan
    
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Close your application completely" -ForegroundColor White
    Write-Host "2. Restart the application" -ForegroundColor White
    Write-Host "3. Login as Employee" -ForegroundColor White
    Write-Host "4. Create a transaction (e.g., $`$1234 meal)" -ForegroundColor White
    Write-Host "5. Watch console for debug messages:" -ForegroundColor White
    Write-Host "   - [TransactionService] Transaction created" -ForegroundColor Gray
    Write-Host "   - [TransactionService] Notifying Finance team" -ForegroundColor Gray
    Write-Host "   - [NotificationService] Finance team emails: ..." -ForegroundColor Gray
    Write-Host "6. Login as Finance (martina.popinsk@wsc.com)" -ForegroundColor White
    Write-Host "7. Check Notifications page`n" -ForegroundColor White
    
} catch {
    Write-Host "`n❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`nStack Trace:" -ForegroundColor Yellow
    Write-Host $_.ScriptStackTrace -ForegroundColor Gray
    exit 1
}
