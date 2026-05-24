using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when an agent transitions between operational states.
/// </summary>
public sealed record AgentStatusChangedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    AgentId AgentId,
    AgentStatus PreviousStatus,
    AgentStatus NewStatus
) : IDomainEvent
{
    public static AgentStatusChangedEvent Create(
        AgentId agentId,
        AgentStatus previousStatus,
        AgentStatus newStatus) =>
        new(Guid.NewGuid(), DateTime.UtcNow, agentId, previousStatus, newStatus);
}
