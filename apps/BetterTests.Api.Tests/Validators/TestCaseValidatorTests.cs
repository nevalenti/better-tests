using BetterTests.Application.DTOs;
using BetterTests.Application.Validators;
using BetterTests.Domain.Entities;

using FluentAssertions;

using Xunit;

namespace BetterTests.Api.Tests.Validators;

public class CreateTestCaseRequestValidatorTests
{
    private readonly CreateTestCaseRequestValidator _validator;

    public CreateTestCaseRequestValidatorTests()
    {
        _validator = new CreateTestCaseRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_Succeeds()
    {
        // Arrange
        var request = new CreateTestCaseRequest(
            "Valid Login",
            "Test logging in with valid credentials",
            "User account exists",
            "User is authenticated"
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyName_Fails()
    {
        // Arrange
        var request = new CreateTestCaseRequest("", "Description", null, null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNullName_Fails()
    {
        // Arrange
        var request = new CreateTestCaseRequest(null!, "Description", null, null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithNameExceeding500Characters_Fails()
    {
        // Arrange
        var longName = new string('a', 501);
        var request = new CreateTestCaseRequest(longName, null, null, null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameBelowMinLength_Fails()
    {
        // Arrange
        var shortName = "abcd"; // Min length is 5
        var request = new CreateTestCaseRequest(shortName, null, null, null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithValidNameAndOptionalFields_Succeeds()
    {
        // Arrange
        var request = new CreateTestCaseRequest("Test Case", null, null, null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithLongDescriptionExceeding2000_Fails()
    {
        // Arrange
        var longDescription = new string('a', 2001);
        var request = new CreateTestCaseRequest("Test Case", longDescription, null, null);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithAllFieldsMaxLength_Succeeds()
    {
        // Arrange
        var maxName = new string('a', 200);
        var maxDescription = new string('b', 2000);
        var maxPreconditions = new string('c', 2000);
        var maxExpectedResult = new string('d', 2000);

        var request = new CreateTestCaseRequest(
            maxName,
            maxDescription,
            maxPreconditions,
            maxExpectedResult
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_MultipleErrors_ReturnAllErrors()
    {
        // Arrange
        var request = new CreateTestCaseRequest(
            "",
            new string('a', 2001),
            new string('b', 2001),
            new string('c', 2001)
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}

public class CreateTestCaseStepValidatorTests
{
    private readonly CreateTestCaseStepRequestValidator _validator;

    public CreateTestCaseStepValidatorTests()
    {
        _validator = new CreateTestCaseStepRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_Succeeds()
    {
        // Arrange
        var request = new CreateTestCaseStepRequest(
            1,
            "Click login button",
            "User is logged in"
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithZeroStepNumber_Fails()
    {
        // Arrange
        var request = new CreateTestCaseStepRequest(0, "Action", "Result");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithNegativeStepNumber_Fails()
    {
        // Arrange
        var request = new CreateTestCaseStepRequest(-1, "Action", "Result");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptyAction_Fails()
    {
        // Arrange
        var request = new CreateTestCaseStepRequest(1, "", "Result");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptyExpectedResult_Fails()
    {
        // Arrange
        var request = new CreateTestCaseStepRequest(1, "Action", "");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithActionExceeding1000Characters_Fails()
    {
        // Arrange
        var longAction = new string('a', 1001);
        var request = new CreateTestCaseStepRequest(1, longAction, "Result");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithActionExactly1000Characters_Succeeds()
    {
        // Arrange
        var action = new string('a', 1000);
        var request = new CreateTestCaseStepRequest(1, action, "Expected Result");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithActionBelowMinLength_Fails()
    {
        // Arrange
        var shortAction = "abcd"; // Min length is 5
        var request = new CreateTestCaseStepRequest(1, shortAction, "Result");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithExpectedResultBelowMinLength_Fails()
    {
        // Arrange
        var shortResult = "abcd"; // Min length is 5
        var request = new CreateTestCaseStepRequest(1, "Click button", shortResult);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithLargeStepNumber_Succeeds()
    {
        // Arrange
        var request = new CreateTestCaseStepRequest(999, "Action", "Result");

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

public class CreateTestRunValidatorTests
{
    private readonly CreateTestRunRequestValidator _validator;

    public CreateTestRunValidatorTests()
    {
        _validator = new CreateTestRunRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_Succeeds()
    {
        // Arrange
        var request = new CreateTestRunRequest(
            "v2.1.0 Regression Tests",
            "Production",
            null,
            null,
            TestRunStatus.InProgress
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyName_Fails()
    {
        // Arrange
        var request = new CreateTestRunRequest("", "Production", null, null, TestRunStatus.InProgress);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptyEnvironment_Fails()
    {
        // Arrange
        var request = new CreateTestRunRequest("Run Name", "", null, null, TestRunStatus.InProgress);

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithValidStatus_Succeeds()
    {
        // Arrange
        var validStatuses = new[] { TestRunStatus.InProgress, TestRunStatus.Completed, TestRunStatus.Paused };

        foreach (var status in validStatuses)
        {
            var request = new CreateTestRunRequest("Run", "Production", null, null, status);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue($"Status '{status}' should be valid");
        }
    }
}

public class CreateTestResultValidatorTests
{
    private readonly CreateTestResultRequestValidator _validator;

    public CreateTestResultValidatorTests()
    {
        _validator = new CreateTestResultRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_Succeeds()
    {
        // Arrange
        var request = new CreateTestResultRequest(
            Guid.NewGuid(),
            TestResultStatus.Passed,
            "User successfully logged in",
            null
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyComments_Succeeds()
    {
        // Arrange
        var request = new CreateTestResultRequest(
            Guid.NewGuid(),
            TestResultStatus.Passed,
            "",
            null
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithNullTestCaseId_Succeeds()
    {
        // Arrange
        var request = new CreateTestResultRequest(
            null,
            TestResultStatus.Passed,
            "Result logged",
            null
        );

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithValidStatuses_Succeeds()
    {
        // Arrange
        var validStatuses = new[] { TestResultStatus.Passed, TestResultStatus.Failed, TestResultStatus.Skipped, TestResultStatus.Blocked };

        foreach (var status in validStatuses)
        {
            var request = new CreateTestResultRequest(
                Guid.NewGuid(),
                status,
                "Result logged",
                null
            );

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue($"Status '{status}' should be valid");
        }
    }
}
