using FluentValidation;
using Gateway.Application.AccessPolicies.Dtos;

namespace Gateway.Application.Validators;

public class CreateAccessPolicyRequestValidator : AbstractValidator<CreateAccessPolicyRequest>
{
    public CreateAccessPolicyRequestValidator()
    {
        RuleFor(x => x.ScopeId)
            .GreaterThan(0);

        RuleFor(x => x)
            .Must(x => x.UserId.HasValue || x.ApiKeyId.HasValue)
            .WithMessage("Either UserId or ApiKeyId must be provided.");
    }
}