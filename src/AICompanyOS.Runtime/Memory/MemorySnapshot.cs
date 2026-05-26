namespace AICompanyOS.Runtime.Memory;

/// <summary>
/// An immutable point-in-time snapshot of an agent's runtime memory state.
///
/// Snapshots are produced at key checkpoints during execution (e.g., after
/// each plan step) and can be used for:
///   - Debugging and observability
///   - Resuming interrupted sessions (future)
///   - Feeding context into sub-agent spawning
///
/// Snapshots are read-only views — they do not modify the source memory.
/// </summary>
public sealed class MemorySnapshot
{
    /// <summary>The session this snapshot was taken from.</summary>
    public Guid SessionId { get; init; }

    /// <summary>UTC timestamp when the snapshot was taken.</summary>
    public DateTime TakenAtUtc { get; } = DateTime.UtcNow;

    /// <summary>Snapshot of working memory entries at the time of capture.</summary>
    public IReadOnlyDictionary<string, object?> WorkingMemoryEntries { get; init; } =
        new Dictionary<string, object?>();

    /// <summary>Snapshot of conversation turns at the time of capture.</summary>
    public IReadOnlyList<ConversationTurn> ConversationTurns { get; init; } = [];

    /// <summary>
    /// Creates a snapshot from a live <see cref="WorkingMemory"/> and
    /// <see cref="ConversationMemory"/> pair.
    /// </summary>
    public static MemorySnapshot Capture(
        Guid sessionId,
        WorkingMemory workingMemory,
        ConversationMemory conversationMemory) =>
        new()
        {
            SessionId = sessionId,
            WorkingMemoryEntries = workingMemory.Entries
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value),
            ConversationTurns = conversationMemory.Turns.ToList().AsReadOnly()
        };
}
