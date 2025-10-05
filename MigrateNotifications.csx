#r "nuget: Microsoft.Data.Sqlite, 8.0.0"

using Microsoft.Data.Sqlite;
using System;

var dbPaths = new[]
{
    @"c:\Users\imran\source\repos\dawloom\TrevelOperation\TrevelOperation\TravelOperations.db",
    @"c:\Users\imran\source\repos\dawloom\TrevelOperation\TravelOperation.Core\TravelOperations.db"
};

foreach (var dbPath in dbPaths)
{
    if (File.Exists(dbPath))
    {
        Console.WriteLine($"Processing: {dbPath}");
        try
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "ALTER TABLE Notifications ADD COLUMN CreatedByEmail TEXT;";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("  ✓ Added CreatedByEmail");
                }
                
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "ALTER TABLE Notifications ADD COLUMN CreatedByName TEXT;";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("  ✓ Added CreatedByName");
                }
            }
            Console.WriteLine("  ✅ Success!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Error: {ex.Message}");
        }
    }
}

Console.WriteLine("\nDone! Press any key...");
Console.ReadKey();
