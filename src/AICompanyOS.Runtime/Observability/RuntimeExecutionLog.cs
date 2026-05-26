using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Observability;

/// <summary>
/// A structured log entry for a single execution action within a session.
///
/// Execution logs are more granular than <see cref="RuntimeEvent"/> — they
/// capture individual LLM interactions, token usage, latency, and step-level
/// details. They are intended for detailed performance analysis and cost tracking.
/// </summary>
public sealed class RuntimeExecutionLog
{
    /// <summary>Unique identifier for this log entry.</summary>
    public Guid LogId { get; } = Guid.NewGuid();

    /// <summary>The session this log entry belongs to.</summary>
    public Guid SessionId { get; init; }

    /// <summary>The execution step this log entry is associated with.</summary>
    public Guid StepId { get; init; }

    /// <summary>The runtime agent that produced this log entry.</summary>
    public string AgentRuntimeId { get; init; } = string.Empty;

    /// <summary>The action being logged (e.g., "llm-request", "tool-call", "plan-step").</summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>Human-readable detail about the action.</summary>
    public string Detail { get; init; } = string.Empty;

    /// <summary>Number of input tokens consumed (LLM interactions only).</summary>
    public int? InputTokens { get; init; }

    /// <summary>Number of output tokens produced (LLM interactions only).</summary>
    public int? OutputTokens { get; init; }

    /// <summary>Wall-clock duration of the logged action.</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>UTC timestamp when this log entry was recorded.</summary>
    public DateTime RecordedAtUtc { get; } = DateTime.UtcNow;

    /// <summary>Whether this log entry represents an error condition.</summary>
    public bool IsError { get; init; }

    /// <summary>Error message if this is an error log entry.</summary>
    public string? ErrorMessage { get; init; }
}
