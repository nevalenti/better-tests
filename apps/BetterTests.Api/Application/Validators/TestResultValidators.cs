using BetterTests.Application.DTOs;

using FluentValidation;

namespace BetterTests.Application.Validators;

public class CreateTestResultRequestValidator : AbstractValidator<CreateTestResultRequest>
{
    public CreateTestResultRequestValidator()
    {
        RuleFor(x => x.Result)
            .IsInEnum().WithMessage("Result must be a valid TestResultStatus value");

        RuleFor(x => x.Comments)
            .MaximumLength(2000).WithMessage("Comments must not exceed 2000 characters");

        RuleFor(x => x.DefectLink)
            .MaximumLength(500).WithMessage("Defect link must not exceed 500 characters")
            .Must(x => string.IsNullOrEmpty(x) || Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("Defect link must be a valid URL");
    }
}
