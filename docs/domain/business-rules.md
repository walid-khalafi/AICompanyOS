# Business Rules

This document enumerates the **business rules** enforced by the Domain Layer aggregates.

For each rule, the document states:
- **Rule description**
- **Where enforced** (aggregate + method)
- **What it protects** (invariant / authorization / lifecycle)

> All rules are enforced inside the Domain Layer and are not dependent on orchestration, persistence, or runtime AI behavior.

---

## 1. Agent Rules

### Rule A1 — Activation is allowed only from Inactive
- **Enforced by:** `Agent.Activate()`
- **Invariant:** `Status` must be `Inactive` before activation.
- **Protected behavior:** prevents invalid lifecycle transitions.

### Rule A2 — Busy requires an activated, non-suspended agent
- **Enforced by:** `Agent.MarkBusy()`
- **Invariant:**
  - `Status != Inactive`
  - `Status != Suspended`
  - `Status != Decommissioned`
- **Protected behavior:** prevents routing tasks to unavailable agents.

### Rule A3 — MarkIdle cannot be called for Inactive or Decommissioned agents
- **Enforced by:** `Agent.MarkIdle()`
- **Invariant:**
  - `Status != Inactive`
  - `Status != Decommissioned`
- **Protected behavior:** enforces lifecycle correctness.

### Rule A4 — Suspended cannot be entered from Inactive
- **Enforced by:** `Agent.Suspend()`
- **Invariant:** `Status != Inactive`.
- **Protected behavior:** keeps lifecycle consistent with activation-first model.

### Rule A5 — Reactivation requires Suspended
- **Enforced by:** `Agent.Reactivate()`
- **Invariant:** `Status == Suspended`.
- **Protected behavior:** prevents reactivating agents from invalid states.

### Rule A6 — Decommission is irreversible
- **Enforced by:** `Agent.Decommission()` and guards in other methods
- **Invariant:** once `Status == Decommissioned`, lifecycle transitions that allow work are rejected.
- **Protected behavior:** ensures terminal operational state.

### Rule A7 — Task acceptance eligibility is computed from status
- **Enforced by:** `Agent.GetTaskAcceptanceEligibility()`
- **Outputs:** `AgentEligibility`.
- **Invariant:** `AgentEligibility.Acceptable()` only when `Status == Idle`.

### Rule A8 — Capacity events are conditional on threshold crossings
- **Enforced by:**
  - `Agent.NotifyTaskAccepted(...)`
  - `Agent.NotifyTaskReleased(...)`
- **Rules:**
  - On accept: if `newActiveTaskCount >= maxConcurrentTasks`, raise `AgentCapacityReachedEvent`.
  - On release: if `newActiveTaskCount < maxConcurrentTasks`, raise `AgentCapacityRestoredEvent`.

---

## 2. Task Rules (Canonical Aggregate)

### Rule T1 — Task lifecycle terminal states are immutable
- **Enforced by:** `Task.EnsureNotTerminal()` (called from most mutating methods)
- **Terminal states:** `Completed`, `Failed`, `Cancelled`
- **Protected behavior:** prevents changing outcomes after completion.

### Rule T2 — Assignment exclusivity
- **Enforced by:** `Task.AssignTo(...)`
- **Invariant:** `AssignedAgentId` must be `null` when assigning.
- **Protected behavior:** a task can only be assigned to one agent at a time.

### Rule T3 — Assignment allowed only from Pending or Blocked
- **Enforced by:** `Task.AssignTo(...)`
- **Invariant:** `Status == Pending || Status == Blocked`.
- **Protected behavior:** prevents assignment from states that imply different progress.

### Rule T4 — Assignment requires valid agent eligibility
- **Enforced by:** `Task.AssignTo(...)`
- **Invariant:** `eligibility.CanAcceptWork == true`.
- **Protected behavior:** links assignment to agent availability policy.

### Rule T5 — Starting requires Assigned state
- **Enforced by:** `Task.Start()`
- **Invariant:** `Status == Assigned`.

### Rule T6 — Review submission requires InProgress
- **Enforced by:** `Task.SubmitForReview()`
- **Invariant:** `Status == InProgress`.

### Rule T7 — Completion requires InProgress or UnderReview
- **Enforced by:** `Task.Complete(...)`
- **Invariant:** `Status == InProgress || Status == UnderReview`.
- **Protected behavior:** ensures consistent workflow phases.

### Rule T8 — Failure requires non-terminal and valid reason
- **Enforced by:** `Task.Fail(reason)`
- **Invariants:**
  - not terminal
  - `reason` must be non-empty

