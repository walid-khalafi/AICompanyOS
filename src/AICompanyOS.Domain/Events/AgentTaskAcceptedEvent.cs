using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when an agent accepts a task into its active workload.
/// Distinct from TaskAssignedEvent (which is raised on the Task aggregate) —
/// this event is raised on the Agent aggregate and signals that the agent's
/// internal workload counter has been incremented.
/// </summary>
public sealed record AgentTaskAcceptedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    AgentId AgentId,
    TaskId TaskId,
    int NewActiveTaskCount,
    int MaxConcurrentTasks
) : IDomainEvent
{
    public static AgentTaskAcceptedEvent Create(
        AgentId agentId,
        TaskId taskId,
        int newActiveTaskCount,
        int maxConcurrentTasks) =>
        new(Guid.NewGuid(), DateTime.UtcNow, agentId, taskId, newActiveTaskCount, maxConcurrentTasks);
}
