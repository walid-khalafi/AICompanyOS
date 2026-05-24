using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a task is assigned to an agent.
/// </summary>
public sealed record TaskAssignedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    TaskId TaskId,
    AgentId AssignedToAgentId
) : IDomainEvent
{
    public static TaskAssignedEvent Create(TaskId taskId, AgentId agentId) =>
        new(Guid.NewGuid(), DateTime.UtcNow, taskId, agentId);
}
