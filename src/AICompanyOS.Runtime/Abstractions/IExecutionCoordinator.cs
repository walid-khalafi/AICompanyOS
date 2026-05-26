using AICompanyOS.Runtime.Context;
using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Abstractions;

/// <summary>
/// Coordinates the execution of an <see cref="ExecutionPlan"/> step by step.
///
/// The coordinator drives the planner/executor loop:
///   1. Receives a plan from the <see cref="IExecutionPlanner"/>.
///   2. Iterates through each <see cref="ExecutionStep"/>.
///   3. Dispatches each step to the appropriate agent runtime.
///   4. Handles tool call results and feeds them back into the loop.
///   5. Produces a final <see cref="ExecutionResult"/> when the plan completes.
///
/// The coordinator is the central orchestration engine of the Runtime layer.
/// It does NOT contain business logic — that lives in the Domain and Application layers.
/// </summary>
public interface IExecutionCoordinator
{
    /// <summary>
    /// Executes a plan within the given agent runtime context.
    /// </summary>
    /// <param name="plan">The execution plan to run.</param>
    /// <param name="context">The agent runtime context for this execution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The final result of the plan execution.</returns>
    Task<ExecutionResult> ExecutePlanAsync(
        ExecutionPlan plan,
        AgentRuntimeContext context,
        CancellationToken cancellationToken = default);
}
