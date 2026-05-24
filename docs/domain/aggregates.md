# Aggregates

This document describes each **aggregate root** in the Domain Layer and the domain entities contained within those aggregate boundaries.

In this codebase, the aggregate roots are the **entry points** to consistency boundaries. External code (Application/use-cases) interacts with aggregates via methods that:
- enforce state transitions and invariants locally
- raise domain events for meaningful changes
- avoid coordinating with other aggregates directly

---

## Aggregate: Agent

**Type:** `AICompanyOS.Domain.Entities.Agent`  
**ID:** `AgentId`  
**Role:** system actor that enforces operational status and eligibility for work.

### Responsibilities
- Own and validate **agent lifecycle** state transitions.
- Provide **role-based queries** (`IsCeo`, `IsQa`, etc.).
- Provide an **eligibility value object** (`AgentEligibility`) used by other aggregates (notably `Task`).
- Emit **capacity/workload domain events** so Application can update routing/capacity without reaching into agent internals.

### Invariants & State Machine
State machine is enforced by methods:

- **Inactive → Idle** via `Activate()`
- **Idle → Busy** via `MarkBusy()`
- **Idle → Suspended** via `Suspend()`
- **Busy → Idle** via `MarkIdle()`
- **Busy → Suspended** via `Suspend()`
- **Suspended → Idle** via `Reactivate()`
- **Any → Decommissioned** via `Decommission()` (irreversible)

Guards (examples):
- `Activate()` only allowed when `Status == Inactive`.
- `MarkBusy()` rejects `Inactive`, `Suspended`, `Decommissioned`.
- `MarkIdle()` rejects `Inactive` and `Decommissioned`.
- `Reactivate()` only allowed when `Status == Suspended`.

### Lifecycle / Methods
- `Create(AgentName, AgentRole, description?)`
  - Initializes `Status = Inactive`.
  - Raises `AgentStatusChangedEvent` for the initial transition.
- `Activate()`
  - Sets `Status = Idle` and raises `AgentStatusChangedEvent`.
- `MarkBusy()`
  - Sets `Status = Busy` and raises `AgentStatusChangedEvent`.
- `MarkIdle()`
  - Sets `Status = Idle` and raises `AgentStatusChangedEvent`.
- `Suspend()`
  - Sets `Status = Suspended` and raises `AgentStatusChangedEvent`.
- `Reactivate()`
  - Sets `Status = Idle` and raises `AgentStatusChangedEvent`.
- `Decommission()`
  - Sets `Status = Decommissioned` and raises `AgentStatusChangedEvent`.
- `NotifyTaskAccepted(TaskId taskId, int newActiveTaskCount, int maxConcurrentTasks)`
  - Raises `AgentTaskAcceptedEvent`.
  - If capacity reached, also raises `AgentCapacityReachedEvent`.
  - Disallows notifications while `Inactive`, `Suspended`, or `Decommissioned`.
- `NotifyTaskReleased(TaskId taskId, int newActiveTaskCount, int maxConcurrentTasks)`
  - Raises `AgentTaskReleasedEvent`.
  - If capacity restored, also raises `AgentCapacityRestoredEvent`.
- Queries: `IsCeo/IsQa/IsDeveloper/IsAvailable/IsActive`.
- Eligibility: `GetTaskAcceptanceEligibility()`
  - Returns `AgentEligibility.Acceptable()` only when `Status == Idle`.
  - Otherwise returns `NotAcceptable(reason)`.

### Important Domain Events
Emitted by Agent:
- `AgentStatusChangedEvent`
- `AgentTaskAcceptedEvent`
- `AgentTaskReleasedEvent`
- `AgentCapacityReachedEvent`
- `AgentCapacityRestoredEvent`

---

## Aggregate: Task (Canonical Work Unit)

**Type:** `AICompanyOS.Domain.Entities.Task`  
**ID:** `TaskId`  
**Role:** the single canonical representation of a unit of work within the system.

### Responsibilities
- Enforce the **task lifecycle** state machine.
- Enforce the **assignment invariants**:
  - one active assignment at a time
  - assignment only from specific states
  - assignment requires valid `AgentEligibility`
