using FluentValidation;

namespace AICompanyOS.Application.Features.Tasks.AssignTask;

public sealed class AssignTaskValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskValidator()
    {
        RuleFor(x => x.TaskId).NotEqual(Guid.Empty);
        RuleFor(x => x.AgentId).NotEqual(Guid.Empty);
    }
}

