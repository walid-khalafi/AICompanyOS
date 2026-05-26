namespace AICompanyOS.Runtime.Memory;

/// <summary>
/// A single key-value entry in working memory.
/// Entries are typed by a string key and carry an arbitrary object value.
/// </summary>
public sealed class MemoryEntry
{
    /// <summary>The key identifying this memory entry.</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>The stored value. May be any serializable object.</summary>
    public object? Value { get; init; }

    /// <summary>UTC timestamp when this entry was written.</summary>
    public DateTime WrittenAtUtc { get; } = DateTime.UtcNow;

    /// <summary>Optional tag for categorizing entries (e.g., "tool-result", "plan-output").</summary>
    public string? Tag { get; init; }
}
