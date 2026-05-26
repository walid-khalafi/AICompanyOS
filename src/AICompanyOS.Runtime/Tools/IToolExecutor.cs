using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Tools;

/// <summary>
/// Executes tool call requests dispatched by the runtime execution engine.
///
/// The executor is responsible for:
///   1. Resolving the correct <see cref="ITool"/> from the registry.
///   2. Validating the request arguments against the tool's input schema.
///   3. Invoking the tool and returning the result.
///   4. Handling execution errors gracefully (never throwing to the caller).
///
/// The executor is the single entry point for all tool invocations.
/// It decouples the execution engine from individual tool implementations.
/// </summary>
public interface IToolExecutor
{
    /// <summary>
    /// Executes a single tool call request.
    /// </summary>
    /// <param name="request">The tool call request to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the tool execution.</returns>
    Task<ToolCallResult> ExecuteAsync(ToolCallRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes multiple tool call requests in parallel.
    /// Used when the LLM issues multiple tool calls in a single response.
    /// </summary>
    /// <param name="requests">The tool call requests to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Results in the same order as the input requests.</returns>
    Task<IReadOnlyList<ToolCallResult>> ExecuteManyAsync(
        IReadOnlyList<ToolCallRequest> requests,
        CancellationToken cancellationToken = default);
}
