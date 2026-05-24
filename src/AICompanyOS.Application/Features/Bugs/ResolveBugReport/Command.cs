using AICompanyOS.Application.Common.Result;
using MediatR;

namespace AICompanyOS.Application.Features.Bugs.ResolveBugReport;

public sealed record ResolveBugReportCommand(
    Guid BugReportId,
    Guid ResolvedByAgentId,
    string ResolutionNotes) : IRequest<Result>;

