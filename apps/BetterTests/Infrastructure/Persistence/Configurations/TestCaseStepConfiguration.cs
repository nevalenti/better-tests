using BetterTests.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTests.Infrastructure.Persistence.Configurations;

public class TestCaseStepConfiguration : IEntityTypeConfiguration<TestCaseStep>
{
    public void Configure(EntityTypeBuilder<TestCaseStep> entity)
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
    }
}
