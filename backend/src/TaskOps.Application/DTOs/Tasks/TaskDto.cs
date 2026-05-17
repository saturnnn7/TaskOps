namespace TaskOps.Application.DTOs.Tasks;

public sealed class TaskDto
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public Guid? AssigneeId { get; init; }
    public string? AssigneeName { get; init; }
    public Guid CreatedById { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public DateTime? DueDate { get; init; }
    public int Position { get; init; }
    public int CommentCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}