using Dtos.Dtos;
using FastEndpoints;
using Dtos.ApiDtos;
using Repository.Interfaces;

namespace OmneTest.ApiService.Endpoints
{
    public class PutProductEndpoint : Endpoint<PutProductRequestDto>
    {
        IProductRepository _repository;

        public PutProductEndpoint(IProductRepository repository)
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Put("/products/{Id?}");
            AllowAnonymous();
        }


        public override async Task HandleAsync(PutProductRequestDto req, CancellationToken ct)
        {

            if (!req.Id.HasValue)
            {
                await Send.NotFoundAsync();
                return;
            }
            var product = await _repository.GetByIdAsync(req.Id.Value);
            if (product == null)
            {
                await Send.NotFoundAsync();
                return;
            }

            ProductDto productEditDto = new ProductDto
            {
                Id = req.Id.Value,
                Name = req.Name,
                Price = req.Price,
                Description = req.Description
            };
            bool updated = await _repository.UpdateAsync(productEditDto);

            await Send.OkAsync();
        }
    }
}
