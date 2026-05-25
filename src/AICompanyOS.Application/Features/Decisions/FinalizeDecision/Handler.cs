using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Application.Services;
using MediatR;

namespace AICompanyOS.Application.Features.Decisions.FinalizeDecision;

public sealed class FinalizeDecisionHandler : IRequestHandler<FinalizeDecisionCommand, Result>
{
    private readonly DecisionApplicationService _service;
    private readonly IUnitOfWork _unitOfWork;

    public FinalizeDecisionHandler(DecisionApplicationService service, IUnitOfWork unitOfWork)
    {
        _service = service;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(FinalizeDecisionCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            return await _service.FinalizeAsync(
                request.DecisionId,
                request.FinalizingAgentId,
                request.FinalizingAgentRole,
                request.Verdict,
                request.Reasoning,
                cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}


