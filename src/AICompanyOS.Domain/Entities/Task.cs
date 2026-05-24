using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Events;
using AICompanyOS.Domain.Exceptions;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

// Alias to avoid collision with System.Threading.Tasks.Task
using DomainTaskStatus = AICompanyOS.Domain.Enums.TaskStatus;

namespace AICompanyOS.Domain.Entities;

/// <summary>
/// Aggregate Root — the single canonical representation of a unit of work
/// in the AICompanyOS system.
///
/// Lifecycle:  Pending → Assigned → InProgress → [UnderReview] → Completed
///                                              ↘ Blocked → (back to Assigned)
///                                              ↘ Failed
///                                              ↘ Cancelled
///
/// Invariants owned exclusively by this aggregate:
///   - A task can only be assigned to one agent at a time.
///   - Assignment requires a valid AgentEligibility (produced by Agent.GetTaskAcceptanceEligibility()).
///   - Terminal tasks (Completed / Failed / Cancelled) are immutable.
///   - Only Pending or Blocked tasks can be assigned.
///
/// Rule ownership:
///   - Task lifecycle rules          → this aggregate (single source of truth)
///   - Agent eligibility evaluation  → Agent.GetTaskAcceptanceEligibility()
///   - Cross-aggregate coordination  → Application layer use-cases
/// </summary>
public sealed class Task : AggregateRoot<TaskId>
{
    /// <summary>Short descriptive title of the task.</summary>
    public TaskTitle Title { get; private set; }

    /// <summary>Detailed description of what needs to be done.</summary>
    public string? Description { get; private set; }

    /// <summary>Current lifecycle status.</summary>
    public DomainTaskStatus Status { get; private set; }

    /// <summary>Priority level influencing execution order.</summary>
    public Priority Priority { get; private set; }

    /// <summary>The agent currently assigned to this task, if any.</summary>
    public AgentId? AssignedAgentId { get; private set; }

    /// <summary>The agent who created / requested this task.</summary>
    public AgentId CreatedByAgentId { get; private init; }

    /// <summary>UTC timestamp when the task was created.</summary>
    public DateTime CreatedOnUtc { get; private init; }

    /// <summary>UTC timestamp when the task was last updated.</summary>
    public DateTime? LastUpdatedOnUtc { get; private set; }

    /// <summary>UTC timestamp when the task reached a terminal state.</summary>
    public DateTime? FinishedOnUtc { get; private set; }

    /// <summary>Output produced by the agent upon successful completion.</summary>
    public string? Result { get; private set; }

    /// <summary>Reason for failure, populated only when Status is Failed.</summary>
    public string? FailureReason { get; private set; }

#pragma warning disable CS8618
    private Task() { } // ORM — properties hydrated by the persistence layer
#pragma warning restore CS8618

    private Task(TaskId id, TaskTitle title, string? description, Priority priority, AgentId createdBy)
        : base(id)
    {
        Title            = title;
        Description      = description;
        Priority         = priority;
        Status           = DomainTaskStatus.Pending;
        CreatedByAgentId = createdBy;
        CreatedOnUtc     = DateTime.UtcNow;
    }

    // -------------------------------------------------------------------------
    // Factory
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a new Pending task and raises <see cref="TaskCreatedEvent"/>.
    /// </summary>
    public static Task Create(
        TaskTitle title,
        AgentId createdBy,
        Priority priority = Priority.Normal,
        string? description = null)
    {
        var task = new Task(TaskId.New(), title, description, priority, createdBy);
        task.RaiseDomainEvent(TaskCreatedEvent.Create(task.Id, title.Value, priority));
        return task;
    }

    // -------------------------------------------------------------------------
    // Behavior — lifecycle transitions
    // -------------------------------------------------------------------------

    /// <summary>
    /// Assigns this task to an agent.
    ///
    /// The caller (Application layer) must first call
    /// <c>agent.GetTaskAcceptanceEligibility()</c> and pass the result here.
    /// This keeps eligibility evaluation on the Agent aggregate (single source
    /// of truth) while the Task aggregate enforces its own assignment invariants.
    ///
    /// Invariants:
    ///   - Task must be Pending or Blocked.
    ///   - Task must not already have an active assignment.
    ///   - Agent must be eligible (CanAcceptWork == true).
    /// </summary>
    public void AssignTo(AgentId agentId, AgentEligibility eligibility)
    {
        EnsureNotTerminal();

        if (AssignedAgentId is not null)
            throw new InvalidTaskOperationException(
                $"Task '{Title}' is already assigned to agent '{AssignedAgentId}'. " +
                "Unassign it first before reassigning.");

        if (Status != DomainTaskStatus.Pending && Status != DomainTaskStatus.Blocked)
            throw new InvalidTaskOperationException(
                $"Task '{Title}' cannot be assigned from status '{Status}'. " +
                "Only Pending or Blocked tasks can be assigned.");

        if (!eligibility.CanAcceptWork)
            throw new InvalidTaskOperationException(
                $"Agent '{agentId}' is not eligible to accept this task: {eligibility.Reason}");

        AssignedAgentId  = agentId;
        Status           = DomainTaskStatus.Assigned;
        LastUpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(TaskAssignedEvent.Create(Id, agentId));
    }

