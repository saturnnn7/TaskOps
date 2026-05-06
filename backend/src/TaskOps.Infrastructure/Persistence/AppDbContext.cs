using Microsoft.EntityFrameworkCore;
using TaskOps.Domain.Entities;
using TaskOps.Infrastructure.Persistence.Interceptors;

namespace TaskOps.Infrastructure.Persistence;

/// <summary>
/// Main database context for the application.
/// Configurations are loaded automatically via IEntityTypeConfiguration pattern.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically applies all IEntityTypeConfiguration classes
        // in this assembly — no need to register each one manually
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Register AuditInterceptor to auto-set CreatedAt/UpdatedAt
        optionsBuilder.AddInterceptors(new AuditInterceptor());
    }
}