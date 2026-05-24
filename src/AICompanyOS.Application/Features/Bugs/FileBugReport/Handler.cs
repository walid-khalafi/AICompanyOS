using AICompanyOS.Application.Common.Events;
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
    private readonly IDomainEventDispatcher _dispatcher;

    public FileBugReportHandler(
        IBugReportRepository bugReportRepository,
        IAgentRepository agentRepository,
        IDomainEventDispatcher dispatcher)
    {
        _bugReportRepository = bugReportRepository;
        _agentRepository = agentRepository;
        _dispatcher = dispatcher;
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

            await _dispatcher.DispatchAsync(report.DomainEvents, cancellationToken);
            report.ClearDomainEvents();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

