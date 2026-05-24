using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;


namespace AICompanyOS.Application.Features.Decisions.FinalizeDecision;


public sealed class FinalizeDecisionHandler : IRequestHandler<FinalizeDecisionCommand, Result>
{
    private readonly IDecisionRepository _decisionRepository;
    private readonly IAgentRepository _agentRepository;

    public FinalizeDecisionHandler(
        IDecisionRepository decisionRepository,
        IAgentRepository agentRepository)
    {
        _decisionRepository = decisionRepository;
        _agentRepository = agentRepository;
    }

    public async Task<Result> Handle(FinalizeDecisionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var decision = await _decisionRepository.GetByIdAsync(new DecisionId(request.DecisionId), cancellationToken);
            if (decision is null)
                return Result.Fail($"Decision not found: {request.DecisionId}");

            var finalizingAgent = await _agentRepository.GetByIdAsync(new AgentId(request.FinalizingAgentId), cancellationToken);
            if (finalizingAgent is null)
                return Result.Fail($"Finalizing agent not found: {request.FinalizingAgentId}");

            // Domain enforces authorization + immutability.
            var outcome = new DecisionOutcome(request.Verdict, request.Reasoning);

            decision.Finalize(
                outcome,
                finalizingAgent.Id,
                (AgentRole)request.FinalizingAgentRole);


            _decisionRepository.Update(decision);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

