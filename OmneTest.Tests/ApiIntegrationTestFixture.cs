using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace ApiService.IntegrationTests;

public class ApiIntegrationTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private NpgsqlConnection? _dbConnection;

    public ApiIntegrationTestFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithPortBinding(0, true) // Random port
            .Build();

    }

    public string ConnectionString => _postgresContainer.GetConnectionString();

    public NpgsqlConnection DbConnection => _dbConnection
        ?? throw new InvalidOperationException("Database connection not initialized");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing database registration if any
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(NpgsqlConnection) ||
                d.ServiceType.Name.Contains("ConnectionString"));

            if (descriptor != null)
                services.Remove(descriptor);

            // Add test database connection string for Dapper
            services.AddScoped<NpgsqlConnection>(_ =>
                new NpgsqlConnection(ConnectionString));
        });

        builder.UseEnvironment("Testing");

        // Disable Aspire dashboard and other distributed concerns
        builder.UseSetting("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
        builder.UseSetting("OTEL_EXPORTER_OTLP_ENDPOINT", "");
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        // Initialize database connection for direct queries in tests
        _dbConnection = new NpgsqlConnection(ConnectionString);
        await _dbConnection.OpenAsync();

        // Create Product table
        await CreateProductTableAsync();
    }

    public new async Task DisposeAsync()
    {
        _dbConnection?.Dispose();
        await _postgresContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    private async Task CreateProductTableAsync()
    {
        const string createTableSql = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id SERIAL PRIMARY KEY,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                Description TEXT
            );";

        using var cmd = new NpgsqlCommand(createTableSql, _dbConnection);
        await cmd.ExecuteNonQueryAsync();
    }
}