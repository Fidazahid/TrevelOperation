using Microsoft.Data.Sqlite;
using System;

namespace TravelOperation.DatabaseUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🔧 Updating Notifications table schema...");
            
            // Update all database files
            var dbPaths = new[]
            {
                @"c:\Users\imran\source\repos\dawloom\TrevelOperation\TrevelOperation\TravelOperations.db",
                @"c:\Users\imran\source\repos\dawloom\TrevelOperation\TravelOperation.Core\TravelOperations.db"
            };

            foreach (var dbPath in dbPaths)
            {
                if (System.IO.File.Exists(dbPath))
                {
                    try
                    {
                        UpdateDatabase(dbPath);
                        Console.WriteLine($"✅ Updated: {dbPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error updating {dbPath}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ Not found: {dbPath}");
                }
            }

            Console.WriteLine("\n✅ Schema update complete!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void UpdateDatabase(string dbPath)
        {
            var connectionString = $"Data Source={dbPath}";
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Check if columns already exist
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "PRAGMA table_info(Notifications)";
                
                bool hasCreatedByEmail = false;
                bool hasCreatedByName = false;
                
                using (var reader = checkCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var columnName = reader.GetString(1);
                        if (columnName == "CreatedByEmail") hasCreatedByEmail = true;
                        if (columnName == "CreatedByName") hasCreatedByName = true;
                    }
                }

                // Add CreatedByEmail column if it doesn't exist
                if (!hasCreatedByEmail)
                {
                    var cmd1 = connection.CreateCommand();
                    cmd1.CommandText = "ALTER TABLE Notifications ADD COLUMN CreatedByEmail TEXT;";
                    cmd1.ExecuteNonQuery();
                    Console.WriteLine("  ✓ Added CreatedByEmail column");
                }
                else
                {
                    Console.WriteLine("  ℹ️ CreatedByEmail column already exists");
                }

                // Add CreatedByName column if it doesn't exist
                if (!hasCreatedByName)
                {
                    var cmd2 = connection.CreateCommand();
                    cmd2.CommandText = "ALTER TABLE Notifications ADD COLUMN CreatedByName TEXT;";
                    cmd2.ExecuteNonQuery();
                    Console.WriteLine("  ✓ Added CreatedByName column");
                }
                else
                {
                    Console.WriteLine("  ℹ️ CreatedByName column already exists");
                }
            }
        }
    }
}
