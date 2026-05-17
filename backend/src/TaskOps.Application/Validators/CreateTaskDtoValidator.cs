using FluentValidation;
using TaskOps.Application.DTOs.Tasks;
using TaskOps.Domain.Enums;

namespace TaskOps.Application.Validators;

public sealed class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MinimumLength(2).WithMessage("Task title must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Description must not exceed 4000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Priority)
            .Must(p => Enum.TryParse<TaskPriority>(p, ignoreCase: true, out _))
            .WithMessage($"Priority must be one of: {string.Join(", ", Enum.GetNames<TaskPriority>())}.")
            .When(x => x.Priority is not null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.DueDate is not null);
    }
}