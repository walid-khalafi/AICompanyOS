using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Application.Services;

public sealed class DecisionApplicationService
{
    private readonly IDecisionRepository _decisionRepository;
    private readonly IMeetingRepository _meetingRepository;
    private readonly IAgentRepository _agentRepository;

    public DecisionApplicationService(
        IDecisionRepository decisionRepository,
        IMeetingRepository meetingRepository,
        IAgentRepository agentRepository)
    {
        _decisionRepository = decisionRepository;
        _meetingRepository = meetingRepository;
        _agentRepository = agentRepository;
    }

    public async Task<(Result Result, Decision? Decision)> DraftAsync(
        string subject,
        Guid draftingAgentId,
        AgentRole draftingAgentRole,
        Guid? relatedMeetingId,
        CancellationToken cancellationToken)
    {
        var draftingAgent = await _agentRepository.GetByIdAsync(new AgentId(draftingAgentId), cancellationToken);
        if (draftingAgent is null)
            return (Result.Fail($"Drafting agent not found: {draftingAgentId}"), null);

        MeetingId? meetingId = null;
        if (relatedMeetingId is not null)
        {
            var meeting = await _meetingRepository.GetByIdAsync(new MeetingId(relatedMeetingId.Value), cancellationToken);
            if (meeting is null)
                return (Result.Fail($"Related meeting not found: {relatedMeetingId.Value}"), null);

            meetingId = meeting.Id;
        }

        var decision = Decision.Draft(subject, draftingAgent.Id, draftingAgentRole, meetingId);

        await _decisionRepository.AddAsync(decision, cancellationToken);
        _decisionRepository.Update(decision);

        return (Result.Ok(), decision);
    }

    public async Task<(Result Result, Decision? Decision)> FinalizeAsync(
        Guid decisionId,
        Guid finalizingAgentId,
        AgentRole finalizingAgentRole,
        string verdict,
        string reasoning,
        CancellationToken cancellationToken)
    {
        var decision = await _decisionRepository.GetByIdAsync(new DecisionId(decisionId), cancellationToken);
        if (decision is null)
            return (Result.Fail($"Decision not found: {decisionId}"), null);

        var finalizingAgent = await _agentRepository.GetByIdAsync(new AgentId(finalizingAgentId), cancellationToken);
        if (finalizingAgent is null)
            return (Result.Fail($"Finalizing agent not found: {finalizingAgentId}"), null);

        var outcome = new DecisionOutcome(verdict, reasoning);
        decision.Finalize(outcome, finalizingAgent.Id, finalizingAgentRole);
        _decisionRepository.Update(decision);

        return (Result.Ok(), decision);
    }
}
