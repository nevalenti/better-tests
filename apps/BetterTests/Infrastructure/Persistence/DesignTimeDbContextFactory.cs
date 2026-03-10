using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BetterTests.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Database=design_time_db";

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseNpgsql(connectionString);
        return new AppDbContext(builder.Options);
    }
}
