using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Tools;

/// <summary>
/// Contract for a single executable tool available to AI agents.
///
/// Tools are the mechanism by which agents interact with the outside world:
/// reading files, calling APIs, issuing domain commands, searching the web, etc.
///
/// Each tool implementation must be:
///   - Stateless (no mutable instance state between calls)
///   - Idempotent where possible
///   - Self-describing via <see cref="Name"/> and <see cref="Description"/>
///
/// Tool implementations live in the Infrastructure or Agents layer.
/// This interface lives in Runtime to keep the contract isolated.
/// </summary>
public interface ITool
{
    /// <summary>
    /// The unique registered name of this tool.
    /// Used by the LLM to reference the tool in tool-call requests.
    /// Convention: lowercase, hyphen-separated (e.g., "read-file", "create-task").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Human-readable description of what this tool does.
    /// Included in the LLM system prompt to help the model decide when to use it.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// JSON Schema string describing the tool's input parameters.
    /// Used to validate <see cref="ToolCallRequest.ArgumentsJson"/> before execution.
    /// </summary>
    string InputSchema { get; }

    /// <summary>
    /// Executes the tool with the provided arguments.
    /// </summary>
    /// <param name="request">The tool call request containing serialized arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the tool execution.</returns>
    Task<ToolCallResult> ExecuteAsync(ToolCallRequest request, CancellationToken cancellationToken = default);
}
