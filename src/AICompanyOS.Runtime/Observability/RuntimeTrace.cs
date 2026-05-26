using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Observability;

/// <summary>
/// A complete execution trace for a single runtime session.
///
/// A trace aggregates all <see cref="RuntimeEvent"/> instances produced
/// during a session into a structured, queryable log. It is the primary
/// artifact for post-execution debugging and observability.
///
/// Produced at session completion by the runtime engine.
/// May be forwarded to an external observability sink (future phase).
/// </summary>
public sealed class RuntimeTrace
{
    /// <summary>Unique identifier for this trace.</summary>
    public Guid TraceId { get; } = Guid.NewGuid();

    /// <summary>The session this trace covers.</summary>
    public Guid SessionId { get; init; }

    /// <summary>Correlation ID linking this trace to the originating request.</summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>The runtime agent identifier that produced this trace.</summary>
    public string AgentRuntimeId { get; init; } = string.Empty;

    /// <summary>All events recorded during the session, in chronological order.</summary>
    public IReadOnlyList<RuntimeEvent> Events { get; init; } = [];

    /// <summary>UTC timestamp when the session started.</summary>
    public DateTime SessionStartedAtUtc { get; init; }

    /// <summary>UTC timestamp when the session ended.</summary>
    public DateTime SessionEndedAtUtc { get; init; }

    /// <summary>Total wall-clock duration of the session.</summary>
    public TimeSpan Duration => SessionEndedAtUtc - SessionStartedAtUtc;

    /// <summary>Final status of the session.</summary>
    public ExecutionStatus FinalStatus { get; init; }

    /// <summary>Returns all events of a specific type.</summary>
    public IEnumerable<RuntimeEvent> GetEventsByType(RuntimeEventType eventType) =>
        Events.Where(e => e.EventType == eventType);

    /// <summary>Returns true if the trace contains any error events.</summary>
    public bool HasErrors =>
        Events.Any(e => e.EventType == RuntimeEventType.Error);
}
