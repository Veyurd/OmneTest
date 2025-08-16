using Dapper;
using Dtos.Dtos;
using Npgsql;
using Repository.Interfaces;
using System.Data;

namespace Repository.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly NpgsqlConnection _connection;

        public ProductRepository(NpgsqlConnection pgConnection)
        {
            _connection = pgConnection;
        }

        public Task<int> CreateAsync(ProductCreateDto product)
        {
            const string sql = @"
                INSERT INTO Products (Name, Price, Description)
                VALUES (@Name, @Price, @Description)
                RETURNING Id;";

            return ExecuteWithTransactionAsync(
                tx => _connection.ExecuteScalarAsync<int>(sql, product, tx));
        }

        public Task<ProductDto?> GetByIdAsync(int id)
        {
            const string sql = "SELECT Id, Name, Price, Description FROM Products WHERE Id = @Id;";

            return ExecuteWithTransactionAsync(
                tx => _connection.QueryFirstOrDefaultAsync<ProductDto>(sql, new { Id = id }, tx));
        }

        public Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            const string sql = "SELECT Id, Name, Price, Description FROM Products ORDER BY Id;";

            return ExecuteWithTransactionAsync(
                tx => _connection.QueryAsync<ProductDto>(sql, transaction: tx));
        }

        public Task<bool> UpdateAsync(ProductDto product)
        {
            const string sql = @"
                UPDATE Products
                SET Name = @Name, Price = @Price, Description = @Description
                WHERE Id = @Id;";

            return ExecuteWithTransactionAsync(
                async tx =>
                {
                    var rows = await _connection.ExecuteAsync(sql, product, tx);
                    return rows == 1;
                });
        }

        public Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Products WHERE Id = @Id;";

            return ExecuteWithTransactionAsync(
                async tx =>
                {
                    var rows = await _connection.ExecuteAsync(sql, new { Id = id }, tx);
                    return rows == 1;
                });
        }

        /// <summary>
        /// Helper used to handle transaction wrapping for all db calls of the Repo. Not very usefull right now, good for futureproofing.
        /// </summary>
        private async Task<T> ExecuteWithTransactionAsync<T>(Func<NpgsqlTransaction, Task<T>> operation)
        {
            await EnsureOpenAsync();
            await using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                var result = await operation(transaction);
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await EnsureClosedAsync();
            }
        }

        private async Task EnsureOpenAsync()
        {
            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync();
        }

        private async Task EnsureClosedAsync()
        {
            if (_connection.State != ConnectionState.Closed)
                await _connection.CloseAsync();
        }
    }
}
