using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when an agent's active task count reaches its maximum concurrent limit.
/// The Orchestration layer listens to this event to stop routing new tasks
/// to this agent until capacity is restored.
/// </summary>
public sealed record AgentCapacityReachedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    AgentId AgentId,
    int ActiveTaskCount,
    int MaxConcurrentTasks
) : IDomainEvent
{
    public static AgentCapacityReachedEvent Create(
        AgentId agentId,
        int activeTaskCount,
        int maxConcurrentTasks) =>
        new(Guid.NewGuid(), DateTime.UtcNow, agentId, activeTaskCount, maxConcurrentTasks);
}
