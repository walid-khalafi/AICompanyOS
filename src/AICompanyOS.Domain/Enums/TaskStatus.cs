namespace AICompanyOS.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a task within the system.
/// </summary>
public enum TaskStatus
{
    /// <summary>Task has been created but not yet assigned or started.</summary>
    Pending = 0,

    /// <summary>Task has been assigned to an agent and is awaiting execution.</summary>
    Assigned = 1,

    /// <summary>Task is actively being worked on by an agent.</summary>
    InProgress = 2,

    /// <summary>Task is blocked and cannot proceed without external input.</summary>
    Blocked = 3,

    /// <summary>Task has been submitted for review.</summary>
    UnderReview = 4,

    /// <summary>Task has been successfully completed.</summary>
    Completed = 5,

    /// <summary>Task execution failed.</summary>
    Failed = 6,

    /// <summary>Task was cancelled before completion.</summary>
    Cancelled = 7
}
