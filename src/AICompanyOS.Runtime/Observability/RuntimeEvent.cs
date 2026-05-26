using AICompanyOS.Runtime.Observability;

namespace AICompanyOS.Runtime.Models;

/// <summary>
/// A single structured event emitted during runtime execution.
///
/// Runtime events form the live observability stream for the AI execution engine.
/// They are accumulated in the <see cref="ExecutionSession"/> trace log and can
/// be forwarded to external observability systems (OpenTelemetry, structured logs).
///
/// Not a Domain event — runtime events carry no business meaning and are
/// not dispatched through the Application layer event pipeline.
/// </summary>
public sealed class RuntimeEvent
{
    /// <summary>Unique identifier for this event.</summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>The session this event belongs to.</summary>
    public Guid SessionId { get; init; }

    /// <summary>Correlation ID linking this event to the originating request.</summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>The type/category of this event.</summary>
    public RuntimeEventType EventType { get; init; }

    /// <summary>Human-readable description of what happened.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>UTC timestamp when this event occurred.</summary>
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;

    /// <summary>Optional structured payload for this event.</summary>
    public IReadOnlyDictionary<string, object> Payload { get; init; } =
        new Dictionary<string, object>();

    /// <summary>Creates an informational runtime event.</summary>
    public static RuntimeEvent Info(Guid sessionId, string correlationId, string message,
        IReadOnlyDictionary<string, object>? payload = null) =>
        new()
        {
            SessionId     = sessionId,
            CorrelationId = correlationId,
            EventType     = RuntimeEventType.Information,
            Message       = message,
            Payload       = payload ?? new Dictionary<string, object>()
        };

    /// <summary>Creates a typed runtime event with a structured payload.</summary>
    public static RuntimeEvent Create(Guid sessionId, string correlationId,
        RuntimeEventType eventType, string message,
        IReadOnlyDictionary<string, object>? payload = null) =>
        new()
        {
            SessionId     = sessionId,
            CorrelationId = correlationId,
            EventType     = eventType,
            Message       = message,
            Payload       = payload ?? new Dictionary<string, object>()
        };
}
