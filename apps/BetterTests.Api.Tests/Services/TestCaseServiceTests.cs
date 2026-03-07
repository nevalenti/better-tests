using BetterTests.Application.DTOs;
using BetterTests.Application.Services;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BetterTests.Api.Tests.Services;

public class TestCaseServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITestSuiteRepository> _testSuiteRepositoryMock;
    private readonly Mock<ITestCaseRepository> _testCaseRepositoryMock;
    private readonly TestCaseService _service;

    public TestCaseServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _testSuiteRepositoryMock = new Mock<ITestSuiteRepository>();
        _testCaseRepositoryMock = new Mock<ITestCaseRepository>();

        _unitOfWorkMock.Setup(u => u.TestSuites).Returns(_testSuiteRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.TestCases).Returns(_testCaseRepositoryMock.Object);

        _service = new TestCaseService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetBySuiteIdAsync_WithExistingSuite_ReturnsCases()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var cases = new List<TestCase>
        {
            new() { Id = caseId, SuiteId = suiteId, Name = "Test Case 1", Priority = TestCasePriority.High, Status = TestCaseStatus.Active }
        };

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetBySuiteIdAsync(suiteId)).ReturnsAsync(cases);

        // Act
        var result = await _service.GetBySuiteIdAsync(suiteId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Test Case 1");
        result.TotalCount.Should().Be(1);

        _testSuiteRepositoryMock.Verify(r => r.GetByIdAsync(suiteId), Times.Once);
        _testCaseRepositoryMock.Verify(r => r.GetBySuiteIdAsync(suiteId), Times.Once);
    }

    [Fact]
    public async Task GetBySuiteIdAsync_WithNonExistentSuite_ThrowsNotFoundException()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync((TestSuite?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetBySuiteIdAsync(suiteId));
    }

    [Fact]
    public async Task GetBySuiteIdAsync_WithNoCases_ReturnsEmptyList()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Empty Suite" };

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetBySuiteIdAsync(suiteId)).ReturnsAsync([]);

        // Act
        var result = await _service.GetBySuiteIdAsync(suiteId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetBySuiteIdAsync_WithPriorityFilter_ReturnsFilteredCases()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var cases = new List<TestCase>
        {
            new() { Id = Guid.NewGuid(), SuiteId = suiteId, Name = "High Priority", Priority = TestCasePriority.High, Status = TestCaseStatus.Active },
            new() { Id = Guid.NewGuid(), SuiteId = suiteId, Name = "Low Priority", Priority = TestCasePriority.Low, Status = TestCaseStatus.Active }
        };

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetBySuiteIdAsync(suiteId)).ReturnsAsync(cases);

        // Act
        var result = await _service.GetBySuiteIdAsync(suiteId, priority: TestCasePriority.High);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("High Priority");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingCase_ReturnsCaseWithSteps()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var testCase = new TestCase
        {
            Id = caseId,
            SuiteId = suiteId,
            Name = "Test Case",
            Priority = TestCasePriority.Medium,
            Status = TestCaseStatus.Active,
            Steps =
            [
                new() { Id = Guid.NewGuid(), TestCaseId = caseId, StepOrder = 1, Action = "Step 1", ExpectedResult = "Result 1" }
            ]
        };

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetWithStepsAsync(caseId)).ReturnsAsync(testCase);

        // Act
        var result = await _service.GetByIdAsync(suiteId, caseId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(caseId);
        result.Name.Should().Be("Test Case");
        result.Steps.Should().HaveCount(1);

        _testCaseRepositoryMock.Verify(r => r.GetWithStepsAsync(caseId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentCase_ThrowsNotFoundException()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();

        _testCaseRepositoryMock.Setup(r => r.GetWithStepsAsync(caseId)).ReturnsAsync((TestCase?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(suiteId, caseId));
    }

    [Fact]
    public async Task GetByIdAsync_WithCaseFromDifferentSuite_ThrowsNotFoundException()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var otherSuiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var testCase = new TestCase { Id = caseId, SuiteId = otherSuiteId, Name = "Wrong Suite Case" };

        _testCaseRepositoryMock.Setup(r => r.GetWithStepsAsync(caseId)).ReturnsAsync(testCase);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(suiteId, caseId));
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesSuccessfully()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var request = new CreateTestCaseRequest(
            "New Test Case",
            "Description",
            "Preconditions",
            "Postconditions",
            TestCasePriority.High,
            TestCaseStatus.Active
        );
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var testCase = new TestCase
        {
            Id = Guid.NewGuid(),
            SuiteId = suiteId,
            Name = request.Name,
            Description = request.Description,
            Priority = request.Priority,
            Status = request.Status
        };

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TestCase>())).ReturnsAsync(testCase);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(suiteId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Test Case");
        result.SuiteId.Should().Be(suiteId);

        _testSuiteRepositoryMock.Verify(r => r.GetByIdAsync(suiteId), Times.Once);
        _testCaseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TestCase>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentSuite_ThrowsNotFoundException()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var request = new CreateTestCaseRequest("Test", null, null, null, TestCasePriority.Medium, TestCaseStatus.Active);

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync((TestSuite?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CreateAsync(suiteId, request));
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesSuccessfully()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var existingCase = new TestCase
        {
            Id = caseId,
            SuiteId = suiteId,
            Name = "Old Name",
            Priority = TestCasePriority.Low,
            Status = TestCaseStatus.Active
        };
        var updateRequest = new UpdateTestCaseRequest("Updated Name", null, null, null, TestCasePriority.High, TestCaseStatus.Active);

        TestCase? capturedEntity = null;
        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(existingCase);
        _testCaseRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TestCase>()))
            .Callback<TestCase>(e => capturedEntity = e)
            .ReturnsAsync(existingCase);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(suiteId, caseId, updateRequest);

        // Assert
        _testSuiteRepositoryMock.Verify(r => r.GetByIdAsync(suiteId), Times.Once);
        _testCaseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
        _testCaseRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TestCase>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);

        capturedEntity.Should().NotBeNull();
        capturedEntity!.Name.Should().Be("Updated Name");
        capturedEntity.Priority.Should().Be(TestCasePriority.High);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentCase_ThrowsNotFoundException()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var updateRequest = new UpdateTestCaseRequest("Name", null, null, null, TestCasePriority.Medium, TestCaseStatus.Active);

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync((TestCase?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(suiteId, caseId, updateRequest));
    }

    [Fact]
    public async Task UpdateAsync_WithCaseFromDifferentSuite_ThrowsNotFoundException()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var otherSuiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var existingCase = new TestCase { Id = caseId, SuiteId = otherSuiteId, Name = "Wrong Suite Case" };
        var updateRequest = new UpdateTestCaseRequest("Updated Name", null, null, null, TestCasePriority.High, TestCaseStatus.Active);

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(existingCase);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(suiteId, caseId, updateRequest));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingCase_DeletesSuccessfully()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var testCase = new TestCase { Id = caseId, SuiteId = suiteId, Name = "Case to Delete" };

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);
        _testCaseRepositoryMock.Setup(r => r.DeleteAsync(caseId)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(suiteId, caseId);

        // Assert
        _testSuiteRepositoryMock.Verify(r => r.GetByIdAsync(suiteId), Times.Once);
        _testCaseRepositoryMock.Verify(r => r.GetByIdAsync(caseId), Times.Once);
        _testCaseRepositoryMock.Verify(r => r.DeleteAsync(caseId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentCase_ThrowsNotFoundException()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync((TestCase?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(suiteId, caseId));
    }

    [Fact]
    public async Task DeleteAsync_WithCaseFromDifferentSuite_ThrowsNotFoundException()
    {
        // Arrange
        var suiteId = Guid.NewGuid();
        var otherSuiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var suite = new TestSuite { Id = suiteId, Name = "Test Suite" };
        var testCase = new TestCase { Id = caseId, SuiteId = otherSuiteId, Name = "Wrong Suite Case" };

        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(suite);
        _testCaseRepositoryMock.Setup(r => r.GetByIdAsync(caseId)).ReturnsAsync(testCase);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(suiteId, caseId));
    }
}
