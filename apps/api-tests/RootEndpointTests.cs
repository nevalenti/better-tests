using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiTests;

public class RootEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Get_Root_ReturnsOk()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Get_Root_ReturnsHelloWorld()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");
        var body = await response.Content.ReadAsStringAsync();

        body.Should().Be("Hello, World!");
    }
}
