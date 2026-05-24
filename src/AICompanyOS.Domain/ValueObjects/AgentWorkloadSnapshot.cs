namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// An immutable snapshot of an agent's current workload state.
///
/// This is a VALUE OBJECT — it captures a point-in-time view of how loaded
/// an agent is. It is produced by the Agent aggregate itself and consumed
/// by the Application layer to make routing and scheduling decisions.
///
/// It is NOT a live query — it reflects the state at the moment it was created.
/// The Application layer is responsible for deciding whether the snapshot is
/// fresh enough to act on.
/// </summary>
public sealed record AgentWorkloadSnapshot
{
    /// <summary>The agent this snapshot belongs to.</summary>
    public AgentId AgentId { get; }

    /// <summary>Number of tasks currently active on this agent.</summary>
    public int ActiveTaskCount { get; }

    /// <summary>Maximum number of tasks this agent can handle concurrently.</summary>
    public int MaxConcurrentTasks { get; }

    /// <summary>UTC timestamp when this snapshot was taken.</summary>
    public DateTime SnapshotTakenAtUtc { get; }

    public AgentWorkloadSnapshot(AgentId agentId, int activeTaskCount, int maxConcurrentTasks)
    {
        AgentId = agentId ?? throw new ArgumentNullException(nameof(agentId));

        if (activeTaskCount < 0)
            throw new ArgumentOutOfRangeException(nameof(activeTaskCount),
                "Active task count cannot be negative.");

        if (maxConcurrentTasks < 1)
            throw new ArgumentOutOfRangeException(nameof(maxConcurrentTasks),
                "Max concurrent tasks must be at least 1.");

        ActiveTaskCount = activeTaskCount;
        MaxConcurrentTasks = maxConcurrentTasks;
        SnapshotTakenAtUtc = DateTime.UtcNow;
    }

    /// <summary>True when the agent has no capacity left for new tasks.</summary>
    public bool IsAtCapacity => ActiveTaskCount >= MaxConcurrentTasks;

    /// <summary>True when the agent has room for at least one more task.</summary>
    public bool HasCapacity => !IsAtCapacity;

    /// <summary>Remaining task slots available.</summary>
    public int RemainingCapacity => Math.Max(0, MaxConcurrentTasks - ActiveTaskCount);

    /// <summary>Load ratio from 0.0 (empty) to 1.0 (full).</summary>
    public double LoadRatio => MaxConcurrentTasks == 0
        ? 1.0
        : Math.Min(1.0, (double)ActiveTaskCount / MaxConcurrentTasks);

    public override string ToString() =>
        $"Agent {AgentId}: {ActiveTaskCount}/{MaxConcurrentTasks} tasks ({LoadRatio:P0} load)";
}
