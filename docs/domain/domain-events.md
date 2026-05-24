# Domain Events

Domain events are immutable records emitted by aggregate roots when something meaningful happens inside the Domain Layer.

They serve as **notifications** for the Application layer (and possibly other consumers) to:
- update read models / projections
- trigger follow-up use-case logic
- update capacity tracking

They do **not** encode orchestration logic inside the Domain Layer.

Events are produced via `AggregateRoot<TId>.RaiseDomainEvent(...)` and collected in `AggregateRoot<TId>.DomainEvents`.

---

## Event: AgentStatusChangedEvent

**Emitted by:** `Agent` methods `Create`, `Activate`, `MarkBusy`, `MarkIdle`, `Suspend`, `Reactivate`, `Decommission`.

**Business meaning:** the agent operational status transitioned.

**Typical use in Application:**
- update agent status read model
- inform routing rules that depend on availability

---

## Event: AgentTaskAcceptedEvent

**Emitted by:** `Agent.NotifyTaskAccepted(taskId, newActiveTaskCount, maxConcurrentTasks)`.

**Business meaning:** the agent acknowledged a newly assigned task and incremented its active workload.

**Typical use in Application:**
- update capacity tracking for the agent
- optionally correlate tasks assigned to agent workload

---

## Event: AgentCapacityReachedEvent

**Emitted by:** `Agent.NotifyTaskAccepted(...)` when `newActiveTaskCount >= maxConcurrentTasks`.

**Business meaning:** the agent reached (or exceeded) its concurrency limit.

**Typical use in Application:**
- stop routing new tasks to the agent until capacity is restored

---

## Event: AgentTaskReleasedEvent

**Emitted by:** `Agent.NotifyTaskReleased(taskId, newActiveTaskCount, maxConcurrentTasks)`.

**Business meaning:** the agent released a task from active workload.

**Typical use in Application:**
- update capacity tracking

---

## Event: AgentCapacityRestoredEvent

**Emitted by:** `Agent.NotifyTaskReleased(...)` when `newActiveTaskCount < maxConcurrentTasks`.

**Business meaning:** the agent dropped below the concurrency limit and can accept additional work.

---

## Event: TaskCreatedEvent

**Emitted by:** `Task.Create(...)`.

**Business meaning:** a new task entered the domain as a `Pending` item.

---

## Event: TaskAssignedEvent

**Emitted by:** `Task.AssignTo(agentId, eligibility)`.

**Business meaning:** a task transitioned to `Assigned` and acquired an `AssignedAgentId`.

---

## Event: TaskCompletedEvent

**Emitted by:** `Task.Complete(result)`.

**Business meaning:** a task transitioned to `Completed`.

---

## Event: TaskFailedEvent

**Emitted by:** `Task.Fail(reason)`.

**Business meaning:** a task transitioned to `Failed` with a failure reason.

---

## Event: TaskCancelledEvent

**Emitted by:** `Task.Cancel()`.

**Business meaning:** a task transitioned to `Cancelled`.

---

## Event: MeetingStartedEvent

**Emitted by:** `Meeting.Start()`.

**Business meaning:** a meeting moved from `Scheduled` to `InProgress`.

---

## Event: MeetingConcludedEvent

**Emitted by:** `Meeting.Conclude(summary)`.

**Business meaning:** a meeting concluded; includes optional summary.

---

## Event: MessageSentEvent

**Emitted by:** `Meeting.PostMessage(message)`.

**Business meaning:** a participant posted a message in the meeting thread.

---

## Event: DecisionMadeEvent

**Emitted by:** `Decision.Finalize(outcome, finalizingAgentId, finalizingAgentRole)`.

**Business meaning:** a CEO finalized a decision, capturing verdict and reasoning.

---

## Event: BugReportedEvent

**Emitted by:** `BugReport.File(...)`.

**Business meaning:** QA filed a new bug report.

---

## Event: BugResolvedEvent

**Emitted by:** `BugReport.Resolve(resolvedByAgentId, resolutionNotes)`.

**Business meaning:** the bug report was resolved and closed (with mandatory resolution notes).

---

## Implementation Notes (Domain Event Design)

- All events are immutable records implementing `IDomainEvent`.
- Each event includes:
  - a unique `EventId`
  - a `OccurredOnUtc` timestamp
  - domain payload identifiers (typed IDs)
- Aggregates emit events as part of their mutation methods.
- The Application layer is responsible for dispatching events after persistence.

