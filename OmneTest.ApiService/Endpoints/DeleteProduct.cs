using FastEndpoints;
using Dtos.ApiDtos;
using Repository.Interfaces;

namespace OmneTest.ApiService.Endpoints
{
    public class DeleteProductEndpoint : Endpoint<DeleteProductRequestDto>
    {
        IProductRepository _repository;

        public DeleteProductEndpoint(IProductRepository repository)
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Delete("/products/{Id?}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(DeleteProductRequestDto req, CancellationToken ct)
        {
            //Null id parameter caught by validator
            var product = await _repository.GetByIdAsync(req.Id!.Value);
            if (product == null)
            {
                await Send.NotFoundAsync();
                return;
            }

            bool deleted = await _repository.DeleteAsync(req.Id!.Value);
            if (!deleted)
            {
                await Send.ErrorsAsync();
                return;
            }

            await Send.NoContentAsync();
        }
    }
}
