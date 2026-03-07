using System.Net;
using System.Net.Http.Json;

using BetterTests.Api.Tests.Fixtures;
using BetterTests.Api.Tests.Helpers;
using BetterTests.Application.DTOs;

using FluentAssertions;

using Xunit;

namespace BetterTests.Api.Tests.Controllers;

[Collection("Sequential")]
public class ProjectsControllerTests : IAsyncLifetime
{
    private ApiWebApplicationFactory _factory = null!;

    [Fact]
    public async Task GetProjects_WithNoParams_Returns200WithPaginatedList()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/projects");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<ProjectResponse>>(HttpClientExtensions.JsonOptions);
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty();
        content.Page.Should().Be(1);
        content.PageSize.Should().Be(20);
        content.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetProject_WithExistingId_Returns200WithDetail()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectName = $"Test Project {Guid.NewGuid()}";
        var createRequest = new CreateProjectRequest(projectName, "A test project");
        var createResponse = await client.PostAsJsonAsync("/api/v1/projects", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        var projectId = created!.Id;

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var project = await response.Content.ReadFromJsonAsync<ProjectDetailResponse>(HttpClientExtensions.JsonOptions);
        project.Should().NotBeNull();
        project!.Id.Should().Be(projectId);
        project.Name.Should().Be(projectName);
        project.TestSuites.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProject_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProject_WithValidRequest_Returns201WithLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateProjectRequest("New Project", "Description");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        project.Should().NotBeNull();
        project!.Name.Should().Be("New Project");
        project.Description.Should().Be("Description");
    }

    [Fact]
    public async Task CreateProject_WithInvalidRequest_Returns400()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateProjectRequest("", "Description");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Contain("problem+json");
    }

    [Fact]
    public async Task CreateProject_WithDuplicateName_Returns409()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateProjectRequest("Duplicate Project", "Description");

        await client.PostAsJsonAsync("/api/v1/projects", request);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/projects", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateProject_WithValidRequest_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateProjectRequest("Original Name", "Original Description");
        var createResponse = await client.PostAsJsonAsync("/api/v1/projects", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        var projectId = created!.Id;

        var updateRequest = new UpdateProjectRequest("Updated Name", "Updated Description");

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/projects/{projectId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync($"/api/v1/projects/{projectId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<ProjectDetailResponse>(HttpClientExtensions.JsonOptions);
        updated!.Name.Should().Be("Updated Name");
        updated.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task UpdateProject_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateProjectRequest("Updated Name", "Updated Description");

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/projects/{nonExistentId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProject_WithInvalidRequest_Returns400()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateProjectRequest("Original Name", "Original Description");
        var createResponse = await client.PostAsJsonAsync("/api/v1/projects", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        var projectId = created!.Id;

        var updateRequest = new UpdateProjectRequest("", "Description");

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/projects/{projectId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProject_WithExistingId_Returns204()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createRequest = new CreateProjectRequest("To Delete", "Description");
        var createResponse = await client.PostAsJsonAsync("/api/v1/projects", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        var projectId = created!.Id;

        // Act
        var response = await client.DeleteAsync($"/api/v1/projects/{projectId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/projects/{projectId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProject_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/v1/projects/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    Task IAsyncLifetime.InitializeAsync()
    {
        _factory = new ApiWebApplicationFactory();
        return Task.CompletedTask;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _factory.DisposeAsync();
    }
}
