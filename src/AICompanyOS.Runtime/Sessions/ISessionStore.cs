using AICompanyOS.Runtime.Models;

namespace AICompanyOS.Runtime.Sessions;

/// <summary>
/// In-memory store for active execution sessions.
///
/// The session store tracks all currently running sessions so the runtime
/// can look them up by ID (e.g., for status queries or cancellation).
///
/// This is an in-process, in-memory store only.
/// Sessions are NOT persisted — they exist only for the duration of execution.
/// Durable session storage is a future concern (Phase 8+).
/// </summary>
public interface ISessionStore
{
    /// <summary>Registers a new session in the store.</summary>
    void Register(ExecutionSession session);

    /// <summary>
    /// Retrieves an active session by ID.
    /// Returns null if the session is not found or has already been removed.
    /// </summary>
    ExecutionSession? Get(Guid sessionId);

    /// <summary>Removes a session from the store after it reaches a terminal state.</summary>
    void Remove(Guid sessionId);

    /// <summary>Returns all currently active (non-terminal) sessions.</summary>
    IReadOnlyList<ExecutionSession> GetActiveSessions();
}
