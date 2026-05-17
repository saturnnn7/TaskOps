namespace TaskOps.Application.DTOs.Tasks;

public sealed class CreateTaskDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Priority { get; init; }
    public Guid? AssigneeId { get; init; }
    public DateTime? DueDate { get; init; }
}