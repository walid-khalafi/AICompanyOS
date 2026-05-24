using FluentValidation;

namespace AICompanyOS.Application.Features.Bugs.ResolveBugReport;

public sealed class ResolveBugReportValidator : AbstractValidator<ResolveBugReportCommand>
{
    public ResolveBugReportValidator()
    {
        RuleFor(x => x.BugReportId).NotEqual(Guid.Empty);
        RuleFor(x => x.ResolvedByAgentId).NotEqual(Guid.Empty);
        RuleFor(x => x.ResolutionNotes).NotEmpty();
    }
}

