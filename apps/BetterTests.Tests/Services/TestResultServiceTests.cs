using BetterTests.Application.DTOs;
using BetterTests.Application.Services;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using FluentAssertions;

using Moq;

using Xunit;

namespace BetterTests.Api.Tests.Services;

public class TestResultServiceTests
{
    private readonly Mock<ITestRunRepository> _testRunRepositoryMock;
    private readonly Mock<ITestResultRepository> _testResultRepositoryMock;
    private readonly TestResultService _service;

    public TestResultServiceTests()
    {
        _testRunRepositoryMock = new Mock<ITestRunRepository>();
        _testResultRepositoryMock = new Mock<ITestResultRepository>();

        _service = new TestResultService(_testRunRepositoryMock.Object, _testResultRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByRunIdAsync_WithExistingRun_ReturnsResults()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };
        var results = new List<TestResult>
        {
            new() { Id = resultId, TestRunId = runId, TestCaseId = caseId, Result = TestResultStatus.Passed, ExecutedBy = "user", ExecutedAt = DateTime.UtcNow }
        };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetPagedByRunIdAsync(runId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((results, 1));

        // Act
        var result = await _service.GetByRunIdAsync(runId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Result.Should().Be(TestResultStatus.Passed);
        result.TotalCount.Should().Be(1);

        _testRunRepositoryMock.Verify(r => r.GetByIdAsync(runId), Times.Once);
        _testResultRepositoryMock.Verify(r => r.GetPagedByRunIdAsync(runId, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetByRunIdAsync_WithNonExistentRun_ThrowsNotFoundException()
    {
        // Arrange
        var runId = Guid.NewGuid();
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync((TestRun?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByRunIdAsync(runId));
    }

    [Fact]
    public async Task GetByRunIdAsync_WithNoResults_ReturnsEmptyList()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Empty Run", Environment = "Dev", ExecutedBy = "user" };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetPagedByRunIdAsync(runId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<TestResult>(), 0));

        // Act
        var result = await _service.GetByRunIdAsync(runId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingResult_ReturnsResult()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };
        var testResult = new TestResult
        {
            Id = resultId,
            TestRunId = runId,
            TestCaseId = caseId,
            Result = TestResultStatus.Passed,
            Comments = "Test passed successfully",
            ExecutedAt = DateTime.UtcNow,
            ExecutedBy = "user@test.com"
        };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetByIdAsync(resultId)).ReturnsAsync(testResult);

        // Act
        var result = await _service.GetByIdAsync(runId, resultId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(resultId);
        result.TestRunId.Should().Be(runId);
        result.Result.Should().Be(TestResultStatus.Passed);

        _testRunRepositoryMock.Verify(r => r.GetByIdAsync(runId), Times.Once);
        _testResultRepositoryMock.Verify(r => r.GetByIdAsync(resultId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentRun_ThrowsNotFoundException()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync((TestRun?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(runId, resultId));
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentResult_ThrowsNotFoundException()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetByIdAsync(resultId)).ReturnsAsync((TestResult?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(runId, resultId));
    }

    [Fact]
    public async Task GetByIdAsync_WithResultFromDifferentRun_ThrowsNotFoundException()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var otherRunId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };
        var testResult = new TestResult { Id = resultId, TestRunId = otherRunId, TestCaseId = caseId, Result = TestResultStatus.Passed, ExecutedBy = "user" };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetByIdAsync(resultId)).ReturnsAsync(testResult);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(runId, resultId));
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesSuccessfully()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var request = new CreateTestResultRequest(caseId, TestResultStatus.Passed, "Test passed", null);
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };
        var testResult = new TestResult
        {
            Id = resultId,
            TestRunId = runId,
            TestCaseId = request.TestCaseId,
            Result = request.Result,
            Comments = request.Comments,
            DefectLink = request.DefectLink,
            ExecutedAt = DateTime.UtcNow,
            ExecutedBy = "user@test.com"
        };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TestResult>())).ReturnsAsync(testResult);
        _testResultRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(runId, request, "user@test.com");

        // Assert
        result.Should().NotBeNull();
        result.TestRunId.Should().Be(runId);
        result.TestCaseId.Should().Be(caseId);
        result.Result.Should().Be(TestResultStatus.Passed);

        _testRunRepositoryMock.Verify(r => r.GetByIdAsync(runId), Times.Once);
        _testResultRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TestResult>()), Times.Once);
        _testResultRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentRun_ThrowsNotFoundException()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var request = new CreateTestResultRequest(Guid.NewGuid(), TestResultStatus.Passed, null, null);

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync((TestRun?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CreateAsync(runId, request, "user@test.com"));
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesSuccessfully()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };
        var existingResult = new TestResult
        {
            Id = resultId,
            TestRunId = runId,
            TestCaseId = caseId,
            Result = TestResultStatus.Passed,
            Comments = "Old comment",
            ExecutedBy = "user"
        };
        var updateRequest = new UpdateTestResultRequest(TestResultStatus.Failed, "Updated comment", "https://defect-link.com");

        TestResult? capturedEntity = null;
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetByIdAsync(resultId)).ReturnsAsync(existingResult);
        _testResultRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TestResult>()))
            .Callback<TestResult>(e => capturedEntity = e)
            .ReturnsAsync(existingResult);
        _testResultRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(runId, resultId, updateRequest);

        // Assert
        _testRunRepositoryMock.Verify(r => r.GetByIdAsync(runId), Times.Once);
        _testResultRepositoryMock.Verify(r => r.GetByIdAsync(resultId), Times.Once);
        _testResultRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TestResult>()), Times.Once);
        _testResultRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        capturedEntity.Should().NotBeNull();
        capturedEntity!.Result.Should().Be(TestResultStatus.Failed);
        capturedEntity.Comments.Should().Be("Updated comment");
        capturedEntity.DefectLink.Should().Be("https://defect-link.com");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentResult_ThrowsNotFoundException()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };
        var updateRequest = new UpdateTestResultRequest(TestResultStatus.Passed, null, null);

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetByIdAsync(resultId)).ReturnsAsync((TestResult?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(runId, resultId, updateRequest));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingResult_DeletesSuccessfully()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };
        var testResult = new TestResult { Id = resultId, TestRunId = runId, TestCaseId = caseId, Result = TestResultStatus.Passed, ExecutedBy = "user" };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetByIdAsync(resultId)).ReturnsAsync(testResult);
        _testResultRepositoryMock.Setup(r => r.DeleteAsync(resultId)).Returns(Task.CompletedTask);
        _testResultRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(runId, resultId);

        // Assert
        _testRunRepositoryMock.Verify(r => r.GetByIdAsync(runId), Times.Once);
        _testResultRepositoryMock.Verify(r => r.GetByIdAsync(resultId), Times.Once);
        _testResultRepositoryMock.Verify(r => r.DeleteAsync(resultId), Times.Once);
        _testResultRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentResult_ThrowsNotFoundException()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetByIdAsync(resultId)).ReturnsAsync((TestResult?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(runId, resultId));
    }

    [Fact]
    public async Task DeleteAsync_WithResultFromDifferentRun_ThrowsNotFoundException()
    {
        // Arrange
        var runId = Guid.NewGuid();
        var otherRunId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var run = new TestRun { Id = runId, Name = "Test Run", Environment = "Dev", ExecutedBy = "user" };
        var testResult = new TestResult { Id = resultId, TestRunId = otherRunId, TestCaseId = caseId, Result = TestResultStatus.Passed, ExecutedBy = "user" };

        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testResultRepositoryMock.Setup(r => r.GetByIdAsync(resultId)).ReturnsAsync(testResult);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(runId, resultId));
    }
}
