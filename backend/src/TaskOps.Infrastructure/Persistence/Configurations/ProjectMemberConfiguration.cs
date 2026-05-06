using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskOps.Domain.Entities;
using TaskOps.Domain.Enums;

namespace TaskOps.Infrastructure.Persistence.Configurations;

public sealed class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("project_members");

        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.Id)
            .HasColumnName("id");

        builder.Property(pm => pm.ProjectId)
            .HasColumnName("project_id");

        builder.Property(pm => pm.UserId)
            .HasColumnName("user_id");

        builder.Property(pm => pm.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(ProjectRole.Member)
            .HasSentinel(ProjectRole.Owner);

        builder.Property(pm => pm.JoinedAt)
            .HasColumnName("joined_at");

        builder.Property(pm => pm.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(pm => pm.UpdatedAt)
            .HasColumnName("updated_at");

        // One user can only appear once per project
        builder.HasIndex(pm => new { pm.ProjectId, pm.UserId })
            .IsUnique()
            .HasDatabaseName("ix_project_members_project_user");

        builder.HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}