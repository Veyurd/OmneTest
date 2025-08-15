using Dtos.ApiDtos;
using Dtos.Dtos;
using System.Linq.Expressions;

namespace OmneTest.Web
{
    public class ProductApiClient(HttpClient httpClient)
    {
        public async Task<List<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            List<ProductDto> products = new List<ProductDto>();

            var payload = await httpClient.GetFromJsonAsync<GetProductResponseDto>("/products", cancellationToken);
            if (payload != null && payload.Products != null)
                products = payload.Products;
            return products;
        }

        public async Task<ProductDto> GetProductByIdAsync(int Id, CancellationToken cancellationToken = default)
        {
            ProductDto? product = null;

            var response = await httpClient.GetAsync($"/products/{Id}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<GetProductResponseDto>(cancellationToken: cancellationToken);
                if (payload != null && payload.Products != null)
                    product = payload.Products.FirstOrDefault();
            }
           
            return product;
        }

        public async Task<bool> UpdateProductAsync(ProductDto model, CancellationToken cancellationToken = default)
        {
            //TODO improve this
            PutProductRequestDto request = new PutProductRequestDto
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                Description = model.Description
            };

            var response = await httpClient.PutAsJsonAsync("/products", request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }

        public async Task<bool> DeleteProductAsync(int Id, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.DeleteAsync($"/products/{Id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<int> AddProductAsync(ProductCreateDto model, CancellationToken cancellationToken = default)
        {
            PostProductRequestDto request = new PostProductRequestDto
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description
            };

            var response = await httpClient.PostAsJsonAsync("/products", request);

            int result = -1;
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<PostProductResponseDto>();
                if (responseData != null)
                {
                    result = responseData.Id;
                }
            }
            else
            {
                result= -1;
            }

            return result;
        }
    }
}
