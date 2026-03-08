using FluentValidation;

namespace BetterTests.Application.Validators;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> MustBeHttpUrl<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(uri => string.IsNullOrEmpty(uri)
                || (Uri.TryCreate(uri, UriKind.Absolute, out var parsed) && parsed.Scheme is "http" or "https"))
            .WithMessage("Must be a valid HTTP or HTTPS URL");
    }
}
