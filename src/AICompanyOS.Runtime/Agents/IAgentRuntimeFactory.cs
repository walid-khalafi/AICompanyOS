using AICompanyOS.Runtime.Context;

namespace AICompanyOS.Runtime.Agents;

/// <summary>
/// Creates <see cref="AgentRuntimeContext"/> instances for agent execution sessions.
///
/// The factory is responsible for assembling the full runtime context from
/// a descriptor and a task goal. It initializes working memory, conversation
/// memory, correlation context, and execution metadata.
///
/// Called by the Application layer before invoking <see cref="AICompanyOS.Runtime.Abstractions.IExecutionRuntime"/>.
/// </summary>
public interface IAgentRuntimeFactory
{
    /// <summary>
    /// Creates a fully initialized <see cref="AgentRuntimeContext"/> ready for execution.
    /// </summary>
    /// <param name="descriptor">The runtime descriptor for the agent.</param>
    /// <param name="domainTaskId">The domain task ID this session will execute.</param>
    /// <param name="taskGoal">The natural-language goal for the agent to achieve.</param>
    /// <param name="correlationId">The correlation ID from the originating request.</param>
    /// <param name="metadata">Optional execution metadata overrides.</param>
    /// <returns>A fully constructed <see cref="AgentRuntimeContext"/>.</returns>
    AgentRuntimeContext Create(
        AgentRuntimeDescriptor descriptor,
        Guid domainTaskId,
        string taskGoal,
        string correlationId,
        RuntimeExecutionMetadata? metadata = null);
}
