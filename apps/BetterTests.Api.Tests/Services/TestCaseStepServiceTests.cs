using BetterTests.Application.DTOs;
using BetterTests.Application.Services;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BetterTests.Api.Tests.Services;

public class TestCaseStepServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITestCaseRepository> _testCaseRepositoryMock;
    private readonly Mock<ITestCaseStepRepository> _testCaseStepRepositoryMock;
    private readonly TestCaseStepService _service;

    public TestCaseStepServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _testCaseRepositoryMock = new Mock<ITestCaseRepository>();
        _testCaseStepRepositoryMock = new Mock<ITestCaseStepRepository>();

        _unitOfWorkMock.Setup(u => u.TestCases).Returns(_testCaseRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.TestCaseSteps).Returns(_testCaseStepRepositoryMock.Object);

        _service = new TestCaseStepService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByCaseIdAsync_WithExistingCase_ReturnsSteps()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };
        var steps = new List<TestCaseStep>
        {
            new() { Id = stepId, TestCaseId = caseId, StepOrder = 1, Action = "Step 1 Action", ExpectedResult = "Step 1 Result" }
        };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByCaseIdAsync(caseId)).ReturnsAsync(steps);

        // Act
        var result = await _service.GetByCaseIdAsync(caseId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().StepOrder.Should().Be(1);
        result.Items.First().Action.Should().Be("Step 1 Action");
        result.TotalCount.Should().Be(1);

        _testCaseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
        _testCaseStepRepositoryMock.Verify(r => r.GetByCaseIdAsync(caseId), Times.Once);
    }

    [Fact]
    public async Task GetByCaseIdAsync_WithNonExistentCase_ThrowsNotFoundException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync((TestCase?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByCaseIdAsync(caseId));
    }

    [Fact]
    public async Task GetByCaseIdAsync_WithNoSteps_ReturnsEmptyList()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Case Without Steps" };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByCaseIdAsync(caseId)).ReturnsAsync([]);

        // Act
        var result = await _service.GetByCaseIdAsync(caseId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingStep_ReturnsStep()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };
        var step = new TestCaseStep
        {
            Id = stepId,
            TestCaseId = caseId,
            StepOrder = 1,
            Action = "Do something",
            ExpectedResult = "Expected result"
        };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByIdAsync(stepId)).ReturnsAsync(step);

        // Act
        var result = await _service.GetByIdAsync(caseId, stepId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(stepId);
        result.StepOrder.Should().Be(1);
        result.Action.Should().Be("Do something");

        _testCaseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
        _testCaseStepRepositoryMock.Verify(r => r.GetByIdAsync(stepId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentCase_ThrowsNotFoundException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync((TestCase?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(caseId, stepId));
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentStep_ThrowsNotFoundException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByIdAsync(stepId)).ReturnsAsync((TestCaseStep?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(caseId, stepId));
    }

    [Fact]
    public async Task GetByIdAsync_WithStepFromDifferentCase_ThrowsNotFoundException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var otherCaseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };
        var step = new TestCaseStep { Id = stepId, TestCaseId = otherCaseId, StepOrder = 1, Action = "Action", ExpectedResult = "Result" };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByIdAsync(stepId)).ReturnsAsync(step);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(caseId, stepId));
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesSuccessfully()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var request = new CreateTestCaseStepRequest(1, "Perform action", "Expected outcome");
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };
        var step = new TestCaseStep
        {
            Id = Guid.NewGuid(),
            TestCaseId = caseId,
            StepOrder = request.StepOrder,
            Action = request.Action,
            ExpectedResult = request.ExpectedResult
        };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TestCaseStep>())).ReturnsAsync(step);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(caseId, request);

        // Assert
        result.Should().NotBeNull();
        result.TestCaseId.Should().Be(caseId);
        result.StepOrder.Should().Be(1);
        result.Action.Should().Be("Perform action");

        _testCaseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
        _testCaseStepRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TestCaseStep>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentCase_ThrowsNotFoundException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var request = new CreateTestCaseStepRequest(1, "Action", "Result");

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync((TestCase?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CreateAsync(caseId, request));
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesSuccessfully()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };
        var existingStep = new TestCaseStep
        {
            Id = stepId,
            TestCaseId = caseId,
            StepOrder = 1,
            Action = "Old Action",
            ExpectedResult = "Old Result"
        };
        var updateRequest = new UpdateTestCaseStepRequest(2, "New Action", "New Result");

        TestCaseStep? capturedEntity = null;
        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByIdAsync(stepId)).ReturnsAsync(existingStep);
        _testCaseStepRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TestCaseStep>()))
            .Callback<TestCaseStep>(e => capturedEntity = e)
            .ReturnsAsync(existingStep);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(caseId, stepId, updateRequest);

        // Assert
        _testCaseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
        _testCaseStepRepositoryMock.Verify(r => r.GetByIdAsync(stepId), Times.Once);
        _testCaseStepRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TestCaseStep>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);

        capturedEntity.Should().NotBeNull();
        capturedEntity!.StepOrder.Should().Be(2);
        capturedEntity.Action.Should().Be("New Action");
        capturedEntity.ExpectedResult.Should().Be("New Result");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentStep_ThrowsNotFoundException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };
        var updateRequest = new UpdateTestCaseStepRequest(1, "Action", "Result");

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByIdAsync(stepId)).ReturnsAsync((TestCaseStep?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(caseId, stepId, updateRequest));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingStep_DeletesSuccessfully()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };
        var step = new TestCaseStep { Id = stepId, TestCaseId = caseId, StepOrder = 1, Action = "Action", ExpectedResult = "Result" };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByIdAsync(stepId)).ReturnsAsync(step);
        _testCaseStepRepositoryMock.Setup(r => r.DeleteAsync(stepId)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(caseId, stepId);

        // Assert
        _testCaseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
        _testCaseStepRepositoryMock.Verify(r => r.GetByIdAsync(stepId), Times.Once);
        _testCaseStepRepositoryMock.Verify(r => r.DeleteAsync(stepId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentStep_ThrowsNotFoundException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByIdAsync(stepId)).ReturnsAsync((TestCaseStep?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(caseId, stepId));
    }

    [Fact]
    public async Task DeleteAsync_WithStepFromDifferentCase_ThrowsNotFoundException()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var otherCaseId = Guid.NewGuid();
        var stepId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, Name = "Test Case" };
        var step = new TestCaseStep { Id = stepId, TestCaseId = otherCaseId, StepOrder = 1, Action = "Action", ExpectedResult = "Result" };

        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseStepRepositoryMock.Setup(r => r.GetByIdAsync(stepId)).ReturnsAsync(step);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(caseId, stepId));
    }
}
