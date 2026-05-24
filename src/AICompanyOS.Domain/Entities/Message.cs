using AICompanyOS.Domain.Events;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Entities;

/// <summary>
/// Entity — represents a message sent from one agent to another.
/// Messages are immutable once created; they are a record of communication.
/// </summary>
public sealed class Message : Entity<MessageId>
{
    /// <summary>The agent who sent this message.</summary>
    public AgentId SenderId { get; private init; }

    /// <summary>The agent this message is addressed to.</summary>
    public AgentId RecipientId { get; private init; }

    /// <summary>The body of the message.</summary>
    public MessageContent Content { get; private init; }

    /// <summary>UTC timestamp when the message was sent.</summary>
    public DateTime SentOnUtc { get; private init; }

    /// <summary>Optional reference to a meeting this message belongs to.</summary>
    public MeetingId? MeetingId { get; private init; }

    /// <summary>Optional reference to a task this message is about.</summary>
    public TaskId? RelatedTaskId { get; private init; }

#pragma warning disable CS8618
    private Message() { } // ORM — properties are set by the persistence layer
#pragma warning restore CS8618

    private Message(
        MessageId id,
        AgentId senderId,
        AgentId recipientId,
        MessageContent content,
        MeetingId? meetingId,
        TaskId? relatedTaskId)
        : base(id)
    {
        SenderId = senderId;
        RecipientId = recipientId;
        Content = content;
        SentOnUtc = DateTime.UtcNow;
        MeetingId = meetingId;
        RelatedTaskId = relatedTaskId;
    }

    /// <summary>
    /// Factory method — creates a direct message between two agents.
    /// </summary>
    public static Message Send(
        AgentId senderId,
        AgentId recipientId,
        MessageContent content,
        TaskId? relatedTaskId = null)
    {
        if (senderId == recipientId)
            throw new ArgumentException("An agent cannot send a message to itself.");

        return new Message(MessageId.New(), senderId, recipientId, content, null, relatedTaskId);
    }

    /// <summary>
    /// Factory method — creates a message within a meeting context.
    /// </summary>
    public static Message SendInMeeting(
        AgentId senderId,
        AgentId recipientId,
        MessageContent content,
        MeetingId meetingId)
    {
        if (senderId == recipientId)
            throw new ArgumentException("An agent cannot send a message to itself.");

        return new Message(MessageId.New(), senderId, recipientId, content, meetingId, null);
    }
}
