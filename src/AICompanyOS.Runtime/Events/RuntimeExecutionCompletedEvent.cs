using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Events;

/// <summary>
/// Raised by the runtime engine when an execution session completes
/// (successfully or with failure).
///
/// This is a RUNTIME event — not a Domain event.
/// It is consumed by the Application layer to trigger the appropriate
/// domain command (Task.Complete, Task.Fail, etc.).
///
/// The Application layer is the bridge between runtime events and domain commands.
/// The runtime never issues domain commands directly.
/// </summary>
public sealed class RuntimeExecutionCompletedEvent
{
    /// <summary>Unique identifier for this event.</summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>The session that completed.</summary>
    public Guid SessionId { get; init; }

    /// <summary>The domain task ID this session was executing.</summary>
    public Guid DomainTaskId { get; init; }

    /// <summary>The runtime agent that executed the session.</summary>
    public string AgentRuntimeId { get; init; } = string.Empty;

    /// <summary>The final result of the execution.</summary>
    public ExecutionResult Result { get; init; } = ExecutionResult.Failure("Not set.");

    /// <summary>Correlation ID for end-to-end tracing.</summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>UTC timestamp when this event was raised.</summary>
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
