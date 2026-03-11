using BetterTests.Application.DTOs;
using BetterTests.Application.Services;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using FluentAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace BetterTests.Api.Tests.Services;

public class TestSuiteServiceTests
{
    private readonly Mock<ITestSuiteRepository> _testSuiteRepositoryMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly TestSuiteService _service;

    public TestSuiteServiceTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _testSuiteRepositoryMock = new Mock<ITestSuiteRepository>();

        _service = new TestSuiteService(_projectRepositoryMock.Object, _testSuiteRepositoryMock.Object, NullLogger<TestSuiteService>.Instance);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WithExistingProject_ReturnsSuites()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var suites = new List<TestSuite>
        {
            new() { Id = suiteId, Name = "Auth Tests", ProjectId = projectId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetPagedByProjectIdAsync(projectId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((suites, 1));

        // Act
        var result = await _service.GetByProjectIdAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Auth Tests");
        result.TotalCount.Should().Be(1);

        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.GetPagedByProjectIdAsync(projectId, It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WithNonExistentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByProjectIdAsync(projectId));
    }

    [Fact]
    public async Task GetByProjectIdAsync_WithNoSuites_ReturnsEmptyList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Empty Project" };
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetPagedByProjectIdAsync(projectId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<TestSuite>(), 0));

        // Act
        var result = await _service.GetByProjectIdAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingSuite_ReturnsWithTestCases()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var testSuite = new TestSuite
        {
            Id = suiteId,
            Name = "Auth Tests",
            ProjectId = projectId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TestCases =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Valid Login",
                    SuiteId = suiteId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            ]
        };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetWithCasesAsync(suiteId)).ReturnsAsync(testSuite);

        // Act
        var result = await _service.GetByIdAsync(projectId, suiteId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(suiteId);
        result.TestCases.Should().HaveCount(1);

        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.GetWithCasesAsync(suiteId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentSuite_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetWithCasesAsync(suiteId)).ReturnsAsync((TestSuite?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(projectId, suiteId));
    }

    [Fact]
    public async Task GetByIdAsync_WithSuiteFromDifferentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var testSuite = new TestSuite
        {
            Id = suiteId,
            Name = "Auth Tests",
            ProjectId = otherProjectId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetWithCasesAsync(suiteId)).ReturnsAsync(testSuite);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(projectId, suiteId));
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new CreateTestSuiteRequest("New Suite", "Description");
        var project = new Project { Id = projectId, Name = "Test Project" };
        var testSuite = new TestSuite
        {
            Id = Guid.NewGuid(),
            Name = "New Suite",
            Description = "Description",
            ProjectId = projectId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TestSuite>())).ReturnsAsync(testSuite);
        _testSuiteRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(projectId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Suite");
        result.ProjectId.Should().Be(projectId);

        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TestSuite>()), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new CreateTestSuiteRequest("New Suite", "Description");

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CreateAsync(projectId, request));
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var existingSuite = new TestSuite
        {
            Id = suiteId,
            Name = "Old Name",
            ProjectId = projectId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var updateRequest = new UpdateTestSuiteRequest("Updated Name", "Updated Description");
        var updatedSuite = new TestSuite
        {
            Id = suiteId,
            Name = "Updated Name",
            Description = "Updated Description",
            ProjectId = projectId,
            CreatedAt = existingSuite.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        TestSuite? capturedEntity = null;
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(existingSuite);
        _testSuiteRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TestSuite>()))
            .Callback<TestSuite>(e => capturedEntity = e)
            .ReturnsAsync(updatedSuite);
        _testSuiteRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(projectId, suiteId, updateRequest);

        // Assert
        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.GetByIdAsync(suiteId), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TestSuite>()), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        capturedEntity.Should().NotBeNull();
        capturedEntity!.Name.Should().Be("Updated Name");
        capturedEntity.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentSuite_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var updateRequest = new UpdateTestSuiteRequest("Name", null);

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync((TestSuite?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(projectId, suiteId, updateRequest));
    }

    [Fact]
    public async Task UpdateAsync_WithSuiteFromDifferentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var existingSuite = new TestSuite
        {
            Id = suiteId,
            Name = "Old Name",
            ProjectId = otherProjectId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var updateRequest = new UpdateTestSuiteRequest("Updated Name", "Description");

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(existingSuite);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(projectId, suiteId, updateRequest));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingSuite_DeletesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var testSuite = new TestSuite { Id = suiteId, Name = "Suite to Delete", ProjectId = projectId };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(testSuite);
        _testSuiteRepositoryMock.Setup(r => r.DeleteAsync(suiteId)).Returns(Task.CompletedTask);
        _testSuiteRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(projectId, suiteId);

        // Assert
        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.GetByIdAsync(suiteId), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.DeleteAsync(suiteId), Times.Once);
        _testSuiteRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentSuite_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync((TestSuite?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(projectId, suiteId));
    }

    [Fact]
    public async Task DeleteAsync_WithSuiteFromDifferentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var suiteId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var testSuite = new TestSuite { Id = suiteId, Name = "Suite to Delete", ProjectId = otherProjectId };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testSuiteRepositoryMock.Setup(r => r.GetByIdAsync(suiteId)).ReturnsAsync(testSuite);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(projectId, suiteId));
    }
}
