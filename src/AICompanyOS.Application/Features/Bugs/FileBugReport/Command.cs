using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Enums;
using MediatR;

namespace AICompanyOS.Application.Features.Bugs.FileBugReport;

public sealed record FileBugReportCommand(
    string Title,
    string Description,
    Priority Severity,
    Guid ReportingAgentId,
    AgentRole ReportingAgentRole) : IRequest<Result>;

