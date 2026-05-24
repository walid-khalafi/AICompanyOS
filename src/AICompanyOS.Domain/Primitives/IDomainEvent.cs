namespace AICompanyOS.Domain.Primitives;

/// <summary>
/// Marker interface for all domain events in the system.
/// Domain events are immutable notifications that something meaningful happened
/// within the domain. They are raised by aggregates and consumed by handlers
/// in the Application layer.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Unique identifier for this event occurrence.</summary>
    Guid EventId { get; }

    /// <summary>UTC timestamp of when the event occurred.</summary>
    DateTime OccurredOnUtc { get; }
}
