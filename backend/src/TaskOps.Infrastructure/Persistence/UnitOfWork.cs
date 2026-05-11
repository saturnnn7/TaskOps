using TaskOps.Domain.Interfaces;
using TaskOps.Infrastructure.Repositories;

namespace TaskOps.Infrastructure.Persistence;

/// <summary>
/// Coordinates all repositories under one DbContext instance.
/// Ensures all operations in a single request share the same transaction.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public IProjectRepository Projects { get; }
    public ITaskRepository Tasks { get; }
    public ICommentRepository Comments { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Projects = new ProjectRepository(context);
        Tasks = new TaskRepository(context);
        Comments = new CommentRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
    
    public void Dispose()
        => _context.Dispose();
}