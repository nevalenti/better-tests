using BetterTests.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTests.Infrastructure.Persistence.Configurations;

public class TestCaseConfiguration : IEntityTypeConfiguration<TestCase>
{
    public void Configure(EntityTypeBuilder<TestCase> entity)
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
    }
}
