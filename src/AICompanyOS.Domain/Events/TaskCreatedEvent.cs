using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a new task is created in the system.
/// </summary>
public sealed record TaskCreatedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    TaskId TaskId,
    string Title,
    Priority Priority
) : IDomainEvent
{
    public static TaskCreatedEvent Create(TaskId taskId, string title, Priority priority) =>
        new(Guid.NewGuid(), DateTime.UtcNow, taskId, title, priority);
}
