namespace TaskOps.Application.DTOs.Projects;

public sealed class CreateProjectDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}