using FluentValidation;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Validators.Tasks;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}