    /// <summary>
    /// Transitions the task to InProgress.
    /// Invariant: task must be in Assigned status.
    /// </summary>
    public void Start()
    {
        EnsureNotTerminal();

        if (Status != DomainTaskStatus.Assigned)
            throw new InvalidTaskOperationException(
                $"Task '{Title}' must be Assigned before it can start. Current: '{Status}'.");

        Status           = DomainTaskStatus.InProgress;
        LastUpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the task as successfully completed with an optional result payload.
    /// Invariant: task must be InProgress or UnderReview.
    /// </summary>
    public void Complete(string? result = null)
    {
        EnsureNotTerminal();

        if (Status != DomainTaskStatus.InProgress && Status != DomainTaskStatus.UnderReview)
            throw new InvalidTaskOperationException(
                $"Task '{Title}' must be InProgress or UnderReview to complete. Current: '{Status}'.");

        Status           = DomainTaskStatus.Completed;
        Result           = result?.Trim();
        FinishedOnUtc    = DateTime.UtcNow;
        LastUpdatedOnUtc = DateTime.UtcNow;

        var completedBy = AssignedAgentId ?? CreatedByAgentId;
        RaiseDomainEvent(TaskCompletedEvent.Create(Id, completedBy));
    }

    /// <summary>
    /// Marks the task as failed with a mandatory reason.
    /// Invariant: task must not already be in a terminal state.
    /// </summary>
    public void Fail(string reason)
    {
        EnsureNotTerminal();

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A failure reason must be provided.", nameof(reason));

        Status           = DomainTaskStatus.Failed;
        FailureReason    = reason.Trim();
        FinishedOnUtc    = DateTime.UtcNow;
        LastUpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(TaskFailedEvent.Create(Id, AssignedAgentId, reason));
    }

    /// <summary>
    /// Blocks the task — it cannot proceed without external input.
    /// Invariant: task must be InProgress or Assigned.
    /// </summary>
    public void Block()
    {
        EnsureNotTerminal();

        if (Status != DomainTaskStatus.InProgress && Status != DomainTaskStatus.Assigned)
            throw new InvalidTaskOperationException(
                $"Task '{Title}' can only be blocked from InProgress or Assigned. Current: '{Status}'.");

        Status           = DomainTaskStatus.Blocked;
        LastUpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Submits the task for QA review before final completion.
    /// Invariant: task must be InProgress.
    /// </summary>
    public void SubmitForReview()
    {
        EnsureNotTerminal();

        if (Status != DomainTaskStatus.InProgress)
            throw new InvalidTaskOperationException(
                $"Task '{Title}' must be InProgress to submit for review. Current: '{Status}'.");

        Status           = DomainTaskStatus.UnderReview;
        LastUpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the task.
    /// Invariants:
    ///   - Terminal tasks (Completed / Failed) cannot be cancelled.
    ///   - Already-cancelled is idempotent.
    /// </summary>
    public void Cancel()
    {
        // Reuse the shared terminal guard — this correctly blocks
        // cancellation of both Completed AND Failed tasks.
        EnsureNotTerminal();

        if (Status == DomainTaskStatus.Cancelled)
            return; // idempotent

        var cancelledBy = AssignedAgentId ?? CreatedByAgentId;

        Status           = DomainTaskStatus.Cancelled;
        FinishedOnUtc    = DateTime.UtcNow;
        LastUpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(TaskCancelledEvent.Create(Id, cancelledBy));
    }

    /// <summary>
    /// Removes the current agent assignment, returning the task to Pending.
    /// Invariant: terminal tasks cannot be unassigned.
    /// </summary>
    public void Unassign()
    {
        EnsureNotTerminal();

        if (AssignedAgentId is null)
            return; // idempotent

        AssignedAgentId  = null;
        Status           = DomainTaskStatus.Pending;
        LastUpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the task priority.
    /// Invariant: terminal tasks are immutable.
    /// </summary>
    public void UpdatePriority(Priority priority)
    {
        EnsureNotTerminal();
        Priority         = priority;
        LastUpdatedOnUtc = DateTime.UtcNow;
    }

    // -------------------------------------------------------------------------
    // Queries
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns true if the task has reached a terminal state.
    /// Terminal tasks are immutable — no further transitions are allowed.
    /// </summary>
    public bool IsTerminal() =>
        Status is DomainTaskStatus.Completed
               or DomainTaskStatus.Failed
               or DomainTaskStatus.Cancelled;

    // -------------------------------------------------------------------------
    // Guards
    // -------------------------------------------------------------------------

    private void EnsureNotTerminal()
    {
        if (IsTerminal())
            throw new InvalidTaskOperationException(
                $"Task '{Title}' is in terminal state '{Status}' and cannot be modified.");
    }
}
