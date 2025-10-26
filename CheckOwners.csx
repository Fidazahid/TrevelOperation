using Microsoft.Data.Sqlite;

var connectionString = "Data Source=TrevelOperation\\TravelOperations.db";
using var connection = new SqliteConnection(connectionString);
connection.Open();

// Check Owners table
var command = connection.CreateCommand();
command.CommandText = "SELECT OwnerId, Name, Email FROM Owners ORDER BY Name";

using var reader = command.ExecuteReader();
Console.WriteLine("=== OWNERS IN DATABASE ===");
int count = 0;
while (reader.Read())
{
    count++;
    Console.WriteLine($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}, Email: {reader.GetString(2)}");
}

if (count == 0)
{
    Console.WriteLine("⚠️ NO OWNERS FOUND!");
}
else
{
    Console.WriteLine($"\n✅ Total Owners: {count}");
}

// Check Trips with Owner
Console.WriteLine("\n=== TRIPS WITH OWNERS ===");
command = connection.CreateCommand();
command.CommandText = @"
    SELECT t.TripId, t.TripName, t.OwnerId, o.Name as OwnerName 
    FROM Trips t 
    LEFT JOIN Owners o ON t.OwnerId = o.OwnerId 
    LIMIT 5";

using var tripReader = command.ExecuteReader();
int tripCount = 0;
while (tripReader.Read())
{
    tripCount++;
    var ownerId = tripReader.IsDBNull(2) ? "NULL" : tripReader.GetInt32(2).ToString();
    var ownerName = tripReader.IsDBNull(3) ? "NULL" : tripReader.GetString(3);
    Console.WriteLine($"Trip: {tripReader.GetString(1)}, OwnerId: {ownerId}, Owner: {ownerName}");
}

if (tripCount == 0)
{
    Console.WriteLine("⚠️ NO TRIPS FOUND!");
}
else
{
    Console.WriteLine($"\n✅ Sample of {tripCount} trips shown");
}

connection.Close();
