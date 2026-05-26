namespace AICompanyOS.Runtime.Memory;

/// <summary>
/// Short-term, mutable key-value store scoped to a single execution session.
///
/// Working memory holds intermediate results, tool outputs, and scratchpad
/// data that the agent accumulates during execution. It is discarded when
/// the session ends.
///
/// Not persisted. Not shared across sessions.
/// </summary>
public sealed class WorkingMemory
{
    private readonly Dictionary<string, MemoryEntry> _entries = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>All current entries in working memory.</summary>
    public IReadOnlyDictionary<string, MemoryEntry> Entries => _entries;

    /// <summary>Writes a value into working memory under the given key.</summary>
    public void Write(string key, object? value, string? tag = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Memory key cannot be null or whitespace.", nameof(key));

        _entries[key] = new MemoryEntry { Key = key, Value = value, Tag = tag };
    }

    /// <summary>Reads a value from working memory. Returns null if the key does not exist.</summary>
    public object? Read(string key) =>
        _entries.TryGetValue(key, out var entry) ? entry.Value : null;

    /// <summary>Reads a typed value from working memory.</summary>
    public T? Read<T>(string key) =>
        _entries.TryGetValue(key, out var entry) && entry.Value is T typed ? typed : default;

    /// <summary>Returns true if the key exists in working memory.</summary>
    public bool Contains(string key) => _entries.ContainsKey(key);

    /// <summary>Removes a key from working memory.</summary>
    public void Remove(string key) => _entries.Remove(key);

    /// <summary>Clears all entries from working memory.</summary>
    public void Clear() => _entries.Clear();

    /// <summary>Returns all entries with a specific tag.</summary>
    public IEnumerable<MemoryEntry> GetByTag(string tag) =>
        _entries.Values.Where(e => string.Equals(e.Tag, tag, StringComparison.OrdinalIgnoreCase));
}
