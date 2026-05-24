namespace AICompanyOS.Domain.Enums;

/// <summary>
/// Represents the priority level of a task or work item.
/// </summary>
public enum Priority
{
    /// <summary>Low priority — can be deferred.</summary>
    Low = 0,

    /// <summary>Normal priority — standard execution order.</summary>
    Normal = 1,

    /// <summary>High priority — should be addressed before normal items.</summary>
    High = 2,

    /// <summary>Critical priority — requires immediate attention.</summary>
    Critical = 3
}
