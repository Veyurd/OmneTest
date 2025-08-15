using FastEndpoints;
using FluentValidation;
using Dtos.ApiDtos;

namespace OmneTest.ApiService.Validators
{
    public class PutProductRequestDtoValidator : Validator<PutProductRequestDto>
    {
        public PutProductRequestDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().NotNull();

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("your name is required!");


            RuleFor(x => x.Price)
                     .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}
