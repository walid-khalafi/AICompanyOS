using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a task is marked as completed.
/// </summary>
public sealed record TaskCompletedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    TaskId TaskId,
    AgentId CompletedByAgentId
) : IDomainEvent
{
    public static TaskCompletedEvent Create(TaskId taskId, AgentId agentId) =>
        new(Guid.NewGuid(), DateTime.UtcNow, taskId, agentId);
}
