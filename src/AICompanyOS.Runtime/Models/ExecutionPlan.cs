namespace AICompanyOS.Runtime.Models;

/// <summary>
/// Represents a structured sequence of execution steps produced by the planner.
///
/// A plan is the output of <see cref="AICompanyOS.Runtime.Abstractions.IExecutionPlanner"/>
/// and the input to <see cref="AICompanyOS.Runtime.Abstractions.IExecutionCoordinator"/>.
///
/// Plans are ephemeral — they exist only for the duration of a session.
/// They are NOT domain objects and carry no business invariants.
/// </summary>
public sealed class ExecutionPlan
{
    /// <summary>Unique identifier for this plan.</summary>
    public Guid PlanId { get; } = Guid.NewGuid();

    /// <summary>The session this plan belongs to.</summary>
    public Guid SessionId { get; init; }

    /// <summary>Human-readable description of the plan's goal.</summary>
    public string Goal { get; init; } = string.Empty;

    /// <summary>Ordered list of steps to execute.</summary>
    public IReadOnlyList<ExecutionStep> Steps { get; init; } = [];

    /// <summary>UTC timestamp when this plan was created by the planner.</summary>
    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;

    /// <summary>Whether all steps in this plan have reached a terminal state.</summary>
    public bool IsComplete =>
        Steps.All(s => s.Status is ExecutionStatus.Completed
                                or ExecutionStatus.Failed
                                or ExecutionStatus.Cancelled
                                or ExecutionStatus.TimedOut);

    /// <summary>Whether any step in this plan has failed.</summary>
    public bool HasFailures =>
        Steps.Any(s => s.Status is ExecutionStatus.Failed or ExecutionStatus.TimedOut);

    /// <summary>Returns the next pending step, or null if none remain.</summary>
    public ExecutionStep? NextPendingStep() =>
        Steps.FirstOrDefault(s => s.Status == ExecutionStatus.Pending);
}
