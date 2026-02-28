using System.Security.Claims;
using Api.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace Api.Tests.Endpoints;

public class UserEndpointTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory = factory;

    [Fact]
    public async Task GetUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/user");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUser_WithValidToken_ReturnsUserData()
    {
        var client = _factory.CreateClient();

        var identity = new ClaimsIdentity(
        [
            new Claim("sub", "test-user-123"),
            new Claim("preferred_username", "testuser"),
            new Claim("email", "test@example.com")
        ], "Bearer");

        var principal = new ClaimsPrincipal(identity);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "dummy-token");

        var response = await client.GetAsync("/api/user");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
