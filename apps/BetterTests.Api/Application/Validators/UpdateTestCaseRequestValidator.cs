using BetterTests.Application.DTOs;

using FluentValidation;

namespace BetterTests.Application.Validators;

public class UpdateTestCaseRequestValidator : AbstractValidator<UpdateTestCaseRequest>
{
    public UpdateTestCaseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Test case name is required")
            .MaximumLength(500).WithMessage("Name must not exceed 500 characters")
            .MinimumLength(5).WithMessage("Name must be at least 5 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Preconditions)
            .MaximumLength(2000).WithMessage("Preconditions must not exceed 2000 characters");

        RuleFor(x => x.Postconditions)
            .MaximumLength(2000).WithMessage("Postconditions must not exceed 2000 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be a valid TestCasePriority value");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid TestCaseStatus value");
    }
}
