using BetterTests.Application.DTOs;

using FluentValidation;

namespace BetterTests.Application.Validators;

public class UpdateTestSuiteRequestValidator : AbstractValidator<UpdateTestSuiteRequest>
{
    public UpdateTestSuiteRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Test suite name is required")
            .MaximumLength(255).WithMessage("Test suite name must not exceed 255 characters")
            .MinimumLength(3).WithMessage("Test suite name must be at least 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
    }
}
