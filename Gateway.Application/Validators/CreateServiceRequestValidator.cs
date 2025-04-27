using FluentValidation;
using Gateway.Application.Services.Dtos;

namespace Gateway.Application.Validators;

public class CreateServiceRequestValidator : AbstractValidator<CreateServiceRequest>
{
    public CreateServiceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        //RuleFor(x => x.BaseUrl)
        //    .NotEmpty().WithMessage("BaseUrl is required.")
        //    .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
        //    .WithMessage("BaseUrl must be a valid URL.");
    }
}
