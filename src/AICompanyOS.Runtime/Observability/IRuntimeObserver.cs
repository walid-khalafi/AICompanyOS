using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Observability;

/// <summary>
/// Receives and processes runtime observability events during execution.
///
/// Implementations may forward events to:
///   - Structured logging (Serilog, Microsoft.Extensions.Logging)
///   - OpenTelemetry traces and spans
///   - In-memory trace buffers (for testing)
///   - External monitoring systems (future)
///
/// The observer is called by the runtime engine at key execution points.
/// It must not throw — errors in the observer must not affect execution.
/// </summary>
public interface IRuntimeObserver
{
    /// <summary>Called when a runtime event is emitted during execution.</summary>
    Task OnEventAsync(RuntimeEvent runtimeEvent, CancellationToken cancellationToken = default);

    /// <summary>Called when an execution log entry is produced.</summary>
    Task OnLogAsync(RuntimeExecutionLog logEntry, CancellationToken cancellationToken = default);

    /// <summary>Called when a complete session trace is available (session end).</summary>
    Task OnTraceAsync(RuntimeTrace trace, CancellationToken cancellationToken = default);
}
