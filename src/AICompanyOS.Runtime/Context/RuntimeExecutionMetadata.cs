namespace AICompanyOS.Runtime.Context;

/// <summary>
/// Carries configuration and metadata for a single runtime execution.
///
/// Provides the runtime engine with the parameters it needs to execute
/// an agent session: timeouts, retry policy, model preferences, etc.
///
/// Ephemeral — constructed per execution, not persisted.
/// </summary>
public sealed class RuntimeExecutionMetadata
{
    /// <summary>Maximum wall-clock time allowed for the entire session.</summary>
    public TimeSpan SessionTimeout { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>Maximum number of planner/executor loop iterations before aborting.</summary>
    public int MaxIterations { get; init; } = 20;

    /// <summary>Maximum number of tool calls permitted per execution step.</summary>
    public int MaxToolCallsPerStep { get; init; } = 10;

    /// <summary>
    /// Preferred AI model identifier hint (e.g., "gpt-4o", "claude-3-5-sonnet").
    /// The runtime may override this based on agent role or system policy.
    /// Null means use the system default.
    /// </summary>
    public string? PreferredModelId { get; init; }

    /// <summary>
    /// Temperature hint for the LLM (0.0 = deterministic, 1.0 = creative).
    /// Null means use the model default.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>Whether the agent is permitted to call tools during this execution.</summary>
    public bool ToolCallingEnabled { get; init; } = true;

    /// <summary>Whether the agent should stream its output token-by-token.</summary>
    public bool StreamingEnabled { get; init; } = false;

    /// <summary>Arbitrary key-value metadata passed through to the execution engine.</summary>
    public IReadOnlyDictionary<string, string> Tags { get; init; } =
        new Dictionary<string, string>();
}
