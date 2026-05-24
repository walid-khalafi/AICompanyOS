using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Events;
using AICompanyOS.Domain.Exceptions;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Entities;

/// <summary>
/// Aggregate Root — represents an AI agent operating within the company OS.
///
/// Responsibilities (pure domain):
///   - Protect its own state invariants (status transitions).
///   - Enforce role-based identity queries (IsCeo, IsQa, etc.).
///   - Raise domain events when meaningful state changes occur.
///   - Signal workload acceptance and release so the Application layer
///     can raise capacity events without reaching into Agent internals.
///
/// NOT responsible for:
///   - Tracking which tasks are assigned (that is a query/read-model concern).
///   - Coordinating with other aggregates (that belongs in Application layer use-cases).
///
/// Lifecycle:
///   Inactive → Idle        (Activate)
///   Idle     → Busy        (MarkBusy)
///   Idle     → Suspended   (Suspend)
///   Busy     → Idle        (MarkIdle)
///   Busy     → Suspended   (Suspend)
///   Suspended→ Idle        (Reactivate)
///   Any      → Decommissioned (Decommission) — irreversible
/// </summary>
public sealed class Agent : AggregateRoot<AgentId>
{
    /// <summary>Human-readable name of the agent (e.g., "CEO-Agent-01").</summary>
    public AgentName Name { get; private set; }

    /// <summary>The functional role this agent plays in the company.</summary>
    public AgentRole Role { get; private set; }

    /// <summary>Current operational status of the agent.</summary>
    public AgentStatus Status { get; private set; }

    /// <summary>UTC timestamp when this agent was registered in the system.</summary>
    public DateTime CreatedOnUtc { get; private init; }

    /// <summary>UTC timestamp of the last status change.</summary>
    public DateTime? LastStatusChangedOnUtc { get; private set; }

    /// <summary>Optional description of the agent's purpose or capabilities.</summary>
    public string? Description { get; private set; }

#pragma warning disable CS8618
    private Agent() { } // ORM — properties are set by the persistence layer
#pragma warning restore CS8618

