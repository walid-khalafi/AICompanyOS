using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Application.Services;
using MediatR;

namespace AICompanyOS.Application.Features.Decisions.DraftDecision;

public sealed class DraftDecisionHandler : IRequestHandler<DraftDecisionCommand, Result>
{
    private readonly DecisionApplicationService _service;
    private readonly IUnitOfWork _unitOfWork;

    public DraftDecisionHandler(DecisionApplicationService service, IUnitOfWork unitOfWork)
    {
        _service = service;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DraftDecisionCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            return await _service.DraftAsync(
                request.Subject,
                request.DraftingAgentId,
                request.DraftingAgentRole,
                request.RelatedMeetingId,
                cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}



