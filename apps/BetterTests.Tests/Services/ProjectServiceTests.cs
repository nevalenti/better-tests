using AutoMapper;

using BetterTests.Application.DTOs;
using BetterTests.Application.Services;
using BetterTests.Domain.Entities;
using BetterTests.Domain.Exceptions;
using BetterTests.Domain.Interfaces;

using FluentAssertions;

using Moq;

using Xunit;

namespace BetterTests.Api.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new ProjectService(_projectRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_WithProjects_ReturnsProjectList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var projects = new List<Project>
        {
            new() { Id = projectId, Name = "Project 1", Description = "Test project" }
        };

        var responses = new List<ProjectResponse>
        {
            new(projectId, "Project 1", "Test project", DateTime.UtcNow, DateTime.UtcNow)
        };

        _projectRepositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((projects, 1));
        _mapperMock.Setup(m => m.Map<IEnumerable<ProjectResponse>>(It.IsAny<IEnumerable<Project>>()))
            .Returns(responses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("Project 1");
        result.TotalCount.Should().Be(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);

        _projectRepositoryMock.Verify(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithNoProjects_ReturnsEmptyList()
    {
        // Arrange
        _projectRepositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((new List<Project>(), 0));
        _mapperMock.Setup(m => m.Map<IEnumerable<ProjectResponse>>(It.IsAny<IEnumerable<Project>>()))
            .Returns([]);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingProject_ReturnsProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            Description = "A test project",
            TestSuites = []
        };

        var response = new ProjectDetailResponse(
            projectId,
            "Test Project",
            "A test project",
            DateTime.UtcNow,
            DateTime.UtcNow,
            []
        );

        _projectRepositoryMock.Setup(r => r.GetWithSuitesAsync(projectId)).ReturnsAsync(project);
        _mapperMock.Setup(m => m.Map<ProjectDetailResponse>(project)).Returns(response);

        // Act
        var result = await _service.GetByIdAsync(projectId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(projectId);
        result.Name.Should().Be("Test Project");

        _projectRepositoryMock.Verify(r => r.GetWithSuitesAsync(projectId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentProject_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _projectRepositoryMock.Setup(r => r.GetWithSuitesAsync(projectId)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(projectId));
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesAndReturnsProject()
    {
        // Arrange
        var request = new CreateProjectRequest("New Project", "Description");
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            Id = projectId,
            Name = "New Project",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var response = new ProjectResponse(
            projectId,
            "New Project",
            "Description",
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        _projectRepositoryMock.Setup(r => r.GetByNameAsync(request.Name)).ReturnsAsync((Project?)null);

        _mapperMock.Setup(m => m.Map<Project>(request)).Returns(project);
        _mapperMock.Setup(m => m.Map<ProjectResponse>(project)).Returns(response);

        _projectRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Project>())).ReturnsAsync(project);
        _projectRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Project");
        result.Description.Should().Be("Description");

        _projectRepositoryMock.Verify(r => r.GetByNameAsync(request.Name), Times.Once);
        _projectRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
        _projectRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ThrowsDuplicateEntityException()
    {
        // Arrange
        var request = new CreateProjectRequest("Existing Project", "Description");
        var existingProject = new Project { Name = "Existing Project" };

        _projectRepositoryMock.Setup(r => r.GetByNameAsync(request.Name)).ReturnsAsync(existingProject);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => _service.CreateAsync(request));

        _projectRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingProject_UpdatesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project
        {
            Id = projectId,
            Name = "Old Name",
            Description = "Old Description"
        };

        var updateRequest = new UpdateProjectRequest("Updated Name", "Updated Description");

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(existingProject);
        _projectRepositoryMock.Setup(r => r.GetByNameAsync(updateRequest.Name)).ReturnsAsync((Project?)null);
        _mapperMock.Setup(m => m.Map(updateRequest, existingProject));
        _projectRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>())).ReturnsAsync(existingProject);
        _projectRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(projectId, updateRequest);

        // Assert
        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _projectRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Once);
        _projectRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentProject_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var updateRequest = new UpdateProjectRequest("Name", null);

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(projectId, updateRequest));
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateName_ThrowsDuplicateEntityException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project { Id = projectId, Name = "Old Name" };
        var otherProject = new Project { Id = Guid.NewGuid(), Name = "New Name" };
        var updateRequest = new UpdateProjectRequest("New Name", null);

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(existingProject);
        _projectRepositoryMock.Setup(r => r.GetByNameAsync("New Name")).ReturnsAsync(otherProject);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => _service.UpdateAsync(projectId, updateRequest));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingProject_DeletesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, Name = "Project to Delete" };

        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _projectRepositoryMock.Setup(r => r.DeleteAsync(projectId)).Returns(Task.CompletedTask);
        _projectRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(projectId);

        // Assert
        _projectRepositoryMock.Verify(r => r.GetByIdAsync(projectId), Times.Once);
        _projectRepositoryMock.Verify(r => r.DeleteAsync(projectId), Times.Once);
        _projectRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentProject_ThrowsEntityNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _projectRepositoryMock.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Project?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(projectId));
    }
}
