using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Events;
using AICompanyOS.Application.Common.Outbox;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Application.Services;
using MediatR;

namespace AICompanyOS.Application.Features.Decisions.FinalizeDecision;

public sealed class FinalizeDecisionHandler : IRequestHandler<FinalizeDecisionCommand, Result>
{
    private readonly DecisionApplicationService _service;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IOutboxWriter _outboxWriter;
    private readonly IUnitOfWork _unitOfWork;

    public FinalizeDecisionHandler(
        DecisionApplicationService service,
        IDomainEventDispatcher dispatcher,
        IOutboxWriter outboxWriter,
        IUnitOfWork unitOfWork)
    {
        _service      = service;
        _dispatcher   = dispatcher;
        _outboxWriter = outboxWriter;
        _unitOfWork   = unitOfWork;
    }

    public async Task<Result> Handle(FinalizeDecisionCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var (serviceResult, decision) = await _service.FinalizeAsync(
                request.DecisionId,
                request.FinalizingAgentId,
                request.FinalizingAgentRole,
                request.Verdict,
                request.Reasoning,
                cancellationToken);

            if (!serviceResult.IsSuccess)
                return serviceResult;

            if (decision is null)
                return Result.Fail("Decision finalization produced no aggregate instance.");

            var occurredOnUtc = DateTime.UtcNow;
            var outboxMessages = DomainEventOutboxMapper.MapToOutboxMessages(decision.DomainEvents, occurredOnUtc);
            if (outboxMessages.Count > 0)
                await _outboxWriter.AddAsync(outboxMessages, cancellationToken);

            await _dispatcher.DispatchAsync(decision.DomainEvents, cancellationToken);
            decision.ClearDomainEvents();

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
