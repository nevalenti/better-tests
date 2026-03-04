using System.Net;
using System.Net.Http.Json;
using Api.Tests.Fixtures;
using Api.Tests.Helpers;
using BetterTests.Application.DTOs;
using BetterTests.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Api.Tests.Controllers;

[Collection("Sequential")]
public class TestRunsControllerTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory = factory;

    private async Task<Guid> CreateProjectAsync(HttpClient client)
    {
        var request = new CreateProjectRequest($"Project-{Guid.NewGuid()}", "Description");
        var response = await client.PostAsJsonAsync("/api/v1/projects", request);
        var created = await response.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    [Fact]
    public async Task GetTestRuns_WithValidProjectId_Returns200WithPaginatedList()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}/runs");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<TestRunResponse>>(HttpClientExtensions.JsonOptions);
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty();
        content.Page.Should().Be(1);
        content.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GetTestRuns_WithStatusFilter_Returns200WithFiltered()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var createRequest = new CreateTestRunRequest($"Run-{Guid.NewGuid()}", "Production", null, null, TestRunStatus.InProgress);
        await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/runs", createRequest);

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}/runs?status={TestRunStatus.Completed}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<TestRunResponse>>(HttpClientExtensions.JsonOptions);
        content!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTestRun_WithExistingId_Returns200WithDetail()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var runName = $"Run-{Guid.NewGuid()}";
        var createRequest = new CreateTestRunRequest(runName, "Staging", null, null, TestRunStatus.InProgress);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/runs", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestRunResponse>(HttpClientExtensions.JsonOptions);
        var runId = created!.Id;

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}/runs/{runId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var run = await response.Content.ReadFromJsonAsync<TestRunResponse>(HttpClientExtensions.JsonOptions);
        run.Should().NotBeNull();
        run!.Id.Should().Be(runId);
        run.Name.Should().Be(runName);
        run.ExecutedBy.Should().Be("test-user");
    }

    [Fact]
    public async Task GetTestRun_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var nonExistentRunId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}/runs/{nonExistentRunId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTestRun_WithValidRequest_Returns201WithLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var request = new CreateTestRunRequest("New Run", "Production", null, null, TestRunStatus.InProgress);

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/runs", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var run = await response.Content.ReadFromJsonAsync<TestRunResponse>(HttpClientExtensions.JsonOptions);
        run.Should().NotBeNull();
        run!.Name.Should().Be("New Run");
        run.Environment.Should().Be("Production");
        run.ExecutedBy.Should().Be("test-user");
    }

    [Fact]
    public async Task CreateTestRun_WithInvalidRequest_Returns400()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var request = new CreateTestRunRequest("", "Production", null, null, TestRunStatus.InProgress);

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/runs", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTestRun_WithMissingProject_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistentProjectId = Guid.NewGuid();

        var request = new CreateTestRunRequest("New Run", "Production", null, null, TestRunStatus.InProgress);

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{nonExistentProjectId}/runs", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTestRun_SetsExecutedByFromClaims()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var request = new CreateTestRunRequest("Auth Run", "Production", null, null, TestRunStatus.InProgress);

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/runs", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var run = await response.Content.ReadFromJsonAsync<TestRunResponse>(HttpClientExtensions.JsonOptions);
        run!.ExecutedBy.Should().Be("test-user");
    }

    [Fact]
    public async Task UpdateTestRun_WithValidRequest_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var createRequest = new CreateTestRunRequest("Original", "Staging", null, null, TestRunStatus.InProgress);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/runs", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestRunResponse>(HttpClientExtensions.JsonOptions);
        var runId = created!.Id;

        var updateRequest = new UpdateTestRunRequest("Updated", "Production", null, null, TestRunStatus.Completed);

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/projects/{projectId}/runs/{runId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync($"/api/v1/projects/{projectId}/runs/{runId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<TestRunResponse>(HttpClientExtensions.JsonOptions);
        updated!.Name.Should().Be("Updated");
        updated.Environment.Should().Be("Production");
        updated.Status.Should().Be(TestRunStatus.Completed);
    }

    [Fact]
    public async Task UpdateTestRun_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var nonExistentRunId = Guid.NewGuid();

        var updateRequest = new UpdateTestRunRequest("Updated", "Production", null, null, TestRunStatus.Completed);

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/projects/{projectId}/runs/{nonExistentRunId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestRun_WithExistingId_Returns204()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var createRequest = new CreateTestRunRequest("To Delete", "Production", null, null, TestRunStatus.InProgress);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/runs", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestRunResponse>(HttpClientExtensions.JsonOptions);
        var runId = created!.Id;

        // Act
        var response = await client.DeleteAsync($"/api/v1/projects/{projectId}/runs/{runId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/projects/{projectId}/runs/{runId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestRun_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var nonExistentRunId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/v1/projects/{projectId}/runs/{nonExistentRunId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
