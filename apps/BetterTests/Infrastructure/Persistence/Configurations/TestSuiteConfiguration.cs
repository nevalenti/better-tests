using BetterTests.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTests.Infrastructure.Persistence.Configurations;

public class TestSuiteConfiguration : IEntityTypeConfiguration<TestSuite>
{
    public void Configure(EntityTypeBuilder<TestSuite> entity)
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
    }
}
