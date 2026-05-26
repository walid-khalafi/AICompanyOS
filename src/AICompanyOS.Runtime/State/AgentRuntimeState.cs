using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.State;

/// <summary>
/// Represents the live runtime state of a single agent across all its active sessions.
///
/// This is distinct from the Domain Agent aggregate's status. The Domain tracks
/// durable business state (Idle, Busy, Suspended). The runtime state tracks
/// ephemeral execution state (how many sessions are active, what they are doing).
///
/// The Application layer is responsible for keeping Domain state and Runtime state
/// in sync by issuing domain commands when runtime state changes.
/// </summary>
public sealed class AgentRuntimeState
{
    /// <summary>The runtime agent identifier.</summary>
    public string AgentRuntimeId { get; init; } = string.Empty;

    /// <summary>The domain agent ID as a raw Guid.</summary>
    public Guid DomainAgentId { get; init; }

    /// <summary>IDs of all currently active execution sessions for this agent.</summary>
    public IReadOnlyList<Guid> ActiveSessionIds => _activeSessionIds.AsReadOnly();

    /// <summary>Number of currently active sessions.</summary>
    public int ActiveSessionCount => _activeSessionIds.Count;

    /// <summary>Maximum concurrent sessions this agent supports.</summary>
    public int MaxConcurrentSessions { get; init; } = 1;

    /// <summary>Whether this agent has capacity for another session.</summary>
    public bool HasCapacity => ActiveSessionCount < MaxConcurrentSessions;

    /// <summary>UTC timestamp of the last state change.</summary>
    public DateTime LastUpdatedAtUtc { get; private set; } = DateTime.UtcNow;

    private readonly List<Guid> _activeSessionIds = [];

    /// <summary>Registers a new active session for this agent.</summary>
    public void AddSession(Guid sessionId)
    {
        if (!_activeSessionIds.Contains(sessionId))
        {
            _activeSessionIds.Add(sessionId);
            LastUpdatedAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>Removes a completed or failed session from this agent's active list.</summary>
    public void RemoveSession(Guid sessionId)
    {
        _activeSessionIds.Remove(sessionId);
        LastUpdatedAtUtc = DateTime.UtcNow;
    }
}
