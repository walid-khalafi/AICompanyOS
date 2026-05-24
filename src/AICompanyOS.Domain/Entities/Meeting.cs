using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Events;
using AICompanyOS.Domain.Exceptions;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Entities;

/// <summary>
/// Aggregate Root — represents a meeting between two or more AI agents.
/// Business rules:
///   - A meeting requires at least 2 participants.
///   - Only Scheduled meetings can be started.
///   - Only InProgress meetings can be concluded.
///   - Concluded or Cancelled meetings are immutable.
/// </summary>
public sealed class Meeting : AggregateRoot<MeetingId>
{
    private readonly List<AgentId> _participantIds = [];
    private readonly List<Message> _messages       = [];

    /// <summary>Topic or agenda of the meeting.</summary>
    public string Topic { get; private set; }

    /// <summary>Current status of the meeting.</summary>
    public MeetingStatus Status { get; private set; }

    /// <summary>The agent who organized/scheduled this meeting.</summary>
    public AgentId OrganizedByAgentId { get; private init; }

    /// <summary>UTC timestamp when the meeting was scheduled.</summary>
    public DateTime ScheduledOnUtc { get; private init; }

    /// <summary>UTC timestamp when the meeting actually started.</summary>
    public DateTime? StartedOnUtc { get; private set; }

    /// <summary>UTC timestamp when the meeting concluded.</summary>
    public DateTime? ConcludedOnUtc { get; private set; }

    /// <summary>Optional summary written after the meeting concludes.</summary>
    public string? Summary { get; private set; }

    /// <summary>Read-only list of agent IDs participating in this meeting.</summary>
    public IReadOnlyList<AgentId> ParticipantIds => _participantIds.AsReadOnly();

    /// <summary>Read-only list of messages exchanged during this meeting.</summary>
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

#pragma warning disable CS8618
    private Meeting() { } // ORM — properties are set by the persistence layer
#pragma warning restore CS8618

    private Meeting(MeetingId id, string topic, AgentId organizer, IEnumerable<AgentId> participants)
        : base(id)
    {
        Topic              = topic;
        OrganizedByAgentId = organizer;
        Status             = MeetingStatus.Scheduled;
        ScheduledOnUtc     = DateTime.UtcNow;
        _participantIds.AddRange(participants);
    }

    /// <summary>
    /// Factory method — schedules a new meeting.
    /// Enforces the minimum 2-participant rule.
    /// </summary>
    public static Meeting Schedule(string topic, AgentId organizer, IEnumerable<AgentId> participants)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Meeting topic cannot be empty.", nameof(topic));

        var participantList = participants?.ToList()
            ?? throw new ArgumentNullException(nameof(participants));

        // Ensure organizer is included in participants
        if (!participantList.Contains(organizer))
            participantList.Insert(0, organizer);

        // Deduplicate
        var distinct = participantList.Distinct().ToList();

        if (distinct.Count < 2)
            throw new InvalidMeetingOperationException(
                "A meeting requires at least 2 distinct participants.");

        return new Meeting(MeetingId.New(), topic.Trim(), organizer, distinct);
    }

    // -------------------------------------------------------------------------
    // Behavior
    // -------------------------------------------------------------------------

    /// <summary>
    /// Starts the meeting. Only valid from Scheduled status.
    /// </summary>
    public void Start()
    {
        if (Status != MeetingStatus.Scheduled)
            throw new InvalidMeetingOperationException(
                $"Meeting '{Topic}' cannot be started from status '{Status}'.");

        Status       = MeetingStatus.InProgress;
        StartedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(MeetingStartedEvent.Create(Id, _participantIds.AsReadOnly()));
    }

    /// <summary>
    /// Concludes the meeting with an optional summary.
    /// Only valid from InProgress status.
    /// </summary>
    public void Conclude(string? summary = null)
    {
        if (Status != MeetingStatus.InProgress)
            throw new InvalidMeetingOperationException(
                $"Meeting '{Topic}' cannot be concluded from status '{Status}'.");

        Status         = MeetingStatus.Concluded;
        Summary        = summary?.Trim();
        ConcludedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(MeetingConcludedEvent.Create(Id, Summary));
    }

    /// <summary>
    /// Cancels the meeting. Cannot cancel a concluded meeting.
    /// </summary>
    public void Cancel()
    {
        if (Status == MeetingStatus.Concluded)
            throw new InvalidMeetingOperationException(
                "A concluded meeting cannot be cancelled.");

        if (Status == MeetingStatus.Cancelled)
            return; // idempotent

        Status = MeetingStatus.Cancelled;
    }

    /// <summary>
    /// Adds a participant to a scheduled meeting.
    /// Invariant: participants can only be added while the meeting is Scheduled.
    /// </summary>
    public void AddParticipant(AgentId agentId)
    {
        if (Status != MeetingStatus.Scheduled)
            throw new InvalidMeetingOperationException(
                "Participants can only be added to a Scheduled meeting.");

        if (!_participantIds.Contains(agentId))
            _participantIds.Add(agentId);
    }

    /// <summary>
    /// Posts a message into the meeting's conversation thread and raises
    /// <see cref="MessageSentEvent"/> so the Application layer can fan out
    /// notifications or persist the message independently.
    ///
    /// Invariants:
    ///   - Meeting must be InProgress.
    ///   - Sender must be a registered participant.
    /// </summary>
    public void PostMessage(Message message)
    {
        if (Status != MeetingStatus.InProgress)
            throw new InvalidMeetingOperationException(
                "Messages can only be posted to an InProgress meeting.");

        if (!_participantIds.Contains(message.SenderId))
            throw new InvalidMeetingOperationException(
                $"Agent '{message.SenderId}' is not a participant in this meeting.");

        _messages.Add(message);

        RaiseDomainEvent(MessageSentEvent.Create(message.Id, message.SenderId, message.RecipientId));
    }
}
