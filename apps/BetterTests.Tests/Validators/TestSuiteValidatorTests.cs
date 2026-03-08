using BetterTests.Application.DTOs;
using BetterTests.Application.Validators;

using FluentAssertions;

using Xunit;

namespace BetterTests.Tests.Validators;

public class CreateTestSuiteRequestValidatorTests
{
    private readonly CreateTestSuiteRequestValidator _validator;

    public CreateTestSuiteRequestValidatorTests()
    {
        _validator = new CreateTestSuiteRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidName_Succeeds()
    {
        // Arrange
        var request = new CreateTestSuiteRequest("Valid Suite Name", "Description");

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
        var request = new CreateTestSuiteRequest("", "Description");

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
        var request = new CreateTestSuiteRequest(null!, "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameLessThan3Characters_FailsWithMinLengthError()
    {
        // Arrange
        var request = new CreateTestSuiteRequest("AB", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameExceeding255Characters_FailsWithMaxLengthError()
    {
        // Arrange
        var longName = new string('a', 256);
        var request = new CreateTestSuiteRequest(longName, "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameExactly3Characters_Succeeds()
    {
        // Arrange
        var request = new CreateTestSuiteRequest("ABC", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithNameExactly255Characters_Succeeds()
    {
        // Arrange
        var name = new string('a', 255);
        var request = new CreateTestSuiteRequest(name, "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithNullDescription_Succeeds()
    {
        // Arrange
        var request = new CreateTestSuiteRequest("Suite Name", null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyDescription_Succeeds()
    {
        // Arrange
        var request = new CreateTestSuiteRequest("Suite Name", "");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithDescriptionExceeding1000Characters_FailsWithMaxLengthError()
    {
        // Arrange
        var longDescription = new string('a', 1001);
        var request = new CreateTestSuiteRequest("Suite Name", longDescription);

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
        var request = new CreateTestSuiteRequest("Suite Name", description);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

public class UpdateTestSuiteRequestValidatorTests
{
    private readonly UpdateTestSuiteRequestValidator _validator;

    public UpdateTestSuiteRequestValidatorTests()
    {
        _validator = new UpdateTestSuiteRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidUpdateRequest_Succeeds()
    {
        // Arrange
        var request = new UpdateTestSuiteRequest("Updated Suite Name", "Updated Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyName_Fails()
    {
        // Arrange
        var request = new UpdateTestSuiteRequest("", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithNameBelowMinLength_Fails()
    {
        // Arrange
        var request = new UpdateTestSuiteRequest("AB", "Description");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNullDescription_Succeeds()
    {
        // Arrange
        var request = new UpdateTestSuiteRequest("Suite Name", null);

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
        var request = new UpdateTestSuiteRequest(longName, longDescription);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
