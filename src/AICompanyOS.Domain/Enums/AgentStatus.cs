namespace AICompanyOS.Domain.Enums;

/// <summary>
/// Represents the operational status of an AI agent.
/// </summary>
public enum AgentStatus
{
    /// <summary>Agent is registered but not yet active.</summary>
    Inactive = 0,

    /// <summary>Agent is available and ready to accept tasks.</summary>
    Idle = 1,

    /// <summary>Agent is currently executing a task.</summary>
    Busy = 2,

    /// <summary>Agent is temporarily suspended.</summary>
    Suspended = 3,

    /// <summary>Agent has been permanently decommissioned.</summary>
    Decommissioned = 4
}
