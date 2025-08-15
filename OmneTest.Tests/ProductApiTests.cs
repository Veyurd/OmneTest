using Dapper;
using Dtos.ApiDtos;
using Dtos.Dtos;
using System.Net.Http.Json;

namespace ApiService.IntegrationTests;


/// <summary>
/// Multiple tests can be added, the set bellow is a showcase going throught the required HTTP verbs in the requirements.
/// </summary>
[Collection("Integration Tests")]
public class ProductApiTests : IClassFixture<ApiIntegrationTestFixture>
{
    private readonly ApiIntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    public ProductApiTests(ApiIntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ShouldPersistToDatabase()
    {
        // Arrange
        var product = new PostProductRequestDto
        {
            Name = "Test Product",
            Price = 29.99f,
            Description = "Test prod description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/products", product);

        // Assert API response
        response.EnsureSuccessStatusCode();
        var productFound = await response.Content.ReadFromJsonAsync<PostProductResponseDto>();
        Assert.True(productFound.Id > 0);

        // Assert database state using Dapper
        var dbProduct = await _fixture.DbConnection.QuerySingleOrDefaultAsync<ProductDto>(
            "SELECT Id, Name, Price, Description FROM Products WHERE Id = @Id",
            new { Id = productFound.Id });

        Assert.NotNull(dbProduct);
        Assert.Equal(product.Name, dbProduct.Name);
        Assert.Equal(product.Price, dbProduct.Price);
        Assert.Equal(product.Description, dbProduct.Description);
    }

    [Fact]
    public async Task GetProductById_WithExistingProduct_ShouldReturnCorrectData()
    {
        // Arrange - Insert test product directly
        var productId = await InsertTestProductAsync("Get Test Product", 15.50m, "Get test description");

        // Act
        var response = await _client.GetAsync($"/products/{productId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<GetProductResponseDto>();

        Assert.NotNull(payload);
        Assert.NotNull(payload.Products);
        Assert.Single(payload.Products);
        // Verify the response matches database
        var dbProduct = await _fixture.DbConnection.QuerySingleAsync<ProductDto>(
            "SELECT ID, Name, Price, Description FROM Products WHERE ID = @Id",
            new { Id = productId });

        Assert.Equal(dbProduct.Name, payload.Products.First().Name);
    }

    [Fact]
    public async Task GetAllProducts_WithExistingData_ShouldReturnAllProducts()
    {
        // Arrange - Clean table and insert test data
        await CleanupProductTableAsync();
        await InsertTestProductAsync("Product 1", 10.00m, "Description 1");
        await InsertTestProductAsync("Product 2", 20.00m, "Description 2");
        await InsertTestProductAsync("Product 3", 30.00m, "Description 3");

        // Act
        var response = await _client.GetAsync("/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<GetProductResponseDto>();
        Assert.NotNull(payload);
        Assert.NotNull(payload.Products);
        Assert.Equal(3, payload.Products.Count);

        // Verify database count matches
        var dbCount = await _fixture.DbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Products");
        Assert.Equal(3, dbCount);
    }

    [Fact]
    public async Task UpdateProduct_ShouldModifyDatabaseRecord()
    {
        // Arrange
        var originalProductId = await InsertTestProductAsync("Original Product", 25.00m, "Original description");
        var updatedProduct = new PutProductRequestDto
        {
            Id = originalProductId,
            Name = "Updated Product",
            Price = 35.99f,
            Description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/products/{originalProductId}", updatedProduct);

        // Assert API response
        response.EnsureSuccessStatusCode();

        // Assert database state
        var dbProduct = await _fixture.DbConnection.QuerySingleAsync<ProductDto>(
            "SELECT ID, Name, Price, Description FROM Products WHERE ID = @Id",
            new { Id = originalProductId });

        Assert.Equal(updatedProduct.Name, dbProduct.Name);
        Assert.Equal(updatedProduct.Price, dbProduct.Price);
        Assert.Equal(updatedProduct.Description, dbProduct.Description);
    }

    [Fact]
    public async Task DeleteProduct_ShouldRemoveFromDatabase()
    {
        // Arrange
        var productId = await InsertTestProductAsync("Delete Me", 99.99m, "Delete test");

        // Act
        var response = await _client.DeleteAsync($"/products/{productId}");

        // Assert API response
        response.EnsureSuccessStatusCode();

        // Assert database state - product should be gone
        var productExists = await _fixture.DbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Products WHERE ID = @Id",
            new { Id = productId });

        Assert.Equal(0, productExists);
    }

    [Fact]
    public async Task GetProduct_NonExistentId_ShouldReturn404()
    {
        // Act
        var response = await _client.GetAsync("/products/99999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_InvalidData_ShouldReturnBadRequest()
    {
        // Arrange - Invalid product (missing required fields)
        var invalidProduct = new PostProductRequestDto { Price = -10.2f }; // Negative price, no name
        var initialProductCount = await _fixture.DbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Products");
        // Act
        var response = await _client.PostAsJsonAsync("/products", invalidProduct);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        // Verify no product was created in database
        var productCount = await _fixture.DbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Products");
        Assert.Equal(initialProductCount, productCount);
    }

    private async Task<int> InsertTestProductAsync(string name, decimal price, string description)
    {
        const string insertSql = @"
            INSERT INTO Products (Name, Price, Description) 
            VALUES (@Name, @Price, @Description) 
            RETURNING ID";

        return await _fixture.DbConnection.QuerySingleAsync<int>(insertSql, new
        {
            Name = name,
            Price = price,
            Description = description
        });
    }

    private async Task CleanupProductTableAsync()
    {
        await _fixture.DbConnection.ExecuteAsync("TRUNCATE TABLE Products RESTART IDENTITY");
    }
}