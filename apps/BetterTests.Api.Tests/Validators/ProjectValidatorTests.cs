using BetterTests.Application.DTOs;
using BetterTests.Application.Validators;

using FluentAssertions;

using Xunit;

namespace BetterTests.Api.Tests.Validators;

public class CreateProjectRequestValidatorTests
{
    private readonly CreateProjectRequestValidator _validator;

    public CreateProjectRequestValidatorTests()
    {
        _validator = new CreateProjectRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidName_Succeeds()
    {
        // Arrange
        var request = new CreateProjectRequest("Valid Project Name", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithEmptyName_FailsWithRequiredError()
    {
        // Arrange
        var request = new CreateProjectRequest("", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNullName_FailsWithRequiredError()
    {
        // Arrange
        var request = new CreateProjectRequest(null!, "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameExceeding255Characters_FailsWithLengthError()
    {
        // Arrange
        var longName = new string('a', 256);
        var request = new CreateProjectRequest(longName, "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorCode == "MaximumLengthValidator");
    }

    [Fact]
    public async Task Validate_WithNameExactly200Characters_Succeeds()
    {
        // Arrange
        var name = new string('a', 200);
        var request = new CreateProjectRequest(name, "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithLeadingWhitespace_Succeeds()
    {
        // Arrange
        var request = new CreateProjectRequest(" Leading Whitespace", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithNullDescription_Succeeds()
    {
        // Arrange
        var request = new CreateProjectRequest("Project Name", null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyDescription_Succeeds()
    {
        // Arrange
        var request = new CreateProjectRequest("Project Name", "");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithDescriptionExceeding1000Characters_FailsWithLengthError()
    {
        // Arrange
        var longDescription = new string('a', 1001);
        var request = new CreateProjectRequest("Project Name", longDescription);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task Validate_WithDescriptionExactly1000Characters_Succeeds()
    {
        // Arrange
        var description = new string('a', 1000);
        var request = new CreateProjectRequest("Project Name", description);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithMinimumValidName_Succeeds()
    {
        // Arrange
        var request = new CreateProjectRequest("ABC", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithSpecialCharactersInName_Succeeds()
    {
        // Arrange
        var request = new CreateProjectRequest("Project-Name_123 (Test)", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithUnicodeCharactersInName_Succeeds()
    {
        // Arrange
        var request = new CreateProjectRequest("Project Café 日本語", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

public class UpdateProjectRequestValidatorTests
{
    private readonly UpdateProjectRequestValidator _validator;

    public UpdateProjectRequestValidatorTests()
    {
        _validator = new UpdateProjectRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidUpdateRequest_Succeeds()
    {
        // Arrange
        var request = new UpdateProjectRequest("Updated Name", "Updated Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyName_Fails()
    {
        // Arrange
        var request = new UpdateProjectRequest("", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithNullDescription_Succeeds()
    {
        // Arrange
        var request = new UpdateProjectRequest("Project Name", null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithNameExactly3Characters_Succeeds()
    {
        // Arrange
        var request = new UpdateProjectRequest("ABC", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithNameExceeding255Characters_Fails()
    {
        // Arrange
        var longName = new string('a', 256);
        var request = new UpdateProjectRequest(longName, "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorCode == "MaximumLengthValidator");
    }

    [Fact]
    public async Task Validate_WithDescriptionExceeding1000Characters_Fails()
    {
        // Arrange
        var longDescription = new string('a', 1001);
        var request = new UpdateProjectRequest("Project Name", longDescription);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task Validate_WithDescriptionExactly1000Characters_Succeeds()
    {
        // Arrange
        var description = new string('a', 1000);
        var request = new UpdateProjectRequest("Project Name", description);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithUnicodeCharactersInName_Succeeds()
    {
        // Arrange
        var request = new UpdateProjectRequest("Project Café 日本語", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithSpecialCharactersInName_Succeeds()
    {
        // Arrange
        var request = new UpdateProjectRequest("Project-Name_123 (Test)", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_MultipleValidationErrors_ReturnAllErrors()
    {
        // Arrange
        var longName = new string('a', 256);
        var longDescription = new string('b', 1001);
        var request = new UpdateProjectRequest(longName, longDescription);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
