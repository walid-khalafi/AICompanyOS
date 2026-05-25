using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Application.Services;

public sealed class MeetingApplicationService
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IAgentRepository _agentRepository;

    public MeetingApplicationService(
        IMeetingRepository meetingRepository,
        IAgentRepository agentRepository)
    {
        _meetingRepository = meetingRepository;
        _agentRepository = agentRepository;
    }

    public async Task<Result> ScheduleAsync(
        string topic,
        Guid organizerAgentId,
        IReadOnlyCollection<Guid> participantAgentIds,
        CancellationToken cancellationToken)
    {
        var organizer = await _agentRepository.GetByIdAsync(new AgentId(organizerAgentId), cancellationToken);

        if (organizer is null)
            return Result.Fail($"Organizer agent not found: {organizerAgentId}");

        var participantIds = participantAgentIds.Select(id => new AgentId(id)).ToList();
        foreach (var pid in participantIds)
        {
            var agent = await _agentRepository.GetByIdAsync(pid, cancellationToken);
            if (agent is null)
                return Result.Fail($"Participant agent not found: {pid.Value}");
        }

        var validatedParticipantAgentIds = participantIds; // Ids validated above

        var meeting = Meeting.Schedule(topic, organizer.Id, validatedParticipantAgentIds);

        await _meetingRepository.AddAsync(meeting, cancellationToken);
        _meetingRepository.Update(meeting);

        return Result.Ok();
    }

    public async Task<Result> StartAsync(Guid meetingId, CancellationToken cancellationToken)
    {
        var meeting = await _meetingRepository.GetByIdAsync(new MeetingId(meetingId), cancellationToken);
        if (meeting is null)
            return Result.Fail($"Meeting not found: {meetingId}");

        meeting.Start();
        _meetingRepository.Update(meeting);

        return Result.Ok();
    }

    public async Task<Result> ConcludeAsync(Guid meetingId, string? summary, CancellationToken cancellationToken)
    {
        var meeting = await _meetingRepository.GetByIdAsync(new MeetingId(meetingId), cancellationToken);
        if (meeting is null)
            return Result.Fail($"Meeting not found: {meetingId}");

        meeting.Conclude(summary);

        _meetingRepository.Update(meeting);

        return Result.Ok();
    }
}


