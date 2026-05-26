using AICompanyOS.Runtime.Memory;
using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Context;

/// <summary>
/// The complete runtime context for a single agent execution session.
///
/// This is the central object passed through the runtime execution pipeline.
/// It bundles together everything the agent needs to execute:
/// correlation context, execution metadata, working memory, and the
/// current session state.
///
/// Design rules:
///   - Does NOT reference any Domain aggregate directly.
///   - Domain IDs are carried as raw Guids to avoid project coupling.
///   - Ephemeral — scoped to one session, not persisted.
/// </summary>
public sealed class AgentRuntimeContext
{
    /// <summary>Correlation and tracing identifiers for this execution.</summary>
    public RuntimeCorrelationContext Correlation { get; init; } = new();

    /// <summary>Execution configuration and policy for this session.</summary>
    public RuntimeExecutionMetadata Metadata { get; init; } = new();

    /// <summary>
    /// The runtime agent identifier (e.g., "ceo-agent-01").
    /// Not a Domain AgentId — this is a runtime-scoped string identity.
    /// </summary>
    public string AgentRuntimeId { get; init; } = string.Empty;

    /// <summary>
    /// The agent's role name as a string (e.g., "CEO", "Developer", "QA").
    /// Stored as a string to avoid Domain enum coupling.
    /// </summary>
    public string AgentRole { get; init; } = string.Empty;

    /// <summary>
    /// The domain Task ID this agent is executing against.
    /// Raw Guid — the Application layer resolves this to a domain Task.
    /// </summary>
    public Guid DomainTaskId { get; init; }

    /// <summary>The active execution session for this context.</summary>
    public ExecutionSession Session { get; init; } = new();

    /// <summary>The agent's working memory for this execution.</summary>
    public WorkingMemory WorkingMemory { get; init; } = new();

    /// <summary>The conversation history for this execution.</summary>
    public ConversationMemory ConversationMemory { get; init; } = new();

    /// <summary>UTC timestamp when this context was constructed.</summary>
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;
}
