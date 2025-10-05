#r "nuget: Microsoft.EntityFrameworkCore, 8.0.8"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 8.0.8"
#load "TravelOperation.Core/Data/TravelDbContext.cs"

using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;

var connectionString = "Data Source=TravelOperation.Core/TravelExpense.db";
var options = new DbContextOptionsBuilder<TravelDbContext>()
    .UseSqlite(connectionString)
    .Options;

using var context = new TravelDbContext(options);

Console.WriteLine("Applied Migrations:");
var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
foreach (var migration in appliedMigrations)
{
    Console.WriteLine($"  {migration}");
}

Console.WriteLine("\nPending Migrations:");
var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
foreach (var migration in pendingMigrations)
{
    Console.WriteLine($"  {migration}");
}

Console.WriteLine($"\nTotal Applied: {appliedMigrations.Count()}");
Console.WriteLine($"Total Pending: {pendingMigrations.Count()}");