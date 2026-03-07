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
public class TestCasesControllerTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory = factory;

    private async Task<Guid> CreateProjectAsync(HttpClient client, string projectName = "")
    {
        var name = string.IsNullOrEmpty(projectName) ? $"Project-{Guid.NewGuid()}" : projectName;
        var request = new CreateProjectRequest(name, "Description");
        var response = await client.PostAsJsonAsync("/api/v1/projects", request);
        var created = await response.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    private async Task<Guid> CreateTestSuiteAsync(HttpClient client, Guid projectId, string suiteName = "")
    {
        var name = string.IsNullOrEmpty(suiteName) ? $"Suite-{Guid.NewGuid()}" : suiteName;
        var request = new CreateTestSuiteRequest(name, "Description");
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/suites", request);
        var created = await response.Content.ReadFromJsonAsync<TestSuiteResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    [Fact]
    public async Task GetTestCases_WithValidSuiteId_Returns200WithPaginatedList()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);

        // Act
        var response = await client.GetAsync($"/api/v1/suites/{suiteId}/cases");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<TestCaseResponse>>(HttpClientExtensions.JsonOptions);
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty();
        content.Page.Should().Be(1);
        content.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GetTestCases_WithPriorityFilter_Returns200WithFiltered()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);

        var createRequest = new CreateTestCaseRequest($"Case-{Guid.NewGuid()}", "Description", null, null);
        await client.PostAsJsonAsync($"/api/v1/suites/{suiteId}/cases", createRequest);

        // Act
        var response = await client.GetAsync($"/api/v1/suites/{suiteId}/cases?priority={TestCasePriority.High}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<TestCaseResponse>>(HttpClientExtensions.JsonOptions);
        content!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTestCase_WithExistingId_Returns200WithDetail()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);

        var caseName = $"Case-{Guid.NewGuid()}";
        var createRequest = new CreateTestCaseRequest(caseName, "Description", "Preconditions", "Expected Result");
        var createResponse = await client.PostAsJsonAsync($"/api/v1/suites/{suiteId}/cases", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestCaseResponse>(HttpClientExtensions.JsonOptions);
        var caseId = created!.Id;

        // Act
        var response = await client.GetAsync($"/api/v1/suites/{suiteId}/cases/{caseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var testCase = await response.Content.ReadFromJsonAsync<TestCaseDetailResponse>(HttpClientExtensions.JsonOptions);
        testCase.Should().NotBeNull();
        testCase!.Id.Should().Be(caseId);
        testCase.Name.Should().Be(caseName);
        testCase.Steps.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTestCase_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var nonExistentCaseId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/suites/{suiteId}/cases/{nonExistentCaseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTestCase_WithValidRequest_Returns201WithLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);

        var request = new CreateTestCaseRequest("New Test Case", "Description", "Preconditions", "Expected Result");

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/suites/{suiteId}/cases", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var testCase = await response.Content.ReadFromJsonAsync<TestCaseResponse>(HttpClientExtensions.JsonOptions);
        testCase.Should().NotBeNull();
        testCase!.Name.Should().Be("New Test Case");
    }

    [Fact]
    public async Task CreateTestCase_WithInvalidRequest_Returns400()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);

        var request = new CreateTestCaseRequest("", "Description", null, null);

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/suites/{suiteId}/cases", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTestCase_WithValidRequest_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);

        var createRequest = new CreateTestCaseRequest("Original", "Description", null, null);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/suites/{suiteId}/cases", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestCaseResponse>(HttpClientExtensions.JsonOptions);
        var caseId = created!.Id;

        var updateRequest = new UpdateTestCaseRequest("Updated", "New Description", "Preconditions", "Expected Result", TestCasePriority.High, TestCaseStatus.Active);

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/suites/{suiteId}/cases/{caseId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync($"/api/v1/suites/{suiteId}/cases/{caseId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<TestCaseDetailResponse>(HttpClientExtensions.JsonOptions);
        updated!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateTestCase_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var nonExistentCaseId = Guid.NewGuid();

        var updateRequest = new UpdateTestCaseRequest("Updated", "Description", null, null, TestCasePriority.Medium, TestCaseStatus.Draft);

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/suites/{suiteId}/cases/{nonExistentCaseId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestCase_WithExistingId_Returns204()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);

        var createRequest = new CreateTestCaseRequest("To Delete", "Description", null, null);
        var createResponse = await client.PostAsJsonAsync($"/api/v1/suites/{suiteId}/cases", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestCaseResponse>(HttpClientExtensions.JsonOptions);
        var caseId = created!.Id;

        // Act
        var response = await client.DeleteAsync($"/api/v1/suites/{suiteId}/cases/{caseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/suites/{suiteId}/cases/{caseId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestCase_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var nonExistentCaseId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/v1/suites/{suiteId}/cases/{nonExistentCaseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
