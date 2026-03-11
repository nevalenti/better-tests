using BetterTests.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterTests.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> entity)
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
    }
}