- Enforce **terminal immutability** for terminal states.
- Raise domain events whenever the lifecycle changes meaningfully.

### Invariants (owned exclusively by Task)
- **Assignment exclusivity:** `AssignedAgentId` can only be set once unless explicitly cleared via `Unassign()`.
- **Allowed assignment states:** assignment is only allowed when status is `Pending` or `Blocked`.
- **Eligibility required:** `AssignTo(...)` requires a `AgentEligibility` produced by `Agent.GetTaskAcceptanceEligibility()`.
- **Terminal immutability:** `Completed`, `Failed`, and `Cancelled` block further modifications.

### Lifecycle / State Transitions
Lifecycle is implemented via methods and status enum (`TaskStatus`).

- `Pending → Assigned` via `AssignTo(...)` + emits `TaskAssignedEvent`
- `Assigned → InProgress` via `Start()`
- `InProgress → UnderReview` via `SubmitForReview()`
- `InProgress/UnderReview → Completed` via `Complete(result?)` + emits `TaskCompletedEvent`
- `InProgress → Failed` via `Fail(reason)` + emits `TaskFailedEvent`
- `Assigned/InProgress → Blocked` via `Block()`
- `InProgress/Assigned → Cancelled` via `Cancel()` + emits `TaskCancelledEvent`
- `Pending/Blocked` can be re-assigned only by first clearing assignment with `Unassign()` (guarded by terminal checks).

### Lifecycle Methods (important)
- `Create(TaskTitle title, AgentId createdBy, Priority priority, description?)`
  - Initializes status to `Pending`.
  - Raises `TaskCreatedEvent`.
- `AssignTo(AgentId agentId, AgentEligibility eligibility)`
  - Validates terminal status and allowed statuses.
  - Requires `eligibility.CanAcceptWork`.
  - Sets `AssignedAgentId`, transitions to `Assigned`, raises `TaskAssignedEvent`.
- `Start()`
  - Requires `Status == Assigned`.
  - Sets `Status = InProgress`.
- `SubmitForReview()`
  - Requires `Status == InProgress`.
  - Sets `Status = UnderReview`.
- `Complete(string? result)`
  - Requires `Status == InProgress` or `UnderReview`.
  - Sets `Status = Completed`, captures result, raises `TaskCompletedEvent`.
- `Fail(string reason)`
  - Requires not-terminal.
  - Requires non-empty reason.
  - Sets `Status = Failed`, raises `TaskFailedEvent`.
- `Block()`
  - Requires `Status == Assigned` or `InProgress`.
  - Sets `Status = Blocked`.
- `Cancel()`
  - Terminal guard prevents cancellation of `Completed` or `Failed`.
  - Idempotent for already-cancelled.
  - Sets `Status = Cancelled` and raises `TaskCancelledEvent`.
- `Unassign()`
  - Removes `AssignedAgentId` and returns to `Pending`.
  - Idempotent if no assignment exists.
- `UpdatePriority(Priority priority)`
  - Allowed only for non-terminal tasks.

### Important Domain Events
- `TaskCreatedEvent`
- `TaskAssignedEvent`
- `TaskCompletedEvent`
- `TaskFailedEvent`
- `TaskCancelledEvent`

---

## Aggregate: Meeting

**Type:** `AICompanyOS.Domain.Entities.Meeting`  
**ID:** `MeetingId`  
**Role:** collaboration boundary for conversations between agents.

### Responsibilities
- Enforce meeting lifecycle (Scheduled → InProgress → Concluded/Cancelled).
- Enforce minimum participation rule.
- Own the meeting’s message thread (via contained `Message` entities).

### Invariants & Rules
- Meeting creation enforces **>= 2 distinct participants**.
- Organizer is ensured to be part of participant list.
- Participant list is deduplicated.
- `Start()` only allowed when `Status == Scheduled`.
- `Conclude()` only allowed when `Status == InProgress`.
- Cancelled cannot be concluded after cancellation (method currently only blocks cancellation of concluded; guard behavior is implementation-specific but enforced by status checks).
- Messages can only be posted when meeting is `InProgress`.
- Sender of a posted message must be a registered participant.

