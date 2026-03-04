using System.Net;
using System.Net.Http.Json;
using Api.Tests.Fixtures;
using Api.Tests.Helpers;
using BetterTests.Application.DTOs;
using FluentAssertions;
using Xunit;

namespace Api.Tests.Controllers;

[Collection("Sequential")]
public class TestSuitesControllerTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory = factory;

    private async Task<Guid> CreateProjectAsync(HttpClient client, string name = "")
    {
        var projectName = string.IsNullOrEmpty(name) ? $"Project-{Guid.NewGuid()}" : name;
        var request = new CreateProjectRequest(projectName, "Description");
        var response = await client.PostAsJsonAsync("/api/v1/projects", request);
        var created = await response.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    [Fact]
    public async Task GetTestSuites_WithValidProjectId_Returns200WithPaginatedList()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}/suites");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<TestSuiteResponse>>(HttpClientExtensions.JsonOptions);
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty();
        content.Page.Should().Be(1);
        content.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GetTestSuite_WithExistingId_Returns200WithDetail()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var suiteName = $"TestSuite-{Guid.NewGuid()}";
        var createRequest = new CreateTestSuiteRequest(suiteName, "Suite Description");
        var createResponse = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/suites", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestSuiteResponse>(HttpClientExtensions.JsonOptions);
        var suiteId = created!.Id;

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}/suites/{suiteId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var suite = await response.Content.ReadFromJsonAsync<TestSuiteDetailResponse>(HttpClientExtensions.JsonOptions);
        suite.Should().NotBeNull();
        suite!.Id.Should().Be(suiteId);
        suite.Name.Should().Be(suiteName);
        suite.TestCases.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTestSuite_WithWrongProjectId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var wrongProjectId = Guid.NewGuid();

        var createRequest = new CreateTestSuiteRequest($"TestSuite-{Guid.NewGuid()}", "Description");
        var createResponse = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/suites", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestSuiteResponse>(HttpClientExtensions.JsonOptions);
        var suiteId = created!.Id;

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{wrongProjectId}/suites/{suiteId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTestSuite_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var nonExistentSuiteId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/projects/{projectId}/suites/{nonExistentSuiteId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTestSuite_WithValidRequest_Returns201WithLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var suiteName = $"New Suite {Guid.NewGuid()}";
        var request = new CreateTestSuiteRequest(suiteName, "Suite Description");

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/suites", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var suite = await response.Content.ReadFromJsonAsync<TestSuiteResponse>(HttpClientExtensions.JsonOptions);
        suite.Should().NotBeNull();
        suite!.Name.Should().Be(suiteName);
        suite.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public async Task CreateTestSuite_WithInvalidRequest_Returns400()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var request = new CreateTestSuiteRequest("", "Description");

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/suites", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Contain("problem+json");
    }

    [Fact]
    public async Task CreateTestSuite_WithMissingProject_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistentProjectId = Guid.NewGuid();

        var request = new CreateTestSuiteRequest("New Suite", "Description");

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{nonExistentProjectId}/suites", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTestSuite_WithValidRequest_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var createRequest = new CreateTestSuiteRequest("Original Name", "Original Description");
        var createResponse = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/suites", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestSuiteResponse>(HttpClientExtensions.JsonOptions);
        var suiteId = created!.Id;

        var updateRequest = new UpdateTestSuiteRequest("Updated Name", "Updated Description");

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/projects/{projectId}/suites/{suiteId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync($"/api/v1/projects/{projectId}/suites/{suiteId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<TestSuiteDetailResponse>(HttpClientExtensions.JsonOptions);
        updated!.Name.Should().Be("Updated Name");
        updated.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task UpdateTestSuite_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var nonExistentSuiteId = Guid.NewGuid();

        var updateRequest = new UpdateTestSuiteRequest("Updated Name", "Description");

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/projects/{projectId}/suites/{nonExistentSuiteId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestSuite_WithExistingId_Returns204()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);

        var createRequest = new CreateTestSuiteRequest("To Delete", "Description");
        var createResponse = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/suites", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestSuiteResponse>(HttpClientExtensions.JsonOptions);
        var suiteId = created!.Id;

        // Act
        var response = await client.DeleteAsync($"/api/v1/projects/{projectId}/suites/{suiteId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/projects/{projectId}/suites/{suiteId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestSuite_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var nonExistentSuiteId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/v1/projects/{projectId}/suites/{nonExistentSuiteId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
