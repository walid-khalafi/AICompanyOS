namespace AICompanyOS.Runtime.Events;

/// <summary>
/// Raised by the runtime engine when an execution session starts.
///
/// Consumed by the Application layer to transition the domain Task
/// from Assigned → InProgress and the domain Agent from Idle → Busy.
/// </summary>
public sealed class RuntimeExecutionStartedEvent
{
    /// <summary>Unique identifier for this event.</summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>The session that started.</summary>
    public Guid SessionId { get; init; }

    /// <summary>The domain task ID this session is executing.</summary>
    public Guid DomainTaskId { get; init; }

    /// <summary>The runtime agent executing the session.</summary>
    public string AgentRuntimeId { get; init; } = string.Empty;

    /// <summary>The domain agent ID as a raw Guid.</summary>
    public Guid DomainAgentId { get; init; }

    /// <summary>Correlation ID for end-to-end tracing.</summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>UTC timestamp when this event was raised.</summary>
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
