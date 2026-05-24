# Domain Boundaries (Domain vs Application vs AI Runtime)

This document defines **what belongs where** in AICompanyOS.

The Domain Layer must remain independent from:
- orchestration workflows
- AI reasoning and tool execution
- runtime memory/context representations

The goal is to ensure that **business correctness lives in the Domain**, while **execution and coordination live outside**.

---

## 1. Domain Layer: What Belongs

### 1.1 Aggregate roots, entities, and invariants

The Domain Layer owns:
- Aggregate roots and their methods
- Entity containment relationships inside aggregate boundaries
- Lifecycle/state machines
- Business invariants and guards

Examples from the codebase:
- `Task` enforces terminal immutability
- `Meeting` enforces minimum participants and message-post rules
- `Decision` enforces CEO authorization and immutability after finalization
- `BugReport` enforces QA-only filing and resolution immutability

### 1.2 Authorization and role checks

Authorization rules that are part of business policy belong in Domain.

- `Decision.Draft` and `Decision.Finalize` enforce CEO-only rules using `AgentRole`.
- `BugReport.File` enforces QA-only bug filing using `AgentRole`.

> Important: Domain uses `AgentRole` values (passed in as primitives) to stay free of cross-aggregate dependencies.

### 1.3 Domain events

Domain events belong in Domain because they describe:
- “what happened” in business terms
- what changed in a way that other parts of the system must react to

Example: `TaskAssignedEvent` is raised when the Task changes to `Assigned`.

### 1.4 Repository contracts (interface contracts)

Repository interfaces reside in Domain:
- they define the persistence contract
- they allow Application to load aggregates without knowing the storage technology

Implementations are provided by Persistence/Infrastructure.

### 1.5 Exceptions for rule violations

Rule violations are expressed as domain exceptions under `AICompanyOS.Domain.Exceptions`.

- `InvalidTaskOperationException`
- `UnauthorizedAgentOperationException`
- etc.

---

## 2. Application Layer: What Belongs

The Application layer owns coordination and use-case behavior, including:

- loading aggregates via Domain repository interfaces
- calling domain methods in correct order
- persisting aggregate state
- dispatching domain events after persistence
- handling and translating domain exceptions into API responses

### Why orchestration is not in Domain

Domain aggregates should not implement:
- multi-step workflows
- “when event X occurs, call Y then Z” logic
- runtime routing loops

Because that logic depends on application-level decisions:
- transaction boundaries
- how events are dispatched
- what read models are updated
- whether retries/compensations are used

---

## 3. AI Runtime / Agents Layer: What Belongs

The runtime layer owns AI execution details such as:
- LLM prompt construction
- agent reasoning loops
- tool execution
- memory/context management
- execution tracing

The Domain must not depend on any of these.

### How runtime leakage is mitigated

Runtime-specific details are excluded by design through these mechanisms:

1. **Typed domain primitives instead of runtime context objects**
   - The Domain takes typed IDs and value objects.
   - It does not accept “ExecutionContext” or “memory objects” as method parameters.

2. **Domain events are payload-minimal**
   - Domain events carry identifiers and domain-relevant payloads (e.g., `DecisionOutcome`, task IDs).
   - Runtime can map these to internal representations, but Domain remains stable.

3. **No dependency from Domain to Runtime**
   - `AICompanyOS.Domain` is structurally independent and does not import runtime libraries.

---

## 4. Practical Boundary Examples

### Example 1 — Routing a task to an agent
- **Domain:**
  - `Task.AssignTo(...)` enforces:
    - terminal immutability
    - allowed statuses for assignment
    - exclusivity of assignment
    - eligibility requirement via `AgentEligibility`
- **Application:**
  - loads `Task` and `Agent`
  - asks the Agent for `AgentEligibility`
  - calls `Task.AssignTo(agentId, eligibility)`
  - persists changes
  - dispatches events

### Example 2 — Meeting messaging
- **Domain:**
  - `Meeting.PostMessage(...)` enforces:
    - `InProgress` meeting required
    - sender must be participant
- **Application/Runtime:**
  - decides what agent sends which message and when

---

## 5. Summary Rules

- Put **business invariants** and **lifecycle rules** in Domain.
- Put **use-case orchestration**, persistence, and event dispatch in Application.
- Put **AI execution, tool calls, memory, and reasoning** in Runtime.
- Use **typed identifiers/value objects** to prevent runtime model leakage into Domain.

