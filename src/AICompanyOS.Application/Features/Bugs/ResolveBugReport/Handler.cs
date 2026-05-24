using AICompanyOS.Application.Common.Events;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Bugs.ResolveBugReport;

public sealed class ResolveBugReportHandler : IRequestHandler<ResolveBugReportCommand, Result>
{
    private readonly IBugReportRepository _bugReportRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IDomainEventDispatcher _dispatcher;

    public ResolveBugReportHandler(
        IBugReportRepository bugReportRepository,
        IAgentRepository agentRepository,
        IDomainEventDispatcher dispatcher)
    {
        _bugReportRepository = bugReportRepository;
        _agentRepository = agentRepository;
        _dispatcher = dispatcher;
    }

    public async Task<Result> Handle(ResolveBugReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var report = await _bugReportRepository
                .GetByIdAsync(new BugReportId(request.BugReportId), cancellationToken);

            if (report is null)
                return Result.Fail($"Bug report not found: {request.BugReportId}");

            var resolverAgent = await _agentRepository
                .GetByIdAsync(new AgentId(request.ResolvedByAgentId), cancellationToken);

            if (resolverAgent is null)
                return Result.Fail($"Resolver agent not found: {request.ResolvedByAgentId}");

            // Business rules enforced in Domain.
            report.Resolve(resolverAgent.Id, request.ResolutionNotes);

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

