using FluentValidation;

namespace AICompanyOS.Application.Features.Tasks.CreateTask;

public sealed class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CreatedByAgentId).NotEqual(Guid.Empty);
        RuleFor(x => x.Priority).InclusiveBetween(0, 10);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
    }
}