    private Agent(AgentId id, AgentName name, AgentRole role, string? description)
        : base(id)
    {
        Name        = name;
        Role        = role;
        Status      = AgentStatus.Inactive;
        Description = description;
        CreatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method — the only way to create a valid Agent.
    /// Newly created agents start in <see cref="AgentStatus.Inactive"/> status
    /// and must be explicitly activated before they can accept work.
    /// </summary>
    public static Agent Create(AgentName name, AgentRole role, string? description = null)
    {
        var agent = new Agent(AgentId.New(), name, role, description);
        agent.RaiseDomainEvent(AgentStatusChangedEvent.Create(agent.Id, AgentStatus.Inactive, AgentStatus.Inactive));
        return agent;
    }

    // -------------------------------------------------------------------------
    // State transitions — each enforces its own invariants
    // -------------------------------------------------------------------------

    /// <summary>
    /// Activates the agent, making it available to accept tasks.
    /// Invariant: only Inactive agents can be activated.
    /// </summary>
    public void Activate()
    {
        if (Status == AgentStatus.Decommissioned)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is decommissioned and cannot be activated.");

        if (Status != AgentStatus.Inactive)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' cannot be activated from status '{Status}'. Only Inactive agents can be activated.");

        var previous = Status;
        Status = AgentStatus.Idle;
        LastStatusChangedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(AgentStatusChangedEvent.Create(Id, previous, AgentStatus.Idle));
    }

    /// <summary>
    /// Transitions the agent to Busy status.
    /// Called by the Application layer when a task execution begins.
    /// Invariant: only Idle agents can become Busy; decommissioned and suspended agents cannot.
    /// </summary>
    public void MarkBusy()
    {
        if (Status == AgentStatus.Decommissioned)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is decommissioned and cannot be made busy.");

        if (Status == AgentStatus.Suspended)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is suspended and cannot accept work.");

        if (Status == AgentStatus.Inactive)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is inactive and must be activated before accepting work.");

        if (Status == AgentStatus.Busy)
            return; // idempotent

        var previous = Status;
        Status = AgentStatus.Busy;
        LastStatusChangedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(AgentStatusChangedEvent.Create(Id, previous, AgentStatus.Busy));
    }

    /// <summary>
    /// Returns the agent to Idle status.
    /// Called by the Application layer when a task finishes or is unassigned.
    /// Invariant: decommissioned and inactive agents cannot return to Idle.
    /// </summary>
    public void MarkIdle()
    {
        if (Status == AgentStatus.Decommissioned)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is decommissioned and cannot be made idle.");

        if (Status == AgentStatus.Inactive)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is inactive. Call Activate() first.");

        if (Status == AgentStatus.Idle)
            return; // idempotent

        var previous = Status;
        Status = AgentStatus.Idle;
        LastStatusChangedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(AgentStatusChangedEvent.Create(Id, previous, AgentStatus.Idle));
    }

    /// <summary>
    /// Suspends the agent, preventing it from accepting new work.
    /// Invariant: decommissioned and inactive agents cannot be suspended.
    /// </summary>
    public void Suspend()
    {
        if (Status == AgentStatus.Decommissioned)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is already decommissioned.");

        if (Status == AgentStatus.Inactive)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is inactive and cannot be suspended.");

        if (Status == AgentStatus.Suspended)
            return; // idempotent

        var previous = Status;
        Status = AgentStatus.Suspended;
        LastStatusChangedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(AgentStatusChangedEvent.Create(Id, previous, AgentStatus.Suspended));
    }

    /// <summary>
    /// Reactivates a previously suspended agent, returning it to Idle.
    /// Invariant: only Suspended agents can be reactivated.
    /// </summary>
    public void Reactivate()
    {
        if (Status == AgentStatus.Decommissioned)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' is decommissioned and cannot be reactivated.");

        if (Status != AgentStatus.Suspended)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' cannot be reactivated from status '{Status}'. Only Suspended agents can be reactivated.");

        var previous = Status;
        Status = AgentStatus.Idle;
        LastStatusChangedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(AgentStatusChangedEvent.Create(Id, previous, AgentStatus.Idle));
    }

    /// <summary>
    /// Permanently decommissions the agent. This action is irreversible.
    /// </summary>
    public void Decommission()
    {
        if (Status == AgentStatus.Decommissioned)
            return; // idempotent

        var previous = Status;
        Status = AgentStatus.Decommissioned;
        LastStatusChangedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(AgentStatusChangedEvent.Create(Id, previous, AgentStatus.Decommissioned));
    }

    /// <summary>
    /// Signals that this agent has accepted a task into its active workload.
    /// Raises <see cref="AgentTaskAcceptedEvent"/> so the Application layer
    /// can update capacity tracking without accessing agent internals.
    ///
    /// Called by the Application layer immediately after task assignment is persisted.
    /// </summary>
    public void NotifyTaskAccepted(TaskId taskId, int newActiveTaskCount, int maxConcurrentTasks)
    {
        if (Status == AgentStatus.Decommissioned || Status == AgentStatus.Suspended || Status == AgentStatus.Inactive)
            throw new InvalidAgentOperationException(
                $"Agent '{Name}' (status: {Status}) cannot accept task notifications.");

        RaiseDomainEvent(AgentTaskAcceptedEvent.Create(Id, taskId, newActiveTaskCount, maxConcurrentTasks));

        if (newActiveTaskCount >= maxConcurrentTasks)
            RaiseDomainEvent(AgentCapacityReachedEvent.Create(Id, newActiveTaskCount, maxConcurrentTasks));
    }

    /// <summary>
    /// Signals that this agent has released a task from its active workload.
    /// Raises <see cref="AgentTaskReleasedEvent"/> and, if applicable,
    /// <see cref="AgentCapacityRestoredEvent"/> so the Application layer
    /// can resume routing tasks to this agent.
    ///
    /// Called by the Application layer after a task completes, fails, or is unassigned.
    /// </summary>
    public void NotifyTaskReleased(TaskId taskId, int newActiveTaskCount, int maxConcurrentTasks)
    {
        RaiseDomainEvent(AgentTaskReleasedEvent.Create(Id, taskId, newActiveTaskCount, maxConcurrentTasks));

        if (newActiveTaskCount < maxConcurrentTasks)
            RaiseDomainEvent(AgentCapacityRestoredEvent.Create(Id, newActiveTaskCount, maxConcurrentTasks));
    }

    // -------------------------------------------------------------------------
    // Role-based identity queries — pure boolean, no side effects
    // -------------------------------------------------------------------------

    /// <summary>Returns true if this agent holds the CEO role.</summary>
    public bool IsCeo() => Role == AgentRole.CEO;

    /// <summary>Returns true if this agent holds the QA role.</summary>
    public bool IsQa() => Role == AgentRole.QA;

    /// <summary>Returns true if this agent holds the Developer role.</summary>
    public bool IsDeveloper() => Role == AgentRole.Developer;

    /// <summary>Returns true if the agent is available to accept new work.</summary>
    public bool IsAvailable() => Status == AgentStatus.Idle;

    /// <summary>Returns true if the agent is active (not inactive, suspended, or decommissioned).</summary>
    public bool IsActive() =>
        Status != AgentStatus.Inactive &&
        Status != AgentStatus.Suspended &&
        Status != AgentStatus.Decommissioned;

    /// <summary>
    /// Encapsulates eligibility rules for accepting tasks.
    /// Domain callers can use this to obtain an <see cref="AgentEligibility"/> value
    /// without passing raw status primitives.
    /// </summary>
    public AgentEligibility GetTaskAcceptanceEligibility()
    {
        if (Status == AgentStatus.Decommissioned)
            return AgentEligibility.NotAcceptable("Agent is decommissioned.");

        if (Status == AgentStatus.Suspended)
            return AgentEligibility.NotAcceptable("Agent is suspended.");

        if (Status == AgentStatus.Inactive)
            return AgentEligibility.NotAcceptable("Agent is inactive and has not been activated yet.");

        if (Status != AgentStatus.Idle)
            return AgentEligibility.NotAcceptable($"Agent status is '{Status}', not Idle.");

        return AgentEligibility.Acceptable();
    }

    // -------------------------------------------------------------------------
    // Attribute updates
    // -------------------------------------------------------------------------

    /// <summary>Updates the agent's description. No invariants to protect here.</summary>
    public void UpdateDescription(string? description) =>
        Description = description?.Trim();
}
