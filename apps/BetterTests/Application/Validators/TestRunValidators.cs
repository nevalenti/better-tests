using BetterTests.Application.DTOs;

using FluentValidation;

namespace BetterTests.Application.Validators;

public class CreateTestRunRequestValidator : AbstractValidator<CreateTestRunRequest>
{
    public CreateTestRunRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Test run name is required")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters");

        RuleFor(x => x.Environment)
            .NotEmpty().WithMessage("Environment is required")
            .MaximumLength(255).WithMessage("Environment must not exceed 255 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid TestRunStatus value");

        RuleFor(x => x.CompletedAt)
            .GreaterThanOrEqualTo(x => x.StartedAt)
            .When(x => x.StartedAt.HasValue && x.CompletedAt.HasValue)
            .WithMessage("CompletedAt must be greater than or equal to StartedAt");
    }
}
