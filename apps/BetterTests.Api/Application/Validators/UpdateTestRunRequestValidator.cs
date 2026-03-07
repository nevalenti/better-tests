using BetterTests.Application.DTOs;

using FluentValidation;

namespace BetterTests.Application.Validators;

public class UpdateTestRunRequestValidator : AbstractValidator<UpdateTestRunRequest>
{
    public UpdateTestRunRequestValidator()
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
    }
}
