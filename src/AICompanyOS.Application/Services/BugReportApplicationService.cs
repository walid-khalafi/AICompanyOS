using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.Enums;

using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Application.Services;

public sealed class BugReportApplicationService
{
    private readonly IBugReportRepository _bugReportRepository;
    private readonly IAgentRepository _agentRepository;

    public BugReportApplicationService(IBugReportRepository bugReportRepository, IAgentRepository agentRepository)
    {
        _bugReportRepository = bugReportRepository;
        _agentRepository = agentRepository;
    }

    public async Task<(Result Result, AICompanyOS.Domain.Entities.BugReport? Report)> FileAsync(
        string title,
        string description,
        Domain.Enums.Priority severity,
        Guid reportingAgentId,
        Domain.Enums.AgentRole reportingAgentRole,

        CancellationToken cancellationToken)

    {
        var reportingAgent = await _agentRepository.GetByIdAsync(new AgentId(reportingAgentId), cancellationToken);
        if (reportingAgent is null)
            return (Result.Fail($"Reporting agent not found: {reportingAgentId}"), null);

        var report = BugReport.File(title, description, severity, reportingAgent.Id, reportingAgentRole);

        await _bugReportRepository.AddAsync(report, cancellationToken);
        _bugReportRepository.Update(report);

        return (Result.Ok(), report);

    }

    public async Task<(Result Result, AICompanyOS.Domain.Entities.BugReport? Report)> ResolveAsync(
        Guid bugReportId,
        Guid resolvedByAgentId,
        string resolutionNotes,
        CancellationToken cancellationToken)
    {
        var report = await _bugReportRepository.GetByIdAsync(new BugReportId(bugReportId), cancellationToken);
        if (report is null)
            return (Result.Fail($"Bug report not found: {bugReportId}"), null);

        var resolverAgent = await _agentRepository.GetByIdAsync(new AgentId(resolvedByAgentId), cancellationToken);
        if (resolverAgent is null)
            return (Result.Fail($"Resolver agent not found: {resolvedByAgentId}"), null);

        report.Resolve(resolverAgent.Id, resolutionNotes);
        _bugReportRepository.Update(report);

        return (Result.Ok(), report);
    }

}

