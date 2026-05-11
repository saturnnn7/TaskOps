using System.Linq.Expressions;
using TaskOps.Domain.Common;

namespace TaskOps.Domain.Interfaces;

/// <summary>
/// Generic repository interface defining standard data access operations.
/// All entity-specific repositories must implement this interface.
/// </summary>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>Gets an entity by its unique identifier.</summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets all entities matching the given predicate.</summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>Gets a single entity matching the predicate, or null.</summary>
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether any entity matches the given predicate.</summary>
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>Returns total count of entities matching the predicate.</summary>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity to the context (not yet saved).</summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Marks an entity as modified (not yet saved).</summary>
    void Update(TEntity entity);

    /// <summary>Marks an entity for deletion (not yet saved).</summary>
    void Delete(TEntity entity);
}