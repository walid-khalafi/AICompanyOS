# Lifecycle Flows

This document describes the major lifecycle flows governed by the Domain Layer.

All flows are implemented as **state machines** inside aggregate methods with explicit guards and invariants.

---

## 1. Agent Lifecycle

**Aggregate:** `Agent`  
**States:** `Inactive`, `Idle`, `Busy`, `Suspended`, `Decommissioned`

```mermaid
stateDiagram-v2
  [*] --> Inactive

  Inactive --> Idle : Activate()
  Idle --> Busy : MarkBusy()
  Idle --> Suspended : Suspend()

  Busy --> Idle : MarkIdle()
  Busy --> Suspended : Suspend()

  Suspended --> Idle : Reactivate()

  Inactive --> Decommissioned : Decommission()
  Idle --> Decommissioned : Decommission()
  Busy --> Decommissioned : Decommission()
  Suspended --> Decommissioned : Decommission()
```

**Key guards**
- Activation only from `Inactive`.
- Busy only from `Idle`.
- Idle cannot be entered from `Inactive` or `Decommissioned`.
- Reactivation requires `Suspended`.
- Decommission is irreversible.

---

## 2. Task Lifecycle (Canonical)

**Aggregate:** `Task`  
**States:** `Pending`, `Assigned`, `InProgress`, `Blocked`, `UnderReview`, `Completed`, `Failed`, `Cancelled`

```mermaid
stateDiagram-v2
  [*] --> Pending

  Pending --> Assigned : AssignTo(...)

  Assigned --> InProgress : Start()
  Assigned --> Blocked : Block()
  Assigned --> Cancelled : Cancel()

  InProgress --> UnderReview : SubmitForReview()
  InProgress --> Completed : Complete(result)
  InProgress --> Failed : Fail(reason)
  InProgress --> Blocked : Block()
  InProgress --> Cancelled : Cancel()

  UnderReview --> Completed : Complete(result)

  Blocked --> Assigned : AssignTo(...)

  Completed --> [*] : terminal/immutable
  Failed --> [*] : terminal/immutable
  Cancelled --> [*] : terminal/immutable
```

**Key invariants**
- Assignment allowed only from `Pending` or `Blocked`.
- Completion/failure/cancellation produce terminal states.
- Terminal states are immutable: domain methods are blocked by `EnsureNotTerminal()`.
- `AssignTo` requires `AgentEligibility.CanAcceptWork == true`.

---

## 3. Meeting Lifecycle

**Aggregate:** `Meeting`  
**States:** `Scheduled`, `InProgress`, `Concluded`, `Cancelled`

```mermaid
stateDiagram-v2
  [*] --> Scheduled

  Scheduled --> InProgress : Start()
  InProgress --> Concluded : Conclude(summary)

  Scheduled --> Cancelled : Cancel()
  InProgress --> Cancelled : Cancel() (allowed in current code)

  Concluded --> [*] : terminal
  Cancelled --> [*] : terminal/immutable (no stated transitions)
```

**Key guards**
- `Start()` only from `Scheduled`.
- `Conclude()` only from `InProgress`.
- Messages can only be posted while `InProgress`.
- Sender must be a participant.
- Participants can be added only while `Scheduled`.
- Minimum distinct participants enforced at scheduling time (>=2).

---

## 4. Decision Lifecycle

**Aggregate:** `Decision`  
**States:** Draft (implicit), Finalized (`IsFinalized = true`)

```mermaid
stateDiagram-v2
  [*] --> Draft
  Draft --> Finalized : Finalize(outcome)
  Finalized --> [*] : immutable
```

**Key guards**
- Drafting requires CEO role.
- Finalizing requires CEO role and must be done by the drafting agent.
- Finalized decisions are immutable.

---

## 5. BugReport Lifecycle

**Aggregate:** `BugReport`  
**States:** Unresolved (implicit), Resolved (`IsResolved = true`)

```mermaid
stateDiagram-v2
  [*] --> Unresolved
  Unresolved --> Resolved : Resolve(resolutionNotes)
  Resolved --> [*] : immutable
```

**Key guards**
- Filing requires QA role.
- Resolution notes are mandatory.
- Resolved bugs cannot be reassigned, re-linked, or have severity changed.
- Resolve is idempotent.

---

## 6. WorkItem Execution Lifecycle

**Entity:** `WorkItem` (contained in Task boundary by convention)  
**States:** `Pending`, `InProgress`, `Completed`, `Failed`

```mermaid
stateDiagram-v2
  [*] --> Pending

  Pending --> InProgress : StartExecution(agentId)
  InProgress --> Completed : RecordSuccess(output)

  InProgress --> Pending : RecordFailure(error) [if retry remaining]
  InProgress --> Failed : RecordFailure(error) [if retries exhausted]
```

**Key invariants**
- Cannot start completed work items.
- Cannot start when already in progress.
- Retries constrained by `MaxRetries = 3`.
- `CanRetry()` true only when `Status == Pending` and retry count within limits.

