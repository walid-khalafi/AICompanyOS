namespace AICompanyOS.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a meeting between agents.
/// </summary>
public enum MeetingStatus
{
    /// <summary>Meeting has been scheduled but not yet started.</summary>
    Scheduled = 0,

    /// <summary>Meeting is currently in progress.</summary>
    InProgress = 1,

    /// <summary>Meeting has concluded successfully.</summary>
    Concluded = 2,

    /// <summary>Meeting was cancelled before it started.</summary>
    Cancelled = 3
}
