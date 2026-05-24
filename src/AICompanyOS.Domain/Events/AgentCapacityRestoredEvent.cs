using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when an agent's active task count drops below its maximum concurrent limit.
/// The Orchestration layer listens to this event to resume routing tasks
/// to this agent.
/// </summary>
public sealed record AgentCapacityRestoredEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    AgentId AgentId,
    int ActiveTaskCount,
    int MaxConcurrentTasks,
    int RemainingCapacity
) : IDomainEvent
{
    public static AgentCapacityRestoredEvent Create(
        AgentId agentId,
        int activeTaskCount,
        int maxConcurrentTasks) =>
        new(Guid.NewGuid(), DateTime.UtcNow, agentId, activeTaskCount, maxConcurrentTasks,
            Math.Max(0, maxConcurrentTasks - activeTaskCount));
}
