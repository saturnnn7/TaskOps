namespace TaskOps.Application.DTOs.Projects;

public sealed class AddMemberDto
{
    public Guid UserId { get; init; }
    public string Role { get; init; } = "Member";
}