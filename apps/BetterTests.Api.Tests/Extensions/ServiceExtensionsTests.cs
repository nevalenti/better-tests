using BetterTests.Api.Presentation.Extensions;
using BetterTests.Infrastructure.Persistence;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace BetterTests.Api.Tests.Extensions;

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
    public void AddDatabase_RegistersAppDbContextAndAllowsResolution()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;User Id=user;Password=pass;Database=testdb;" }
            })
            .Build();

        services.AddDatabase(config);

        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<AppDbContext>();

        dbContext.Should().NotBeNull("AppDbContext should be resolvable from the service provider");
    }
}
