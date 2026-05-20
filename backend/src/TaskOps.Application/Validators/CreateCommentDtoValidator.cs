using FluentValidation;
using TaskOps.Application.DTOs.Comments;

namespace TaskOps.Application.Validators;

public sealed class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MinimumLength(1).WithMessage("Comment must not be empty.")
            .MaximumLength(4000).WithMessage("Comment must not exceed 4000 characters.");
    }
}