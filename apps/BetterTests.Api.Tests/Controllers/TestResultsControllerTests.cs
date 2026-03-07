using System.Net;
using System.Net.Http.Json;

using BetterTests.Api.Tests.Fixtures;
using BetterTests.Api.Tests.Helpers;
using BetterTests.Application.DTOs;
using BetterTests.Domain.Entities;

using FluentAssertions;

using Xunit;

namespace BetterTests.Api.Tests.Controllers;

[Collection("Sequential")]
public class TestResultsControllerTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory = factory;

    private async Task<Guid> CreateProjectAsync(HttpClient client)
    {
        var request = new CreateProjectRequest($"Project-{Guid.NewGuid()}", "Description");
        var response = await client.PostAsJsonAsync("/api/v1/projects", request);
        var created = await response.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    private async Task<Guid> CreateTestRunAsync(HttpClient client, Guid projectId)
    {
        var request = new CreateTestRunRequest($"Run-{Guid.NewGuid()}", "Production", null, null, TestRunStatus.InProgress);
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/runs", request);
        var created = await response.Content.ReadFromJsonAsync<TestRunResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    [Fact]
    public async Task GetTestResults_WithValidRunId_Returns200WithPaginatedList()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        // Act
        var response = await client.GetAsync($"/api/v1/runs/{runId}/results");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<TestResultResponse>>(HttpClientExtensions.JsonOptions);
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty();
        content.Page.Should().Be(1);
        content.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GetTestResult_WithExistingId_Returns200WithDetail()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        var createRequest = new CreateTestResultRequest(null, TestResultStatus.Passed, "Test passed successfully", null);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/runs/{runId}/results", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        var resultId = created!.Id;

        // Act
        var response = await client.GetAsync($"/api/v1/runs/{runId}/results/{resultId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        result.Should().NotBeNull();
        result!.Id.Should().Be(resultId);
        result.Result.Should().Be(TestResultStatus.Passed);
    }

    [Fact]
    public async Task GetTestResult_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);
        var nonExistentResultId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/runs/{runId}/results/{nonExistentResultId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTestResult_WithValidRequest_Returns201WithLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        var request = new CreateTestResultRequest(null, TestResultStatus.Passed, "Result comment", null);

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/runs/{runId}/results", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var result = await response.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        result.Should().NotBeNull();
        result!.Result.Should().Be(TestResultStatus.Passed);
        result.ExecutedBy.Should().Be("test-user");
    }

    [Fact]
    public async Task CreateTestResult_SetsExecutedByFromClaims()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        var request = new CreateTestResultRequest(null, TestResultStatus.Failed, "Failed test", null);

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/runs/{runId}/results", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        result!.ExecutedBy.Should().Be("test-user");
    }

    [Fact]
    public async Task CreateTestResult_SetsExecutedAtServerSide()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        var request = new CreateTestResultRequest(null, TestResultStatus.Skipped, "Skipped", null);
        var beforeRequest = DateTime.UtcNow;

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/runs/{runId}/results", request);
        var afterRequest = DateTime.UtcNow;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        result!.ExecutedAt.Should().BeOnOrAfter(beforeRequest);
        result.ExecutedAt.Should().BeOnOrBefore(afterRequest.AddSeconds(1));
    }

    [Fact]
    public async Task CreateTestResult_WithDifferentStatuses_Returns201()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        var statuses = new[] { TestResultStatus.Passed, TestResultStatus.Failed, TestResultStatus.Skipped, TestResultStatus.Blocked };

        // Act & Assert
        foreach (var status in statuses)
        {
            var request = new CreateTestResultRequest(null, status, $"Result: {status}", null);
            var response = await client.PostAsJsonAsync($"/api/v1/runs/{runId}/results", request);
            response.StatusCode.Should().Be(HttpStatusCode.Created, $"Status {status} should create successfully");
        }
    }

    [Fact]
    public async Task UpdateTestResult_WithValidRequest_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        var createRequest = new CreateTestResultRequest(null, TestResultStatus.Passed, "Original comment", null);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/runs/{runId}/results", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        var resultId = created!.Id;

        var updateRequest = new UpdateTestResultRequest(TestResultStatus.Failed, "Updated comment", "https://defect.tracker/123");

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/runs/{runId}/results/{resultId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync($"/api/v1/runs/{runId}/results/{resultId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        updated!.Result.Should().Be(TestResultStatus.Failed);
        updated.Comments.Should().Be("Updated comment");
        updated.DefectLink.Should().Be("https://defect.tracker/123");
    }

    [Fact]
    public async Task UpdateTestResult_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);
        var nonExistentResultId = Guid.NewGuid();

        var updateRequest = new UpdateTestResultRequest(TestResultStatus.Failed, "Comment", null);

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/runs/{runId}/results/{nonExistentResultId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTestResult_OnlyUpdatesResultAndComments()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        var createRequest = new CreateTestResultRequest(null, TestResultStatus.Passed, "Original", null);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/runs/{runId}/results", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        var resultId = created!.Id;
        var originalExecutedBy = created.ExecutedBy;
        var originalExecutedAt = created.ExecutedAt;

        var updateRequest = new UpdateTestResultRequest(TestResultStatus.Failed, "Updated", null);

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/runs/{runId}/results/{resultId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync($"/api/v1/runs/{runId}/results/{resultId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        updated!.ExecutedBy.Should().Be(originalExecutedBy);
        updated.ExecutedAt.Should().Be(originalExecutedAt);
    }

    [Fact]
    public async Task DeleteTestResult_WithExistingId_Returns204()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);

        var createRequest = new CreateTestResultRequest(null, TestResultStatus.Passed, "To Delete", null);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/runs/{runId}/results", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestResultResponse>(HttpClientExtensions.JsonOptions);
        var resultId = created!.Id;

        // Act
        var response = await client.DeleteAsync($"/api/v1/runs/{runId}/results/{resultId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/runs/{runId}/results/{resultId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestResult_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var runId = await CreateTestRunAsync(client, projectId);
        var nonExistentResultId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/v1/runs/{runId}/results/{nonExistentResultId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
