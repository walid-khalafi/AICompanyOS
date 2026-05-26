namespace AICompanyOS.Runtime.Models;

/// <summary>
/// Represents a single discrete step within an execution plan.
///
/// A step is the atomic unit of AI work: one agent, one instruction,
/// one result. Steps are sequenced inside an <see cref="ExecutionPlan"/>
/// and executed within an <see cref="ExecutionSession"/>.
///
/// Ephemeral — not persisted as domain state.
/// </summary>
public sealed class ExecutionStep
{
    /// <summary>Unique identifier for this step.</summary>
    public Guid StepId { get; } = Guid.NewGuid();

    /// <summary>Ordinal position within the parent plan (0-based).</summary>
    public int Ordinal { get; init; }

    /// <summary>
    /// The runtime agent identifier assigned to execute this step.
    /// This is a runtime identity string, not a Domain AgentId.
    /// </summary>
    public string AgentRuntimeId { get; init; } = string.Empty;

    /// <summary>The instruction or prompt to be executed by the agent.</summary>
    public string Instruction { get; init; } = string.Empty;

    /// <summary>Current execution status of this step.</summary>
    public ExecutionStatus Status { get; private set; } = ExecutionStatus.Pending;

    /// <summary>Result produced after this step completes.</summary>
    public ExecutionResult? Result { get; private set; }

    /// <summary>UTC timestamp when this step started executing.</summary>
    public DateTime? StartedAtUtc { get; private set; }

    /// <summary>UTC timestamp when this step finished.</summary>
    public DateTime? FinishedAtUtc { get; private set; }

    /// <summary>Optional tool calls requested during this step.</summary>
    public IReadOnlyList<ToolCallRequest> ToolCallRequests { get; private set; } = [];

    /// <summary>Marks the step as running.</summary>
    public void MarkRunning()
    {
        Status       = ExecutionStatus.Running;
        StartedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Records the result and marks the step terminal.</summary>
    public void RecordResult(ExecutionResult result)
    {
        Result        = result;
        Status        = result.Status;
        FinishedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Attaches tool call requests raised during this step.</summary>
    public void AttachToolCalls(IReadOnlyList<ToolCallRequest> toolCalls)
    {
        ToolCallRequests = toolCalls;
        if (toolCalls.Count > 0)
            Status = ExecutionStatus.Waiting;
    }
}
