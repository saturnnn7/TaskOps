using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskOps.Domain.Entities;

namespace TaskOps.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.TaskId)
            .HasColumnName("task_id");

        builder.Property(c => c.AuthorId)
            .HasColumnName("author_id");

        builder.Property(c => c.Content)
            .HasColumnName("content")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        // Index for fast comment loading per task
        builder.HasIndex(c => c.TaskId)
            .HasDatabaseName("ix_comments_task");

        builder.HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}