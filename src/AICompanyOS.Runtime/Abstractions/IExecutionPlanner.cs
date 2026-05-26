using AICompanyOS.Runtime.Context;
using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Abstractions;

/// <summary>
/// Produces an <see cref="ExecutionPlan"/> from an agent's runtime context.
///
/// The planner is responsible for decomposing a high-level task goal into
/// a sequence of discrete <see cref="ExecutionStep"/> instances that the
/// executor can process one at a time.
///
/// Planning strategies (future implementations may include):
///   - Single-step: one instruction, one agent, one result.
///   - Sequential: ordered steps executed one after another.
///   - Parallel: steps that can be executed concurrently.
///   - Reactive: steps generated dynamically based on prior results.
///
/// No AI provider integration here — this is a pure contract.
/// </summary>
public interface IExecutionPlanner
{
    /// <summary>
    /// Creates an execution plan for the given agent runtime context.
    /// </summary>
    /// <param name="context">
    /// The agent's runtime context, including task goal, working memory,
    /// and conversation history.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A structured <see cref="ExecutionPlan"/> ready for the coordinator to execute.
    /// </returns>
    Task<ExecutionPlan> CreatePlanAsync(
        AgentRuntimeContext context,
        CancellationToken cancellationToken = default);
}
