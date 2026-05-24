using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Exceptions;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Entities;

/// <summary>
/// Entity — represents a discrete unit of work (job) within a workflow.
/// A WorkItem is a lower-level execution unit that can be part of a larger task.
/// It tracks execution state, retries, and output.
///
/// Uses <see cref="WorkItemStatus"/> (not <see cref="TaskStatus"/>) to avoid
/// coupling its state model to the higher-level Task lifecycle.
/// </summary>
public sealed class WorkItem : Entity<WorkItemId>
{
    public const int MaxRetries = 3;

    /// <summary>Name/label of this work item.</summary>
    public string Name { get; private set; }

    /// <summary>Input payload for the agent to process.</summary>
    public string? InputPayload { get; private set; }

    /// <summary>Output produced after execution.</summary>
    public string? OutputPayload { get; private set; }

    /// <summary>Current execution status.</summary>
    public WorkItemStatus Status { get; private set; }

    /// <summary>The task this work item belongs to.</summary>
    public TaskId ParentTaskId { get; private init; }

    /// <summary>The agent executing this work item.</summary>
    public AgentId? ExecutingAgentId { get; private set; }

    /// <summary>Number of execution attempts made so far.</summary>
    public int RetryCount { get; private set; }

    /// <summary>UTC timestamp when the most recent execution started.</summary>
    public DateTime? StartedOnUtc { get; private set; }

    /// <summary>UTC timestamp when execution finished (success or final failure).</summary>
    public DateTime? FinishedOnUtc { get; private set; }

    /// <summary>Error message from the most recent failed execution attempt.</summary>
    public string? ErrorMessage { get; private set; }

#pragma warning disable CS8618
    private WorkItem() { } // ORM — properties are set by the persistence layer
#pragma warning restore CS8618

    private WorkItem(WorkItemId id, string name, TaskId parentTaskId, string? inputPayload)
        : base(id)
    {
        Name         = name;
        ParentTaskId = parentTaskId;
        InputPayload = inputPayload;
        Status       = WorkItemStatus.Pending;
        RetryCount   = 0;
    }

    /// <summary>
    /// Factory method — creates a new pending work item.
    /// </summary>
    public static WorkItem Create(string name, TaskId parentTaskId, string? inputPayload = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("WorkItem name cannot be empty.", nameof(name));

        return new WorkItem(WorkItemId.New(), name.Trim(), parentTaskId, inputPayload);
    }

    // -------------------------------------------------------------------------
    // Behavior
    // -------------------------------------------------------------------------

    /// <summary>
    /// Starts execution of this work item by the given agent.
    /// Invariants:
    ///   - Cannot start an already-completed work item.
    ///   - Cannot start a work item that is already in progress.
    ///   - Cannot start if the retry limit has been reached.
    /// </summary>
    public void StartExecution(AgentId agentId)
    {
        if (Status == WorkItemStatus.Completed)
            throw new InvalidTaskOperationException(
                $"WorkItem '{Name}' is already completed and cannot be restarted.");

        if (Status == WorkItemStatus.InProgress)
            throw new InvalidTaskOperationException(
                $"WorkItem '{Name}' is already in progress.");

        if (Status == WorkItemStatus.Failed)
            throw new InvalidTaskOperationException(
                $"WorkItem '{Name}' has permanently failed (retry limit of {MaxRetries} reached).");

        if (RetryCount >= MaxRetries)
            throw new InvalidTaskOperationException(
                $"WorkItem '{Name}' has exceeded the maximum retry limit of {MaxRetries}.");

        ExecutingAgentId = agentId;
        Status           = WorkItemStatus.InProgress;
        StartedOnUtc     = DateTime.UtcNow;
        RetryCount++;
    }

    /// <summary>
    /// Records a successful execution result and transitions the work item to Completed.
    /// Invariant: work item must be InProgress.
    /// </summary>
    public void RecordSuccess(string? outputPayload = null)
    {
        if (Status != WorkItemStatus.InProgress)
            throw new InvalidTaskOperationException(
                $"WorkItem '{Name}' must be InProgress to record success. Current: '{Status}'.");

        Status        = WorkItemStatus.Completed;
        OutputPayload = outputPayload?.Trim();
        FinishedOnUtc = DateTime.UtcNow;
        ErrorMessage  = null;
    }

    /// <summary>
    /// Records a failed execution attempt.
    /// If retries remain, status returns to <see cref="WorkItemStatus.Pending"/> for retry.
    /// If all retries are exhausted, status is set to <see cref="WorkItemStatus.Failed"/>.
    /// Invariant: work item must be InProgress.
    /// </summary>
    public void RecordFailure(string errorMessage)
    {
        if (Status != WorkItemStatus.InProgress)
            throw new InvalidTaskOperationException(
                $"WorkItem '{Name}' must be InProgress to record failure. Current: '{Status}'.");

        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("An error message must be provided when recording failure.", nameof(errorMessage));

        ErrorMessage  = errorMessage.Trim();
        FinishedOnUtc = DateTime.UtcNow;

        Status = RetryCount < MaxRetries
            ? WorkItemStatus.Pending   // eligible for retry
            : WorkItemStatus.Failed;   // all retries exhausted
    }

    // -------------------------------------------------------------------------
    // Queries
    // -------------------------------------------------------------------------

    /// <summary>Returns true if this work item has reached a terminal state (Completed or Failed).</summary>
    public bool IsTerminal() =>
        Status is WorkItemStatus.Completed or WorkItemStatus.Failed;

    /// <summary>Returns true if another execution attempt is permitted.</summary>
    public bool CanRetry() =>
        Status == WorkItemStatus.Pending && RetryCount < MaxRetries;
}
