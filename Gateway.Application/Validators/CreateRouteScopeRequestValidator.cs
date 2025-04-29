using FluentValidation;
using Gateway.Application.RouteScopes.Dtos;

namespace Gateway.Application.Validators;

public class CreateRouteScopeRequestValidator : AbstractValidator<CreateRouteScopeRequest>
{
    public CreateRouteScopeRequestValidator()
    {
        RuleFor(x => x.RouteId).GreaterThan(0);
        RuleFor(x => x.ScopeId).GreaterThan(0);
    }
}