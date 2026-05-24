using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a scheduled meeting transitions to InProgress.
/// </summary>
public sealed record MeetingStartedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    MeetingId MeetingId,
    IReadOnlyList<AgentId> ParticipantIds
) : IDomainEvent
{
    public static MeetingStartedEvent Create(MeetingId meetingId, IReadOnlyList<AgentId> participants) =>
        new(Guid.NewGuid(), DateTime.UtcNow, meetingId, participants);
}
