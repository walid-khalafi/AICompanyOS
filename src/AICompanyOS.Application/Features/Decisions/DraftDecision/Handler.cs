using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Decisions.DraftDecision;

public sealed class DraftDecisionHandler : IRequestHandler<DraftDecisionCommand, Result>
{
    private readonly IDecisionRepository _decisionRepository;
    private readonly IMeetingRepository _meetingRepository;
    private readonly IAgentRepository _agentRepository;

    public DraftDecisionHandler(
        IDecisionRepository decisionRepository,
        IMeetingRepository meetingRepository,
        IAgentRepository agentRepository)
    {
        _decisionRepository = decisionRepository;
        _meetingRepository = meetingRepository;
        _agentRepository = agentRepository;
    }

    public async Task<Result> Handle(DraftDecisionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var draftingAgent = await _agentRepository
                .GetByIdAsync(new AgentId(request.DraftingAgentId), cancellationToken);

            if (draftingAgent is null)
                return Result.Fail($"Drafting agent not found: {request.DraftingAgentId}");

            MeetingId? relatedMeetingId = null;
            if (request.RelatedMeetingId is not null)
            {
                var meeting = await _meetingRepository
                    .GetByIdAsync(new MeetingId(request.RelatedMeetingId.Value), cancellationToken);

                if (meeting is null)
                    return Result.Fail($"Related meeting not found: {request.RelatedMeetingId.Value}");

                relatedMeetingId = meeting.Id;
            }

            // Business rules for role authorization belong to Domain.
            var decision = Decision.Draft(
                request.Subject,
                draftingAgent.Id,
                request.DraftingAgentRole,
                relatedMeetingId);

            await _decisionRepository.AddAsync(decision, cancellationToken);
            _decisionRepository.Update(decision);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

