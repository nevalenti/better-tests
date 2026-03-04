using System.Net;
using Api.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace Api.Tests
{
    [Collection("Sequential")]
    public class HealthCheckTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly ApiWebApplicationFactory _factory = factory;

        [Fact]
        public async Task HealthCheck_Should_Return_Healthy()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/healthz");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Healthy");
        }
    }
}
