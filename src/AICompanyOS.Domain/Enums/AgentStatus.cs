namespace AICompanyOS.Domain.Enums;

/// <summary>
/// Represents the operational status of an AI agent.
///
/// Valid transitions:
///   Inactive → Idle        (via Agent.Activate())
///   Idle     → Busy        (via Agent.MarkBusy())
///   Idle     → Suspended   (via Agent.Suspend())
///   Busy     → Idle        (via Agent.MarkIdle())
///   Busy     → Suspended   (via Agent.Suspend())
///   Suspended→ Idle        (via Agent.Reactivate())
///   Any      → Decommissioned (via Agent.Decommission()) — irreversible
/// </summary>
public enum AgentStatus
{
    /// <summary>Agent is registered but has not yet been activated.</summary>
    Inactive = 0,

    /// <summary>Agent is available and ready to accept tasks.</summary>
    Idle = 1,

    /// <summary>Agent is currently executing a task.</summary>
    Busy = 2,

    /// <summary>Agent is temporarily suspended and cannot accept new work.</summary>
    Suspended = 3,

    /// <summary>Agent has been permanently decommissioned. This state is irreversible.</summary>
    Decommissioned = 4
}
