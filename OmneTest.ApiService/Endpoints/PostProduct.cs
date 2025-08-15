using FastEndpoints;
using Repository.Interfaces;
using Dtos.Dtos;
using Dtos.ApiDtos;

namespace OmneTest.ApiService.Endpoints
{
    public class PostProductEndpoint : Endpoint<PostProductRequestDto, PostProductResponseDto>
    {
        IProductRepository _repository;

        public PostProductEndpoint(IProductRepository repository)
        {
            _repository = repository;
        }

        public override void Configure()
        {
            Post("/products");
            AllowAnonymous();
        }

        public override async Task HandleAsync(PostProductRequestDto req, CancellationToken ct)
        {
            ProductCreateDto newProduct = new ProductCreateDto { Name = req.Name, Price = req.Price, Description = req.Description };
            var NewId = await _repository.CreateAsync(newProduct);
            await Send.OkAsync(new PostProductResponseDto { Id = NewId });
        }
    }



}
