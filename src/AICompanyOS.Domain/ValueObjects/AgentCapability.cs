namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Represents a discrete capability that an AI agent possesses.
///
/// Capabilities are more granular than roles. A Developer agent might have
/// capabilities like "write-code", "review-code", "refactor", while a QA agent
/// might have "write-tests", "run-tests", "file-bug-report".
///
/// This allows the Application layer to route tasks to agents based on what
/// they can actually do, not just their broad role.
///
/// Immutable — capabilities are defined at agent creation and do not change
/// during normal operation.
/// </summary>
public sealed record AgentCapability
{
    public const int MaxLength = 100;


    /// <summary>
    /// The capability identifier (e.g., "write-code", "run-tests", "draft-decision").
    /// Lowercase, hyphen-separated by convention.
    /// </summary>
    public string Value { get; }

    public AgentCapability(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Agent capability cannot be null or whitespace.", nameof(value));

        value = value.Trim().ToLowerInvariant();

        if (value.Length > MaxLength)
            throw new ArgumentException(
                $"Agent capability cannot exceed {MaxLength} characters.", nameof(value));

        Value = value;
    }

    // -------------------------------------------------------------------------
    // Well-known capabilities — use these constants to avoid magic strings
    // -------------------------------------------------------------------------

    // CEO capabilities
    public static readonly AgentCapability DraftDecision = new("draft-decision");
    public static readonly AgentCapability FinalizeDecision = new("finalize-decision");
    public static readonly AgentCapability ApprovePlan = new("approve-plan");

    // Developer capabilities
    public static readonly AgentCapability WriteCode = new("write-code");
    public static readonly AgentCapability ReviewCode = new("review-code");
    public static readonly AgentCapability Refactor = new("refactor");
    public static readonly AgentCapability FixBug = new("fix-bug");

    // QA capabilities
    public static readonly AgentCapability WriteTests = new("write-tests");
    public static readonly AgentCapability RunTests = new("run-tests");
    public static readonly AgentCapability FileBugReport = new("file-bug-report");

    // Research capabilities
    public static readonly AgentCapability Research = new("research");
    public static readonly AgentCapability Summarize = new("summarize");
    public static readonly AgentCapability Analyze = new("analyze");

    // Documentation capabilities
    public static readonly AgentCapability WriteDoc = new("write-doc");
    public static readonly AgentCapability ReviewDoc = new("review-doc");

    // DevOps capabilities
    public static readonly AgentCapability Deploy = new("deploy");
    public static readonly AgentCapability Monitor = new("monitor");

    public override string ToString() => Value;
}
