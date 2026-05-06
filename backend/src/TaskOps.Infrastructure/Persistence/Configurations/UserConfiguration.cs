using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskOps.Domain.Entities;

namespace TaskOps.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id");
        
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(254)
            .IsRequired();
        
        builder.Property(u => u.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();
        
        builder.Property(u => u.IsEmailVerified)
            .HasColumnName("is_email_verified")
            .HasDefaultValue(false);
        
        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at");
        
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at");
        
        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at");
        
        // Unique constraint — one account per email
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email");
    }
}