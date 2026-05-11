using TaskOps.Domain.Entities;

namespace TaskOps.Domain.Interfaces;

/// <summary>
/// User-specific repository operations beyond generic CRUD.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Finds a user by their email address.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Checks if an email address is already registered.</summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}