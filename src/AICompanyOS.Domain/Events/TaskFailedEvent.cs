using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a task execution fails.
/// </summary>
public sealed record TaskFailedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    TaskId TaskId,
    AgentId? FailedByAgentId,
    string Reason
) : IDomainEvent
{
    public static TaskFailedEvent Create(TaskId taskId, AgentId? agentId, string reason) =>
        new(Guid.NewGuid(), DateTime.UtcNow, taskId, agentId, reason);
}
