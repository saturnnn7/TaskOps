namespace TaskOps.Application.DTOs.Tasks;

public sealed class UpdateTaskDto
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Status { get; init; }
    public string? Priority { get; init; }
    public Guid? AssigneeId { get; init; }
    public DateTime? DueDate { get; init; }
}