using BetterTests.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTests.Infrastructure.Persistence.Configurations;

public class TestRunConfiguration : IEntityTypeConfiguration<TestRun>
{
    public void Configure(EntityTypeBuilder<TestRun> entity)
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
    }
}
