using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when an agent sends a message to another agent.
/// </summary>
public sealed record MessageSentEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    MessageId MessageId,
    AgentId SenderId,
    AgentId RecipientId
) : IDomainEvent
{
    public static MessageSentEvent Create(MessageId messageId, AgentId senderId, AgentId recipientId) =>
        new(Guid.NewGuid(), DateTime.UtcNow, messageId, senderId, recipientId);
}
