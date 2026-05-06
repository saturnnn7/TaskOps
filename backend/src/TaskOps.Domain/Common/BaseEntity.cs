namespace TaskOps.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides identity and audit timestamps.
/// Every entity in the system must inherit from this class.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}