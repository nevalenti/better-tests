using BetterTests.Application.DTOs;
using BetterTests.Application.Services;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using FluentAssertions;

using Moq;

using Xunit;

namespace BetterTests.Api.Tests.Services;

public class TestRunServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ITestRunRepository> _testRunRepositoryMock;
    private readonly TestRunService _service;

    public TestRunServiceTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _testRunRepositoryMock = new Mock<ITestRunRepository>();

        _service = new TestRunService(_projectRepositoryMock.Object, _testRunRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WithExistingProject_ReturnsRuns()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var runs = new List<TestRun>
        {
            new() { Id = runId, ProjectId = projectId, Name = "Run 1", Environment = "Dev", ExecutedBy = "user@test.com", Status = TestRunStatus.InProgress }
        };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock
            .Setup(r => r.GetPagedByProjectIdAsync(projectId, It.IsAny<int>(), It.IsAny<int>(), null))
            .ReturnsAsync((runs, 1));

        // Act
        var result = await _service.GetByProjectIdAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Run 1");
        result.Items.First().Status.Should().Be(TestRunStatus.InProgress);
        result.TotalCount.Should().Be(1);

        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testRunRepositoryMock.Verify(r => r.GetPagedByProjectIdAsync(projectId, It.IsAny<int>(), It.IsAny<int>(), null), Times.Once);
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
    public async Task GetByProjectIdAsync_WithNoRuns_ReturnsEmptyList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Empty Project" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetPagedByProjectIdAsync(projectId, It.IsAny<int>(), It.IsAny<int>(), null))
            .ReturnsAsync((new List<TestRun>(), 0));

        // Act
        var result = await _service.GetByProjectIdAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WithStatusFilter_ReturnsFilteredRuns()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var completedRuns = new List<TestRun>
        {
            new() { Id = Guid.NewGuid(), ProjectId = projectId, Name = "Passed Run", Environment = "Dev", Status = TestRunStatus.Completed, ExecutedBy = "user" }
        };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetPagedByProjectIdAsync(projectId, It.IsAny<int>(), It.IsAny<int>(), TestRunStatus.Completed))
            .ReturnsAsync((completedRuns, 1));

        // Act
        var result = await _service.GetByProjectIdAsync(projectId, status: TestRunStatus.Completed);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Passed Run");
        result.Items.First().Status.Should().Be(TestRunStatus.Completed);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingRun_ReturnsRun()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var run = new TestRun
        {
            Id = runId,
            ProjectId = projectId,
            Name = "Test Run",
            Environment = "Prod",
            ExecutedBy = "user@test.com",
            Status = TestRunStatus.InProgress
        };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);

        // Act
        var result = await _service.GetByIdAsync(projectId, runId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(runId);
        result.Name.Should().Be("Test Run");
        result.ProjectId.Should().Be(projectId);

        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testRunRepositoryMock.Verify(r => r.GetByIdAsync(runId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(projectId, runId));
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentRun_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync((TestRun?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(projectId, runId));
    }

    [Fact]
    public async Task GetByIdAsync_WithRunFromDifferentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var run = new TestRun { Id = runId, ProjectId = otherProjectId, Name = "Wrong Project Run", Environment = "Dev", ExecutedBy = "user" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(projectId, runId));
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var request = new CreateTestRunRequest("New Run", "Staging", now, null, TestRunStatus.InProgress);
        var project = new Project { Id = projectId, Name = "Test Project" };
        var run = new TestRun
        {
            Id = runId,
            ProjectId = projectId,
            Name = request.Name,
            Environment = request.Environment,
            ExecutedBy = "user@test.com",
            StartedAt = request.StartedAt,
            CompletedAt = request.CompletedAt,
            Status = request.Status
        };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TestRun>())).ReturnsAsync(run);
        _testRunRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(projectId, request, "user@test.com");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Run");
        result.ProjectId.Should().Be(projectId);
        result.ExecutedBy.Should().Be("user@test.com");

        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testRunRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TestRun>()), Times.Once);
        _testRunRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new CreateTestRunRequest("Run", "Dev", DateTime.UtcNow, null, TestRunStatus.InProgress);

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CreateAsync(projectId, request, "user@test.com"));
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_UpdatesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var existingRun = new TestRun
        {
            Id = runId,
            ProjectId = projectId,
            Name = "Old Name",
            Environment = "Dev",
            Status = TestRunStatus.InProgress,
            ExecutedBy = "user"
        };
        var updateRequest = new UpdateTestRunRequest("Updated Name", "Prod", DateTime.UtcNow, DateTime.UtcNow, TestRunStatus.Completed);

        TestRun? capturedEntity = null;
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(existingRun);
        _testRunRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TestRun>()))
            .Callback<TestRun>(e => capturedEntity = e)
            .ReturnsAsync(existingRun);
        _testRunRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(projectId, runId, updateRequest);

        // Assert
        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testRunRepositoryMock.Verify(r => r.GetByIdAsync(runId), Times.Once);
        _testRunRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TestRun>()), Times.Once);
        _testRunRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        capturedEntity.Should().NotBeNull();
        capturedEntity!.Name.Should().Be("Updated Name");
        capturedEntity.Environment.Should().Be("Prod");
        capturedEntity.Status.Should().Be(TestRunStatus.Completed);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentRun_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var updateRequest = new UpdateTestRunRequest("Name", "Dev", DateTime.UtcNow, null, TestRunStatus.InProgress);

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync((TestRun?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(projectId, runId, updateRequest));
    }

    [Fact]
    public async Task UpdateAsync_WithRunFromDifferentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var existingRun = new TestRun { Id = runId, ProjectId = otherProjectId, Name = "Wrong Project Run", Environment = "Dev", ExecutedBy = "user" };
        var updateRequest = new UpdateTestRunRequest("Updated Name", "Prod", DateTime.UtcNow, DateTime.UtcNow, TestRunStatus.Completed);

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(existingRun);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(projectId, runId, updateRequest));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingRun_DeletesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var run = new TestRun { Id = runId, ProjectId = projectId, Name = "Run to Delete", Environment = "Dev", ExecutedBy = "user" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);
        _testRunRepositoryMock.Setup(r => r.DeleteAsync(runId)).Returns(Task.CompletedTask);
        _testRunRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(projectId, runId);

        // Assert
        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _testRunRepositoryMock.Verify(r => r.GetByIdAsync(runId), Times.Once);
        _testRunRepositoryMock.Verify(r => r.DeleteAsync(runId), Times.Once);
        _testRunRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentRun_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync((TestRun?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(projectId, runId));
    }

    [Fact]
    public async Task DeleteAsync_WithRunFromDifferentProject_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Test Project" };
        var run = new TestRun { Id = runId, ProjectId = otherProjectId, Name = "Run to Delete", Environment = "Dev", ExecutedBy = "user" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _testRunRepositoryMock.Setup(r => r.GetByIdAsync(runId)).ReturnsAsync(run);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(projectId, runId));
    }
}
