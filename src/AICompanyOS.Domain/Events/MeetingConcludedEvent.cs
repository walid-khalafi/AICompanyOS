using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a meeting concludes successfully.
/// </summary>
public sealed record MeetingConcludedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    MeetingId MeetingId,
    string? Summary
) : IDomainEvent
{
    public static MeetingConcludedEvent Create(MeetingId meetingId, string? summary) =>
        new(Guid.NewGuid(), DateTime.UtcNow, meetingId, summary);
}
