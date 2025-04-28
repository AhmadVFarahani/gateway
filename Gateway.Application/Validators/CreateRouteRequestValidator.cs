using FluentValidation;
using Gateway.Application.Routes.Dtos;

namespace Gateway.Application.Validators;

public class CreateRouteRequestValidator : AbstractValidator<CreateRouteRequest>
{
    public CreateRouteRequestValidator()
    {
        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Path is required.");

        RuleFor(x => x.HttpMethod)
            .NotEmpty().WithMessage("HttpMethod is required.");

        RuleFor(x => x.TargetPath)
            .NotEmpty().WithMessage("TargetPath is required.");
    }
}
