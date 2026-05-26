namespace AICompanyOS.Runtime.Agents;

/// <summary>
/// Describes a runtime agent — its identity, role, capabilities, and configuration.
///
/// This is the runtime representation of an agent. It is NOT a Domain aggregate.
/// It carries the information the runtime engine needs to construct an
/// <see cref="AICompanyOS.Runtime.Context.AgentRuntimeContext"/> and route
/// execution to the correct agent implementation.
///
/// Populated from the Domain Agent aggregate by the Application layer
/// before initiating a runtime session.
/// </summary>
public sealed class AgentRuntimeDescriptor
{
    /// <summary>
    /// The runtime identity string for this agent (e.g., "ceo-agent-01").
    /// Derived from the Domain agent's name at session construction time.
    /// </summary>
    public string RuntimeId { get; init; } = string.Empty;

    /// <summary>
    /// The domain agent's ID as a raw Guid.
    /// Used by the Application layer to correlate runtime results back to domain state.
    /// </summary>
    public Guid DomainAgentId { get; init; }

    /// <summary>
    /// The agent's role name (e.g., "CEO", "Developer", "QA").
    /// Stored as a string to avoid Domain enum coupling.
    /// </summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// The set of capability names this agent possesses.
    /// Used by the tool registry to filter available tools per agent.
    /// </summary>
    public IReadOnlyList<string> Capabilities { get; init; } = [];

    /// <summary>
    /// The system prompt template for this agent.
    /// Injected into the conversation memory at session start.
    /// </summary>
    public string SystemPrompt { get; init; } = string.Empty;

    /// <summary>
    /// Maximum number of concurrent tasks this agent can handle.
    /// Mirrors the domain-level capacity concept without coupling to Domain.
    /// </summary>
    public int MaxConcurrentTasks { get; init; } = 1;
}
