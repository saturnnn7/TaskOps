namespace TaskOps.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern — wraps all repositories under a single transaction.
/// Call SaveChangesAsync once after all operations to commit atomically.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProjectRepository Projects { get; }
    ITaskRepository Tasks { get; }
    ICommentRepository Comments { get; }

    /// <summary>
    /// Commits all pending changes to the database in a single transaction.
    /// Returns the number of affected rows.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}