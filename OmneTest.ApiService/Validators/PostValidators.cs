using FluentValidation;
using FastEndpoints;
using Dtos.ApiDtos;


//Fluent Validators Used, comes bundled with Fast Endpoints Package
namespace OmneTest.ApiService.Validators
{
    public class PostProductRequestDtoValidator : Validator<PostProductRequestDto>
    {
        public PostProductRequestDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("your name is required!");
                

            RuleFor(x => x.Price)
                     .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}