### Rule T9 — Blocking allowed only from Assigned or InProgress
- **Enforced by:** `Task.Block()`
- **Invariant:** `Status == Assigned || Status == InProgress`.

### Rule T10 — Cancellation is blocked for Completed and Failed
- **Enforced by:** `Task.Cancel()` using `EnsureNotTerminal()`
- **Invariant:** `EnsureNotTerminal` blocks cancellation when terminal.
- **Protected behavior:** preserves terminal immutability.

### Rule T11 — Unassign returns to Pending
- **Enforced by:** `Task.Unassign()`
- **Invariants:**
  - not terminal
  - if `AssignedAgentId == null` → idempotent
  - otherwise sets `AssignedAgentId = null` and `Status = Pending`

### Rule T12 — Priority cannot be changed for terminal tasks
- **Enforced by:** `Task.UpdatePriority(Priority)`
- **Invariant:** `EnsureNotTerminal()`.

---

## 3. Meeting Rules

### Rule M1 — Meeting requires >= 2 distinct participants
- **Enforced by:** `Meeting.Schedule(...)`
- **Invariant:** distinct participant count `>= 2`.

### Rule M2 — Only Scheduled meetings can be started
- **Enforced by:** `Meeting.Start()`
- **Invariant:** `Status == Scheduled`.

### Rule M3 — Only InProgress meetings can be concluded
- **Enforced by:** `Meeting.Conclude(...)`
- **Invariant:** `Status == InProgress`.

### Rule M4 — Cancellation cannot be applied to concluded meetings
- **Enforced by:** `Meeting.Cancel()`
- **Invariant:** if `Status == Concluded` → throws `InvalidMeetingOperationException`.

### Rule M5 — Participants can only be added to Scheduled meetings
- **Enforced by:** `Meeting.AddParticipant(...)`
- **Invariant:** `Status == Scheduled`.

### Rule M6 — Messages can only be posted to InProgress meetings
- **Enforced by:** `Meeting.PostMessage(Message)`
- **Invariant:** `Status == InProgress`.
- **Additional invariant:** `message.SenderId` must be contained in participant list.

---

## 4. Decision Rules

### Rule D1 — Only CEO role may draft a decision
- **Enforced by:** `Decision.Draft(...)`
- **Invariant:** `ceoAgentRole == AgentRole.CEO`.

### Rule D2 — Only CEO role may finalize
- **Enforced by:** `Decision.Finalize(...)`
- **Invariant:** `finalizingAgentRole == AgentRole.CEO`.

### Rule D3 — Finalization must be done by the drafting agent
- **Enforced by:** `Decision.Finalize(...)`
- **Invariant:** `finalizingAgentId == MadeByAgentId`.

### Rule D4 — Finalized decisions are immutable
- **Enforced by:** `Decision.Finalize(...)`
- **Invariant:** `IsFinalized == false` is required.

---

## 5. Bug Report Rules

### Rule B1 — Only QA role may file a bug report
- **Enforced by:** `BugReport.File(...)`
- **Invariant:** `reportingAgentRole == AgentRole.QA`.

### Rule B2 — Resolved bug reports are immutable
- **Enforced by:**
  - `BugReport.AssignTo(...)`
  - `BugReport.LinkToTask(...)`
  - `BugReport.UpdateSeverity(...)`
- **Invariant:** if `IsResolved == true`, mutation throws.

### Rule B3 — Resolution notes are mandatory
- **Enforced by:** `BugReport.Resolve(...)`
- **Invariant:** `resolutionNotes` must be non-empty.

### Rule B4 — Resolve is idempotent
- **Enforced by:** `BugReport.Resolve(...)`
- **Behavior:** if already resolved, returns without changing state.

---

## 6. Domain Event Production Rules

These rules describe when aggregates emit domain events as part of enforcing business changes.

- Task emits:
  - `TaskCreatedEvent` on creation
  - `TaskAssignedEvent` on assignment
  - `TaskCompletedEvent` on completion
  - `TaskFailedEvent` on failure
  - `TaskCancelledEvent` on cancellation

- Meeting emits:
  - `MeetingStartedEvent` on start
  - `MeetingConcludedEvent` on conclude
  - `MessageSentEvent` when posting a message

- Decision emits:
  - `DecisionMadeEvent` on finalize

- BugReport emits:
  - `BugReportedEvent` on file
  - `BugResolvedEvent` on resolve

- Agent emits capacity/status/workload events:
  - `AgentStatusChangedEvent`
  - `AgentTaskAcceptedEvent`
  - `AgentTaskReleasedEvent`
  - `AgentCapacityReachedEvent`
  - `AgentCapacityRestoredEvent`

