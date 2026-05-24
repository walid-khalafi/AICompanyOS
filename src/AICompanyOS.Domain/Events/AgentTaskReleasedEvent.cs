using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when an agent releases a task from its active workload
/// (task completed, failed, or unassigned).
/// The Orchestration layer uses this to know the agent has freed capacity.
/// </summary>
public sealed record AgentTaskReleasedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    AgentId AgentId,
    TaskId TaskId,
    int NewActiveTaskCount,
    int MaxConcurrentTasks
) : IDomainEvent
{
    public static AgentTaskReleasedEvent Create(
        AgentId agentId,
        TaskId taskId,
        int newActiveTaskCount,
        int maxConcurrentTasks) =>
        new(Guid.NewGuid(), DateTime.UtcNow, agentId, taskId, newActiveTaskCount, maxConcurrentTasks);
}
