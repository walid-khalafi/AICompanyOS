namespace AICompanyOS.Runtime.Memory;

/// <summary>
/// Maintains the ordered conversation history for a single execution session.
///
/// This is the message list passed to the LLM on each planner/executor loop
/// iteration. It grows as the agent produces responses and tool results are
/// appended.
///
/// Ephemeral — scoped to one session. Not persisted between sessions.
/// Long-term memory persistence is a future concern (Phase 8+).
/// </summary>
public sealed class ConversationMemory
{
    private readonly List<ConversationTurn> _turns = [];

    /// <summary>All recorded conversation turns in chronological order.</summary>
    public IReadOnlyList<ConversationTurn> Turns => _turns.AsReadOnly();

    /// <summary>Total number of turns recorded.</summary>
    public int TurnCount => _turns.Count;

    /// <summary>Appends a system prompt to the conversation.</summary>
    public void AppendSystem(string content) =>
        Append(ConversationRole.System, content);

    /// <summary>Appends a user/orchestrator message to the conversation.</summary>
    public void AppendUser(string content) =>
        Append(ConversationRole.User, content);

    /// <summary>Appends an assistant (agent) response to the conversation.</summary>
    public void AppendAssistant(string content) =>
        Append(ConversationRole.Assistant, content);

    /// <summary>Appends a tool result to the conversation.</summary>
    public void AppendToolResult(Guid toolCallId, string resultContent) =>
        _turns.Add(new ConversationTurn
        {
            Role       = ConversationRole.Tool,
            Content    = resultContent,
            ToolCallId = toolCallId
        });

    /// <summary>Returns only the turns matching a specific role.</summary>
    public IEnumerable<ConversationTurn> GetByRole(ConversationRole role) =>
        _turns.Where(t => t.Role == role);

    /// <summary>Clears all turns. Used when resetting a session.</summary>
    public void Clear() => _turns.Clear();

    private void Append(ConversationRole role, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Conversation turn content cannot be empty.", nameof(content));

        _turns.Add(new ConversationTurn { Role = role, Content = content });
    }
}
