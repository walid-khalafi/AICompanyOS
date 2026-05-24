using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.ScheduleMeeting;

public sealed class ScheduleMeetingHandler : IRequestHandler<ScheduleMeetingCommand, Result>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IAgentRepository _agentRepository;

    public ScheduleMeetingHandler(
        IMeetingRepository meetingRepository,
        IAgentRepository agentRepository)
    {
        _meetingRepository = meetingRepository;
        _agentRepository = agentRepository;
    }

    public async Task<Result> Handle(ScheduleMeetingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Existence validation only (no business rules).
            var organizer = await _agentRepository.GetByIdAsync(new AgentId(request.OrganizerAgentId), cancellationToken);
            if (organizer is null)
            {
                return Result.Fail($"Organizer agent not found: {request.OrganizerAgentId}");
            }

            var participantIds = request.ParticipantAgentIds;

            var participantAgentIds = new List<AgentId>(participantIds.Count);
            foreach (var pid in participantIds)
            {
                var agent = await _agentRepository.GetByIdAsync(new AgentId(pid), cancellationToken);
                if (agent is null)
                {
                    return Result.Fail($"Participant agent not found: {pid}");
                }

                participantAgentIds.Add(agent.Id);
            }

            // Business rules enforced in Domain Meeting.Schedule.
            var meeting = Meeting.Schedule(
                request.Topic,
                organizer.Id,
                participantAgentIds);

            await _meetingRepository.AddAsync(meeting, cancellationToken);

            // Domain events dispatching is expected via existing integration abstraction
            // (handled by pipeline/dispatcher in the Application layer).
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

