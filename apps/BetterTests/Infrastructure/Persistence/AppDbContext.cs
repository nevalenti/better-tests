using BetterTests.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace BetterTests.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<TestSuite> TestSuites { get; set; }
    public DbSet<TestCase> TestCases { get; set; }
    public DbSet<TestCaseStep> TestCaseSteps { get; set; }
    public DbSet<TestRun> TestRuns { get; set; }
    public DbSet<TestResult> TestResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new Configurations.ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.TestSuiteConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.TestCaseConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.TestCaseStepConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.TestRunConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.TestResultConfiguration());
    }
}
