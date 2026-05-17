using FluentValidation;
using TaskOps.Application.DTOs.Projects;

namespace TaskOps.Application.Validators;

public sealed class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(2).WithMessage("Project name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Project name must not exceed 100 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => x.Description is not null);
    }
}