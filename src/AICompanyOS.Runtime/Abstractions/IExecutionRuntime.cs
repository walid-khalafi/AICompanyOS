using AICompanyOS.Runtime.Context;
using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Abstractions;

/// <summary>
/// The top-level entry point into the AI Runtime Layer.
///
/// The execution runtime is the facade that the Application layer calls
/// to initiate agent execution. It encapsulates the full plan → coordinate
/// → result lifecycle behind a single method.
///
/// Typical call flow:
///   Application layer
///     → IExecutionRuntime.RunAsync(context)
///       → IExecutionPlanner.CreatePlanAsync(context)
///       → IExecutionCoordinator.ExecutePlanAsync(plan, context)
///       → ExecutionResult
///     → Application layer issues domain command (Task.Complete / Task.Fail)
///
/// The runtime does NOT issue domain commands itself.
/// That responsibility stays in the Application layer.
/// </summary>
public interface IExecutionRuntime
{
    /// <summary>
    /// Runs a complete agent execution session from start to finish.
    /// </summary>
    /// <param name="context">
    /// The fully constructed agent runtime context, including task goal,
    /// correlation context, memory, and execution metadata.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The final <see cref="ExecutionResult"/> of the session.
    /// The caller is responsible for translating this into domain commands.
    /// </returns>
    Task<ExecutionResult> RunAsync(
        AgentRuntimeContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the current status of an active execution session.
    /// Returns null if no session with the given ID is active.
    /// </summary>
    Task<ExecutionSession?> GetSessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);
}
