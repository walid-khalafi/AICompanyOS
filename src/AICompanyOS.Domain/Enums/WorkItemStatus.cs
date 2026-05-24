namespace AICompanyOS.Domain.Enums;

/// <summary>
/// Represents the execution status of a WorkItem.
///
/// WorkItem is a lower-level execution unit within a Task. Its lifecycle
/// is simpler than a Task's and intentionally kept separate to avoid
/// coupling WorkItem state to Task lifecycle semantics.
///
/// Lifecycle:
///   Pending → InProgress → Completed
///           ↘             ↘ Pending (if retries remain)
///                         ↘ Failed  (if retries exhausted)
/// </summary>
public enum WorkItemStatus
{
    /// <summary>Work item is waiting to be executed (including retry-eligible items).</summary>
    Pending = 0,

    /// <summary>Work item is currently being executed by an agent.</summary>
    InProgress = 1,

    /// <summary>Work item completed successfully.</summary>
    Completed = 2,

    /// <summary>Work item failed and all retry attempts have been exhausted.</summary>
    Failed = 3
}
