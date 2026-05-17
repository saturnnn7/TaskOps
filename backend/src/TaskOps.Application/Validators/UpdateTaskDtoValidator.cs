using FluentValidation;
using TaskOps.Application.DTOs.Tasks;
using TaskOps.Domain.Enums;

namespace TaskOps.Application.Validators;

public sealed class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(2).WithMessage("Task title must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters.")
            .When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Description must not exceed 4000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Status)
            .Must(s => Enum.TryParse<WorkTaskStatus>(s, ignoreCase: true, out _))
            .WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames<WorkTaskStatus>())}.")
            .When(x => x.Status is not null);

        RuleFor(x => x.Priority)
            .Must(p => Enum.TryParse<TaskPriority>(p, ignoreCase: true, out _))
            .WithMessage($"Priority must be one of: {string.Join(", ", Enum.GetNames<TaskPriority>())}.")
            .When(x => x.Priority is not null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.DueDate is not null);
    }
}