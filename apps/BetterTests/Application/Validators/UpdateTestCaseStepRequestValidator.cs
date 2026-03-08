using BetterTests.Application.DTOs;

using FluentValidation;

namespace BetterTests.Application.Validators;

public class UpdateTestCaseStepRequestValidator : AbstractValidator<UpdateTestCaseStepRequest>
{
    public UpdateTestCaseStepRequestValidator()
    {
        RuleFor(x => x.StepOrder)
            .GreaterThan(0).WithMessage("Step order must be greater than 0");

        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("Step action is required")
            .MaximumLength(1000).WithMessage("Action must not exceed 1000 characters")
            .MinimumLength(5).WithMessage("Action must be at least 5 characters");

        RuleFor(x => x.ExpectedResult)
            .NotEmpty().WithMessage("Expected result is required")
            .MaximumLength(1000).WithMessage("Expected result must not exceed 1000 characters")
            .MinimumLength(5).WithMessage("Expected result must be at least 5 characters");
    }
}