### Lifecycle / Methods
- `Schedule(topic, organizer, participants)`
  - Creates meeting in `Scheduled` state.
- `Start()`
  - Transitions to `InProgress` and raises `MeetingStartedEvent`.
- `Conclude(string? summary)`
  - Transitions to `Concluded` and raises `MeetingConcludedEvent`.
- `Cancel()`
  - Transitions to `Cancelled` unless already concluded.
- `AddParticipant(AgentId agentId)`
  - Allowed only while `Status == Scheduled`.
- `PostMessage(Message message)`
  - Allowed only while `Status == InProgress`.
  - Validates sender participation.
  - Adds message and raises `MessageSentEvent`.

### Important Domain Events
- `MeetingStartedEvent`
- `MeetingConcludedEvent`
- `MessageSentEvent`

---

## Aggregate: Decision

**Type:** `AICompanyOS.Domain.Entities.Decision`  
**ID:** `DecisionId`  
**Role:** represents a formal CEO decision with a structured outcome.

### Responsibilities
- Enforce role-based authorization for drafting/finalizing decisions.
- Enforce immutability once finalized.

### Invariants
- Drafting is only permitted when `ceoAgentRole == AgentRole.CEO`.
- Finalization is only permitted when:
  - finalizing role is `AgentRole.CEO`
  - finalizing agent id matches the drafted `MadeByAgentId`
- Once `IsFinalized == true`, no further changes are allowed.

### Lifecycle / Methods
- `Draft(string subject, AgentId ceoAgentId, AgentRole ceoAgentRole, MeetingId? relatedMeetingId)`
  - Creates a decision with `IsFinalized = false`.
- `Finalize(DecisionOutcome outcome, AgentId finalizingAgentId, AgentRole finalizingAgentRole)`
  - Sets outcome, marks as finalized, sets finalized timestamp.
  - Raises `DecisionMadeEvent`.

### Important Domain Events
- `DecisionMadeEvent`

---

## Aggregate: BugReport

**Type:** `AICompanyOS.Domain.Entities.BugReport`  
**ID:** `BugReportId`  
**Role:** QA-filed defect report and resolution lifecycle.

### Responsibilities
- Enforce QA-only bug filing.
- Own resolution workflow state.
- Enforce immutability once resolved.

### Invariants
- `File(...)` requires `reportingAgentRole == AgentRole.QA`.
- `Resolve(...)` is idempotent; if already resolved, it returns.
- Resolution notes are mandatory (non-empty) to close a bug report.
- Resolved bugs cannot be reassigned, cannot be linked to new tasks, and cannot update severity.

### Lifecycle / Methods
- `File(title, description, severity, reportingAgentId, reportingAgentRole)`
  - Creates bug report in unresolved state.
  - Raises `BugReportedEvent`.
- `AssignTo(AgentId developerAgentId)`
  - Allowed only while unresolved.
- `LinkToTask(TaskId taskId)`
  - Allowed only while unresolved.
- `Resolve(AgentId resolvedByAgentId, string resolutionNotes)`
  - Marks resolved, stores notes and timestamp.
  - Raises `BugResolvedEvent`.
- `UpdateSeverity(Priority severity)`
  - Allowed only while unresolved.

### Important Domain Events
- `BugReportedEvent`
- `BugResolvedEvent`

---

## Domain Entities (Contained Types)

### WorkItem
- **Type:** `AICompanyOS.Domain.Entities.WorkItem` (Entity, not aggregate root)
- **Role:** lower-level execution unit within a task boundary.
- **Why it exists:** its lifecycle is deliberately simpler and uses `WorkItemStatus` to avoid coupling to the higher-level `TaskStatus` semantics.

Key rules:
- StartExecution is allowed only from safe statuses and while retries are within limits.
- RecordFailure returns to `Pending` if retry attempts remain; otherwise transitions to `Failed`.

### Message
- **Type:** `AICompanyOS.Domain.Entities.Message` (Entity)
- **Role:** immutable representation of communication between agents.
- **Belongs to:**
  - meeting messages (via `MeetingId`)
  - direct messages that may be related to a task (via `TaskId`)

Note: Domain events for message sending (`MessageSentEvent`) are raised by the owning aggregate when messages are posted.

