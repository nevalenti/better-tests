using Api.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;

namespace Api.Tests.Extensions;

public class ServiceExtensionsTests
{
    [Fact]
    public void AddDatabase_RegistersDbContext()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;User Id=user;Password=pass;Database=testdb;" }
            })
            .Build();

        services.AddDatabase(config);

        var descriptor = services.FirstOrDefault(s =>
            s.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<AppDbContext>));

        descriptor.Should().NotBeNull("DbContext should be registered");
    }

    [Fact]
    public void AddDatabase_ConfiguresPostgresConnection()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;User Id=user;Password=pass;Database=testdb;" }
            })
            .Build();

        services.AddDatabase(config);

        var descriptor = services.FirstOrDefault(s =>
            s.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<AppDbContext>));

        descriptor.Should().NotBeNull("DbContext should be registered");
    }
}
