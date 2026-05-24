using AICompanyOS.Domain.Enums;
using FluentValidation;

namespace AICompanyOS.Application.Features.Bugs.FileBugReport;

public sealed class FileBugReportValidator : AbstractValidator<FileBugReportCommand>
{
    public FileBugReportValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.ReportingAgentId).NotEqual(Guid.Empty);
        RuleFor(x => x.ReportingAgentRole).IsInEnum();
    }
}

