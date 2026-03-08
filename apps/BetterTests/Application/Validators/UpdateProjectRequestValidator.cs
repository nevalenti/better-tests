using BetterTests.Application.DTOs;

using FluentValidation;

namespace BetterTests.Application.Validators;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(255).WithMessage("Project name must not exceed 255 characters")
            .MinimumLength(3).WithMessage("Project name must be at least 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
    }
}
