using FluentValidation;

namespace AICompanyOS.Application.Features.Tasks.CompleteTask;

public sealed class CompleteTaskValidator : AbstractValidator<CompleteTaskCommand>
{
    public CompleteTaskValidator()
    {
        RuleFor(x => x.TaskId).NotEqual(Guid.Empty);
        // No business-rule validation here.
    }
}

