using BetterTests.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTests.Infrastructure.Persistence.Configurations;

public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
{
    public void Configure(EntityTypeBuilder<TestResult> entity)
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
    }
}
