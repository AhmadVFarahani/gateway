using FluentValidation;
using Gateway.Application.ApiKeys;

namespace Gateway.Application.Validators;

public class CreateApiKeyRequestValidator : AbstractValidator<CreateApiKeyRequest>
{
    public CreateApiKeyRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.ExpirationDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.ExpirationDate.HasValue)
            .WithMessage("ExpirationDate must be a future date.");
    }
}