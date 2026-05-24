using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a task is cancelled before reaching completion or failure.
/// The Application layer can use this event to release agent capacity,
/// notify stakeholders, or trigger compensating workflows.
/// </summary>
public sealed record TaskCancelledEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    TaskId TaskId,
    AgentId CancelledByAgentId
) : IDomainEvent
{
    public static TaskCancelledEvent Create(TaskId taskId, AgentId cancelledBy) =>
        new(Guid.NewGuid(), DateTime.UtcNow, taskId, cancelledBy);
}
