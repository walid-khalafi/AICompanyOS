namespace AICompanyOS.Runtime.Memory;

/// <summary>
/// A lightweight reference to a memory artifact produced during execution.
///
/// Used when the full memory content is too large to pass inline —
/// instead, a reference is passed and the consumer resolves it on demand.
///
/// This is the runtime equivalent of a pointer into the memory store.
/// No persistence implementation exists yet — this is a contract only.
/// </summary>
public sealed class ExecutionMemoryReference
{
    /// <summary>Unique identifier for this memory reference.</summary>
    public Guid ReferenceId { get; } = Guid.NewGuid();

    /// <summary>The session that produced this memory artifact.</summary>
    public Guid SessionId { get; init; }

    /// <summary>
    /// The key under which the artifact is stored in working memory.
    /// </summary>
    public string MemoryKey { get; init; } = string.Empty;

    /// <summary>
    /// A human-readable description of what this reference points to.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>UTC timestamp when this reference was created.</summary>
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;
}
