using System;
using System.Data.SQLite;

class Program
{
    static void Main()
    {
        string dbPath = @".\TrevelOperation\TravelOperations.db";
        
        Console.WriteLine("\n========================================");
        Console.WriteLine("Creating Notifications Table");
        Console.WriteLine("========================================\n");
        
        if (!System.IO.File.Exists(dbPath))
        {
            Console.WriteLine($"ERROR: Database not found at: {dbPath}");
            return;
        }
        
        Console.WriteLine($"Found database: {dbPath}\n");
        
        string connectionString = $"Data Source={dbPath};Version=3;";
        
        string createTableSql = @"
CREATE TABLE IF NOT EXISTS ""Notifications"" (
    ""NotificationId"" INTEGER NOT NULL CONSTRAINT ""PK_Notifications"" PRIMARY KEY AUTOINCREMENT,
    ""RecipientEmail"" TEXT NOT NULL,
    ""Type"" TEXT NOT NULL,
    ""Category"" TEXT NOT NULL,
    ""Priority"" TEXT NOT NULL,
    ""Title"" TEXT NOT NULL,
    ""Message"" TEXT NOT NULL,
    ""ActionUrl"" TEXT NULL,
    ""ActionLabel"" TEXT NULL,
    ""RelatedEntityType"" TEXT NULL,
    ""RelatedEntityId"" TEXT NULL,
    ""Icon"" TEXT NULL,
    ""IsRead"" INTEGER NOT NULL DEFAULT 0,
    ""CreatedAt"" TEXT NOT NULL,
    ""ReadAt"" TEXT NULL,
    ""ExpiresAt"" TEXT NULL,
    ""EmailSent"" INTEGER NOT NULL DEFAULT 0,
    ""EmailSentAt"" TEXT NULL
);";
        
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("✓ Connected to database");
                
                using (var command = new SQLiteCommand(createTableSql, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("✓ Created Notifications table");
                }
                
                // Create indexes
                using (var command = new SQLiteCommand(@"CREATE INDEX IF NOT EXISTS ""IX_Notifications_RecipientEmail_IsRead"" ON ""Notifications"" (""RecipientEmail"", ""IsRead"");", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("✓ Created index: IX_Notifications_RecipientEmail_IsRead");
                }
                
                using (var command = new SQLiteCommand(@"CREATE INDEX IF NOT EXISTS ""IX_Notifications_CreatedAt"" ON ""Notifications"" (""CreatedAt"");", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("✓ Created index: IX_Notifications_CreatedAt");
                }
                
                using (var command = new SQLiteCommand(@"CREATE INDEX IF NOT EXISTS ""IX_Notifications_ExpiresAt"" ON ""Notifications"" (""ExpiresAt"");", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("✓ Created index: IX_Notifications_ExpiresAt");
                }
                
                // Verify
                using (var command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='Notifications';", connection))
                {
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        Console.WriteLine("\n✅ SUCCESS! Notifications table exists in database!");
                    }
                    else
                    {
                        Console.WriteLine("\n⚠ WARNING: Could not verify table creation");
                    }
                }
            }
            
            Console.WriteLine("\n========================================");
            Console.WriteLine("✅ NOTIFICATIONS TABLE READY");
            Console.WriteLine("========================================\n");
            
            Console.WriteLine("Next steps:");
            Console.WriteLine("1. Close your application completely");
            Console.WriteLine("2. Restart the application");
            Console.WriteLine("3. Login as Employee and create a $1234 meal transaction");
            Console.WriteLine("4. Watch console for debug messages");
            Console.WriteLine("5. Login as Finance (martina.popinsk@wsc.com)");
            Console.WriteLine("6. Check Notifications page\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ ERROR: {ex.Message}");
            Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");
        }
    }
}
