namespace AICompanyOS.Runtime.Models;

/// <summary>
/// Represents a live runtime execution session for one agent working on one task.
///
/// A session is the top-level execution container. It holds the plan,
/// tracks overall status, and accumulates the execution trace.
///
/// Sessions are ephemeral — they are scoped to a single task execution lifecycle.
/// They are NOT domain aggregates and do NOT enforce business invariants.
///
/// Relationship to Domain:
///   A session is initiated by the Application layer in response to a domain
///   command (e.g., Task assigned). When the session completes, the Application
///   layer issues the corresponding domain command (e.g., Task.Complete).
/// </summary>
public sealed class ExecutionSession
{
    /// <summary>Unique identifier for this session.</summary>
    public Guid SessionId { get; } = Guid.NewGuid();

    /// <summary>
    /// The domain Task ID this session is executing against.
    /// Stored as a raw Guid to avoid a Domain project reference.
    /// </summary>
    public Guid DomainTaskId { get; init; }

    /// <summary>
    /// The runtime agent identifier executing this session.
    /// Not a Domain AgentId — this is a runtime-scoped identity string.
    /// </summary>
    public string AgentRuntimeId { get; init; } = string.Empty;

    /// <summary>Correlation ID linking this session to the originating request.</summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>Current status of the session.</summary>
    public ExecutionStatus Status { get; private set; } = ExecutionStatus.Pending;

    /// <summary>The execution plan assigned to this session.</summary>
    public ExecutionPlan? Plan { get; private set; }

    /// <summary>Accumulated execution trace events for this session.</summary>
    public IReadOnlyList<RuntimeEvent> TraceEvents => _traceEvents.AsReadOnly();

    /// <summary>UTC timestamp when the session was created.</summary>
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when the session started running.</summary>
    public DateTime? StartedAtUtc { get; private set; }

    /// <summary>UTC timestamp when the session reached a terminal state.</summary>
    public DateTime? FinishedAtUtc { get; private set; }

    /// <summary>Final result of the session, populated on completion.</summary>
    public ExecutionResult? FinalResult { get; private set; }

    private readonly List<RuntimeEvent> _traceEvents = [];

    /// <summary>Assigns a plan to this session and transitions it to Running.</summary>
    public void Start(ExecutionPlan plan)
    {
        Plan         = plan;
        Status       = ExecutionStatus.Running;
        StartedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Records a runtime trace event into this session's log.</summary>
    public void RecordEvent(RuntimeEvent runtimeEvent) =>
        _traceEvents.Add(runtimeEvent);

    /// <summary>Marks the session as complete with a final result.</summary>
    public void Complete(ExecutionResult result)
    {
        FinalResult   = result;
        Status        = result.Status;
        FinishedAtUtc = DateTime.UtcNow;
    }
}
