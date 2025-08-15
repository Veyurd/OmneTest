using Dapper;
using Dtos.Dtos;
using Npgsql;
using Repository.Interfaces;

namespace Repository.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly NpgsqlConnection _connection;

        public ProductRepository(NpgsqlConnection pgConnection)
        {
            _connection = pgConnection;
        }
 
        public async Task<int> CreateAsync(ProductCreateDto product)
        {
            const string sql = @"
                INSERT INTO Products (Name, Price, Description)
                VALUES (@Name, @Price, @Description)
                RETURNING Id;";

            return await _connection.ExecuteScalarAsync<int>(sql, product);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Products WHERE Id = @Id;";
            return await _connection.QueryFirstOrDefaultAsync<ProductDto>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Products ORDER BY Id;";
            return await _connection.QueryAsync<ProductDto>(sql);
        }

        public async Task<bool> UpdateAsync(ProductDto product)
        {
            const string sql = @"
                UPDATE Products
                SET Name = @Name, Price = @Price, Description = @Description
                WHERE Id = @Id;";

            var rows = await _connection.ExecuteAsync(sql, product);
            return rows == 1;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Products WHERE Id = @Id;";
            var rows = await _connection.ExecuteAsync(sql, new { Id = id });
            return rows == 1;
        }
    }
}
