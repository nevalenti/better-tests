using System.Net;
using System.Net.Http.Json;

using BetterTests.Application.DTOs;
using BetterTests.Tests.Fixtures;
using BetterTests.Tests.Helpers;

using FluentAssertions;

using Xunit;

namespace BetterTests.Tests.Controllers;

[Collection("Sequential")]
public class TestCaseStepsControllerTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory = factory;

    private async Task<Guid> CreateProjectAsync(HttpClient client)
    {
        var request = new CreateProjectRequest($"Project-{Guid.NewGuid()}", "Description");
        var response = await client.PostAsJsonAsync("/api/v1/projects", request);
        var created = await response.Content.ReadFromJsonAsync<ProjectResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    private async Task<Guid> CreateTestSuiteAsync(HttpClient client, Guid projectId)
    {
        var request = new CreateTestSuiteRequest($"Suite-{Guid.NewGuid()}", "Description");
        var response = await client.PostAsJsonAsync($"/api/v1/projects/{projectId}/suites", request);
        var created = await response.Content.ReadFromJsonAsync<TestSuiteResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    private async Task<Guid> CreateTestCaseAsync(HttpClient client, Guid suiteId)
    {
        var request = new CreateTestCaseRequest($"Case-{Guid.NewGuid()}", "Description", null, null);
        var response = await client.PostAsJsonAsync($"/api/v1/suites/{suiteId}/cases", request);
        var created = await response.Content.ReadFromJsonAsync<TestCaseResponse>(HttpClientExtensions.JsonOptions);
        return created!.Id;
    }

    [Fact]
    public async Task GetTestCaseSteps_WithValidCaseId_Returns200WithPaginatedList()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);

        // Act
        var response = await client.GetAsync($"/api/v1/cases/{caseId}/steps");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PaginatedResponse<TestCaseStepResponse>>(HttpClientExtensions.JsonOptions);
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTestCaseStep_WithExistingId_Returns200WithDetail()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);

        var createRequest = new CreateTestCaseStepRequest(1, "Click button", "Button is clicked");
        var createResponse = await client.PostAsJsonAsync($"/api/v1/cases/{caseId}/steps", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestCaseStepResponse>(HttpClientExtensions.JsonOptions);
        var stepId = created!.Id;

        // Act
        var response = await client.GetAsync($"/api/v1/cases/{caseId}/steps/{stepId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var step = await response.Content.ReadFromJsonAsync<TestCaseStepResponse>(HttpClientExtensions.JsonOptions);
        step.Should().NotBeNull();
        step!.Id.Should().Be(stepId);
        step.StepOrder.Should().Be(1);
        step.Action.Should().Be("Click button");
    }

    [Fact]
    public async Task GetTestCaseStep_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);
        var nonExistentStepId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/cases/{caseId}/steps/{nonExistentStepId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTestCaseStep_WithValidRequest_Returns201WithLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);

        var request = new CreateTestCaseStepRequest(1, "Enter username", "Username is entered");

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/cases/{caseId}/steps", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var step = await response.Content.ReadFromJsonAsync<TestCaseStepResponse>(HttpClientExtensions.JsonOptions);
        step.Should().NotBeNull();
        step!.StepOrder.Should().Be(1);
    }

    [Fact]
    public async Task CreateTestCaseStep_WithZeroStepOrder_Returns400()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);

        var request = new CreateTestCaseStepRequest(0, "Action", "Result");

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/cases/{caseId}/steps", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTestCaseStep_WithValidMultipleSteps_Returns201()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);

        // Act
        var request1 = new CreateTestCaseStepRequest(1, "Step 1", "Result 1");
        var response1 = await client.PostAsJsonAsync($"/api/v1/cases/{caseId}/steps", request1);
        response1.StatusCode.Should().Be(HttpStatusCode.Created);

        var request2 = new CreateTestCaseStepRequest(2, "Step 2", "Result 2");
        var response2 = await client.PostAsJsonAsync($"/api/v1/cases/{caseId}/steps", request2);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateTestCaseStep_WithValidRequest_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);

        var createRequest = new CreateTestCaseStepRequest(1, "Original Action", "Original Result");
        var createResponse = await client.PostAsJsonAsync($"/api/v1/cases/{caseId}/steps", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestCaseStepResponse>(HttpClientExtensions.JsonOptions);
        var stepId = created!.Id;

        var updateRequest = new UpdateTestCaseStepRequest(1, "Updated Action", "Updated Result");

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/cases/{caseId}/steps/{stepId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await client.GetAsync($"/api/v1/cases/{caseId}/steps/{stepId}");
        var updated = await getResponse.Content.ReadFromJsonAsync<TestCaseStepResponse>(HttpClientExtensions.JsonOptions);
        updated!.Action.Should().Be("Updated Action");
    }

    [Fact]
    public async Task UpdateTestCaseStep_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);
        var nonExistentStepId = Guid.NewGuid();

        var updateRequest = new UpdateTestCaseStepRequest(1, "Action", "Result");

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/cases/{caseId}/steps/{nonExistentStepId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestCaseStep_WithExistingId_Returns204()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);

        var createRequest = new CreateTestCaseStepRequest(1, "To Delete", "Result");
        var createResponse = await client.PostAsJsonAsync($"/api/v1/cases/{caseId}/steps", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TestCaseStepResponse>(HttpClientExtensions.JsonOptions);
        var stepId = created!.Id;

        // Act
        var response = await client.DeleteAsync($"/api/v1/cases/{caseId}/steps/{stepId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/cases/{caseId}/steps/{stepId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTestCaseStep_WithMissingId_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();
        var projectId = await CreateProjectAsync(client);
        var suiteId = await CreateTestSuiteAsync(client, projectId);
        var caseId = await CreateTestCaseAsync(client, suiteId);
        var nonExistentStepId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/v1/cases/{caseId}/steps/{nonExistentStepId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
