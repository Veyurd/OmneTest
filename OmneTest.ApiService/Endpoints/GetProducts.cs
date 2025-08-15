using Dtos.ApiDtos;
using Dtos.Dtos;
using FastEndpoints;
using Repository.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OmneTest.ApiService.Endpoints
{
    public class GetProductsEndpoint : Endpoint<GetProductRequestDto, GetProductResponseDto>
    {

        IProductRepository _repository;

        public GetProductsEndpoint(IProductRepository repository)
        {
            _repository = repository;
        }
        public override void Configure()
        {
            Get("/products/{Id?}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetProductRequestDto req, CancellationToken ct)
        {
            GetProductResponseDto result=new GetProductResponseDto();
            result.Products = new List<ProductDto>();
            if (req.Id.HasValue)
            {
                var product = await _repository.GetByIdAsync(req.Id.Value);
                if (product == null)
                {
                    await Send.NotFoundAsync();
                    return;
                }
                else
                {
                    result.Products.Add(product);
                }
            }
            else
            {
                result.Products.AddRange(await _repository.GetAllAsync());
            }
            await Send.OkAsync(result);
        }
    }
}
