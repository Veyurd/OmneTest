using Npgsql;
using Dapper;

namespace OmneTest.ApiService.Extensions
{
    public static class WebAppExtensions
    {
        public static WebApplication EnsureDatabaseTable(this WebApplication app)
        {
            // Resolve the connection from DI
            using var scope = app.Services.CreateScope();
            var conn = scope.ServiceProvider.GetRequiredService<NpgsqlConnection>();

            // SQL to create table if it doesn't exist
            var sql = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id SERIAL PRIMARY KEY,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                Description TEXT
            );
        ";

            conn.Execute(sql);

            return app;
        }
    }
}
