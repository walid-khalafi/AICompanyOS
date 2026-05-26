namespace AICompanyOS.Runtime.Observability;

/// <summary>
/// Categorizes the type of a runtime trace event.
/// Used for filtering, routing, and structured log analysis.
/// </summary>
public enum RuntimeEventType
{
    // Session lifecycle
    SessionStarted    = 100,
    SessionCompleted  = 101,
    SessionFailed     = 102,
    SessionCancelled  = 103,

    // Plan lifecycle
    PlanCreated       = 200,
    PlanStepStarted   = 201,
    PlanStepCompleted = 202,
    PlanStepFailed    = 203,

    // Tool execution
    ToolCallRequested = 300,
    ToolCallCompleted = 301,
    ToolCallFailed    = 302,

    // Memory
    MemoryWritten     = 400,
    MemorySnapshotted = 401,

    // Agent communication
    AgentMessageSent  = 500,

    // LLM interaction
    LlmRequestSent    = 600,
    LlmResponseReceived = 601,
    LlmTokensUsed     = 602,

    // General
    Information       = 900,
    Warning           = 901,
    Error             = 902
}
