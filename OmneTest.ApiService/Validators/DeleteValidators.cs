using FastEndpoints;
using FluentValidation;
using Dtos.ApiDtos;

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
