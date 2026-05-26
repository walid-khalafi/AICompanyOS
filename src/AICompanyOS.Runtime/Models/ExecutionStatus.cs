namespace AICompanyOS.Runtime.Models;

/// <summary>
/// Represents the lifecycle status of a runtime execution unit
/// (session, step, or plan).
///
/// These are ephemeral runtime states — they are NOT persisted as
/// domain state and do NOT map to any Domain aggregate status.
/// </summary>
public enum ExecutionStatus
{
    /// <summary>Execution unit has been created but not yet started.</summary>
    Pending = 0,

    /// <summary>Execution is actively running.</summary>
    Running = 1,

    /// <summary>Execution is temporarily paused, awaiting tool result or input.</summary>
    Waiting = 2,

    /// <summary>Execution completed successfully.</summary>
    Completed = 3,

    /// <summary>Execution failed due to an error or unrecoverable condition.</summary>
    Failed = 4,

    /// <summary>Execution was cancelled before completion.</summary>
    Cancelled = 5,

    /// <summary>Execution timed out.</summary>
    TimedOut = 6
}
