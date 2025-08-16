using FastEndpoints;
using FluentValidation;
using Dtos.ApiDtos;


//Fluent Validators Used, comes bundled with Fast Endpoints Package
namespace OmneTest.ApiService.Validators
{
    public class DeleteProductRequestDtoValidator : Validator<DeleteProductRequestDto>
    {
        public DeleteProductRequestDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().NotNull();
        }
    }
}
