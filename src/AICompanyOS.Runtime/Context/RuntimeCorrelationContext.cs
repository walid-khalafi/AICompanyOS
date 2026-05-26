namespace AICompanyOS.Runtime.Context;

/// <summary>
/// Carries correlation and tracing identifiers across the runtime execution boundary.
///
/// This is the runtime equivalent of the Application layer's CorrelationId.
/// It links a runtime session back to the originating API request and
/// Application layer command, enabling end-to-end distributed tracing.
///
/// Ephemeral — scoped to a single execution session lifetime.
/// </summary>
public sealed class RuntimeCorrelationContext
{
    /// <summary>
    /// The correlation ID originating from the API request.
    /// Propagated from the Application layer into the Runtime.
    /// </summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>
    /// The runtime session ID this context belongs to.
    /// </summary>
    public Guid SessionId { get; init; }

    /// <summary>
    /// Optional parent trace ID for nested execution scenarios
    /// (e.g., a sub-agent spawned by a parent agent).
    /// </summary>
    public string? ParentTraceId { get; init; }

    /// <summary>UTC timestamp when this correlation context was created.</summary>
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;

    /// <summary>Creates a new root-level correlation context for a session.</summary>
    public static RuntimeCorrelationContext Create(string correlationId, Guid sessionId) =>
        new() { CorrelationId = correlationId, SessionId = sessionId };

    /// <summary>Creates a child correlation context for a spawned sub-execution.</summary>
    public RuntimeCorrelationContext CreateChild(Guid childSessionId) =>
        new()
        {
            CorrelationId = CorrelationId,
            SessionId     = childSessionId,
            ParentTraceId = SessionId.ToString()
        };
}
