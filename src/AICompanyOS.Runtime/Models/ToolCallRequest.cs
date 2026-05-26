namespace AICompanyOS.Runtime.Models;

/// <summary>
/// Represents a tool call requested by an agent during an execution step.
///
/// When an LLM decides to invoke a tool, the runtime captures the request
/// here before dispatching it to the <see cref="AICompanyOS.Runtime.Abstractions.IToolExecutor"/>.
/// </summary>
public sealed class ToolCallRequest
{
    /// <summary>Unique identifier for this tool call instance.</summary>
    public Guid CallId { get; } = Guid.NewGuid();

    /// <summary>The registered name of the tool to invoke.</summary>
    public string ToolName { get; init; } = string.Empty;

    /// <summary>
    /// Arguments passed to the tool, serialized as a JSON string.
    /// The tool implementation is responsible for deserializing.
    /// </summary>
    public string ArgumentsJson { get; init; } = "{}";

    /// <summary>UTC timestamp when the tool call was requested.</summary>
    public DateTime RequestedAtUtc { get; } = DateTime.UtcNow;
}

/// <summary>
/// Represents the result of a completed tool call.
/// </summary>
public sealed class ToolCallResult
{
    /// <summary>The call ID this result corresponds to.</summary>
    public Guid CallId { get; init; }

    /// <summary>Whether the tool executed successfully.</summary>
    public bool IsSuccess { get; init; }

    /// <summary>The output produced by the tool, serialized as a JSON string.</summary>
    public string? OutputJson { get; init; }

    /// <summary>Error message if the tool failed.</summary>
    public string? ErrorMessage { get; init; }

    /// <summary>UTC timestamp when the tool call completed.</summary>
    public DateTime CompletedAtUtc { get; } = DateTime.UtcNow;
}
