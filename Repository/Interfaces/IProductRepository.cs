using Dtos.Dtos;

namespace Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<int> CreateAsync(ProductCreateDto product);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<bool> UpdateAsync(ProductDto product);
        Task<bool> DeleteAsync(int id);
    }
}
