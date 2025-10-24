using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;

namespace TravelOperation.IntegrationTests;

/// <summary>
/// Base class for integration tests that provides SQLite database setup with transaction support
/// </summary>
public abstract class TestBase : IDisposable
{
    protected TravelDbContext Context { get; private set; }
    private readonly SqliteConnection _connection;

    protected TestBase()
    {
        // Create and open SQLite in-memory connection
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // Create DbContext with SQLite
        var options = new DbContextOptionsBuilder<TravelDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new TravelDbContext(options);
        
        // Create database schema
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
