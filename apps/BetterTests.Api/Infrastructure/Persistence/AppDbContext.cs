using BetterTests.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace BetterTests.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<TestSuite> TestSuites { get; set; }
    public DbSet<TestCase> TestCases { get; set; }
    public DbSet<TestCaseStep> TestCaseSteps { get; set; }
    public DbSet<TestRun> TestRuns { get; set; }
    public DbSet<TestResult> TestResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProject(modelBuilder);
        ConfigureTestSuite(modelBuilder);
        ConfigureTestCase(modelBuilder);
        ConfigureTestCaseStep(modelBuilder);
        ConfigureTestRun(modelBuilder);
        ConfigureTestResult(modelBuilder);
    }

    private static void ConfigureProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("projects");

            // Primary key
            entity.HasKey(e => e.Id)
                .HasName("pk_projects");

            // Properties
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            // Relationships
            entity.HasMany(e => e.TestSuites)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_suites_projects");

            entity.HasMany(e => e.TestRuns)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_runs_projects");

            // Indexes
            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("ix_projects_name_unique");
        });
    }

    private static void ConfigureTestSuite(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestSuite>(entity =>
        {
            entity.ToTable("test_suites");

            // Primary key
            entity.HasKey(e => e.Id)
                .HasName("pk_test_suites");

            // Properties
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.ProjectId)
                .HasColumnName("project_id")
                .IsRequired();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            // Relationships
            entity.HasOne(e => e.Project)
                .WithMany(p => p.TestSuites)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_suites_projects");

            entity.HasMany(e => e.TestCases)
                .WithOne(e => e.Suite)
                .HasForeignKey(e => e.SuiteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_cases_test_suites");

            // Indexes
            entity.HasIndex(e => new { e.ProjectId, e.Name })
                .IsUnique()
                .HasDatabaseName("ix_test_suites_project_id_name_unique");
        });
    }

    private static void ConfigureTestCase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestCase>(entity =>
        {
            entity.ToTable("test_cases");

            // Primary key
            entity.HasKey(e => e.Id)
                .HasName("pk_test_cases");

            // Properties
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.SuiteId)
                .HasColumnName("suite_id")
                .IsRequired();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(2000);

            entity.Property(e => e.Preconditions)
                .HasColumnName("preconditions")
                .IsRequired(false)
                .HasMaxLength(1000);

            entity.Property(e => e.Postconditions)
                .HasColumnName("postconditions")
                .IsRequired(false)
                .HasMaxLength(1000);

            entity.Property(e => e.Priority)
                .HasColumnName("priority")
                .HasConversion<int>();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<int>();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            // Relationships
            entity.HasOne(e => e.Suite)
                .WithMany(s => s.TestCases)
                .HasForeignKey(e => e.SuiteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_cases_test_suites");

            entity.HasMany(e => e.Steps)
                .WithOne(e => e.TestCase)
                .HasForeignKey(e => e.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_case_steps_test_cases");

            entity.HasMany(e => e.TestResults)
                .WithOne(e => e.TestCase)
                .HasForeignKey(e => e.TestCaseId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_test_results_test_cases");

            // Indexes
            entity.HasIndex(e => new { e.SuiteId, e.Name })
                .IsUnique()
                .HasDatabaseName("ix_test_cases_suite_id_name_unique");

            entity.HasIndex(e => e.Priority)
                .HasDatabaseName("ix_test_cases_priority");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("ix_test_cases_status");
        });
    }

    private static void ConfigureTestCaseStep(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestCaseStep>(entity =>
        {
            entity.ToTable("test_case_steps");

            // Primary key
            entity.HasKey(e => e.Id)
                .HasName("pk_test_case_steps");

            // Properties
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.TestCaseId)
                .HasColumnName("test_case_id")
                .IsRequired();

            entity.Property(e => e.StepOrder)
                .HasColumnName("step_order")
                .IsRequired();

            entity.Property(e => e.Action)
                .HasColumnName("action")
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.ExpectedResult)
                .HasColumnName("expected_result")
                .IsRequired()
                .HasMaxLength(1000);

            // Relationships
            entity.HasOne(e => e.TestCase)
                .WithMany(tc => tc.Steps)
                .HasForeignKey(e => e.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_case_steps_test_cases");

            // Indexes
            entity.HasIndex(e => new { e.TestCaseId, e.StepOrder })
                .IsUnique()
                .HasDatabaseName("ix_test_case_steps_case_id_order_unique");

            entity.HasIndex(e => e.TestCaseId)
                .HasDatabaseName("ix_test_case_steps_case_id");
        });
    }

    private static void ConfigureTestRun(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestRun>(entity =>
        {
            entity.ToTable("test_runs");

            // Primary key
            entity.HasKey(e => e.Id)
                .HasName("pk_test_runs");

            // Properties
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.ProjectId)
                .HasColumnName("project_id")
                .IsRequired();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Environment)
                .HasColumnName("environment")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ExecutedBy)
                .HasColumnName("executed_by")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.StartedAt)
                .HasColumnName("started_at")
                .IsRequired(false);

            entity.Property(e => e.CompletedAt)
                .HasColumnName("completed_at")
                .IsRequired(false);

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<int>();

            // Relationships
            entity.HasOne(e => e.Project)
                .WithMany(p => p.TestRuns)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_runs_projects");

            entity.HasMany(e => e.TestResults)
                .WithOne(e => e.TestRun)
                .HasForeignKey(e => e.TestRunId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_results_test_runs");

            // Indexes
            entity.HasIndex(e => e.ProjectId)
                .HasDatabaseName("ix_test_runs_project_id");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("ix_test_runs_status");

            entity.HasIndex(e => e.StartedAt)
                .HasDatabaseName("ix_test_runs_started_at");
        });
    }

    private static void ConfigureTestResult(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.ToTable("test_results");

            // Primary key
            entity.HasKey(e => e.Id)
                .HasName("pk_test_results");

            // Properties
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.TestRunId)
                .HasColumnName("test_run_id")
                .IsRequired();

            entity.Property(e => e.TestCaseId)
                .HasColumnName("test_case_id")
                .IsRequired(false);

            entity.Property(e => e.Result)
                .HasColumnName("result")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.Comments)
                .HasColumnName("comments")
                .IsRequired(false)
                .HasMaxLength(2000);

            entity.Property(e => e.DefectLink)
                .HasColumnName("defect_link")
                .IsRequired(false)
                .HasMaxLength(255);

            entity.Property(e => e.ExecutedAt)
                .HasColumnName("executed_at");

            entity.Property(e => e.ExecutedBy)
                .HasColumnName("executed_by")
                .IsRequired()
                .HasMaxLength(255);

            // Relationships
            entity.HasOne(e => e.TestRun)
                .WithMany(tr => tr.TestResults)
                .HasForeignKey(e => e.TestRunId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_test_results_test_runs");

            entity.HasOne(e => e.TestCase)
                .WithMany(tc => tc.TestResults)
                .HasForeignKey(e => e.TestCaseId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_test_results_test_cases");

            // Indexes
            entity.HasIndex(e => new { e.TestRunId, e.TestCaseId })
                .IsUnique()
                .HasFilter("\"TestCaseId\" IS NOT NULL")
                .HasDatabaseName("ix_test_results_run_id_case_id_unique");

            entity.HasIndex(e => e.Result)
                .HasDatabaseName("ix_test_results_result");

            entity.HasIndex(e => e.TestRunId)
                .HasDatabaseName("ix_test_results_test_run_id");

            entity.HasIndex(e => e.TestCaseId)
                .HasDatabaseName("ix_test_results_test_case_id");
        });
    }
}
