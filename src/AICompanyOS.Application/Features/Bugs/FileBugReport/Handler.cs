using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Bugs.FileBugReport;

public sealed class FileBugReportHandler : IRequestHandler<FileBugReportCommand, Result>
{
    private readonly IBugReportRepository _bugReportRepository;
    private readonly IAgentRepository _agentRepository;

    public FileBugReportHandler(
        IBugReportRepository bugReportRepository,
        IAgentRepository agentRepository)
    {
        _bugReportRepository = bugReportRepository;
        _agentRepository = agentRepository;
    }

    public async Task<Result> Handle(FileBugReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var reportingAgent = await _agentRepository
                .GetByIdAsync(new AgentId(request.ReportingAgentId), cancellationToken);

            if (reportingAgent is null)
                return Result.Fail($"Reporting agent not found: {request.ReportingAgentId}");

            var report = BugReport.File(
                request.Title,
                request.Description,
                request.Severity,
                reportingAgent.Id,
                request.ReportingAgentRole);

            await _bugReportRepository.AddAsync(report, cancellationToken);
            _bugReportRepository.Update(report);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

