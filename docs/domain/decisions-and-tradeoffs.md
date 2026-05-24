# Decisions and Trade-offs

This document explains the key architectural decisions behind the Domain Layer and the trade-offs made to balance **pure DDD** with **practical AI multi-agent systems**.

> This documentation describes decisions reflected in the current `AICompanyOS.Domain` codebase.

---

## 1. Why the Domain Uses Aggregates as the Primary Consistency Boundary

Trade-off:
- **Pros:** aggregates provide a clear place to enforce invariants.
- **Cons:** cross-aggregate coordination is constrained and must be done by Application.

Decision:
- The Domain owns lifecycle state machines and invariants within each aggregate.
- The Application layer coordinates across aggregates when multiple boundaries must be involved.

This is visible in the Task aggregate and related event design:
- `Task` enforces assignment and terminal immutability.
- `Agent` provides eligibility and signals capacity changes through events.

---

## 2. Why `Task` Became the Canonical Aggregate

Decision:
- `Task` is the single canonical representation of a unit of work.

Motivation (from the current model):
- `Task` owns the task lifecycle and the terminal immutability rule.
- Assignment is guarded by `AgentEligibility`, keeping business policy localized.

Trade-off:
- A single canonical aggregate reduces “split-brain” task states.
- It requires execution granularity to be represented separately (as contained entity `WorkItem`).

---

## 3. Why Execution Granularity (`WorkItem`) Is a Contained Entity

Decision:
- `WorkItem` is an entity with its own lifecycle, but it is not modeled as an aggregate root.

Motivation:
- WorkItem execution semantics differ from the higher-level Task lifecycle.
- Keeping it as an entity avoids coupling WorkItem state to Task lifecycle semantics.

Trade-off:
- `WorkItem` persistence and lifecycle changes are managed within the Task boundary.

---

## 4. Why Orchestration Is Not in Domain

Decision:
- Domain aggregates do not implement orchestration workflows.

Motivation:
- Orchestration decisions depend on application concerns:
  - transaction boundaries
  - event dispatch strategy
  - retry/compensation policy
  - read model projection requirements

Trade-off:
- The domain stays strict and invariant-driven.
- The Application layer becomes the coordinator.

---

## 5. Why Runtime Context and Execution Concerns Are Not in Domain

Decision:
- The Domain Layer excludes runtime execution models.

Observed approach:
- Domain method signatures use typed identifiers and value objects.
- Domain events carry domain-relevant payloads (IDs, structured outcomes), not runtime context.

Trade-off:
- Runtime must map domain outcomes into its internal execution representation.
- Domain remains stable when runtime/LLM/tool implementations evolve.

---

## 6. Concepts Like `AgentTask` and `ExecutionContext`

Decision:
- The Domain Layer avoids runtime-oriented first-class objects as responsibilities of aggregates.

In the current implementation:
- Agent workload signaling is expressed through:
  - `Agent.NotifyTaskAccepted(...)`
  - `Agent.NotifyTaskReleased(...)`
  - and capacity threshold events

Instead of modeling “AgentTask” as a separate aggregate, the design uses:
- `Task` as the canonical work record
- `Agent` as the availability/eligibility policy record
- events to connect them asynchronously

Similarly, execution context is not part of Domain invariants because it would:
- introduce runtime coupling
- expand the domain model with execution-specific fields

---

## 7. Balancing Pure DDD with Practical AI Systems

Key balancing principles present in the codebase:

- **Deterministic invariants in Domain**
  - terminal immutability for tasks and bug reports
  - strict lifecycle transitions
  - explicit authorization rules

- **Asynchronous communication via domain events**
  - Domain events represent “what happened”
  - Application handles reactions, projections, and side effects

- **Capacity-aware routing hooks**
  - Agent emits capacity events so Application/runtime can adjust routing
  - Domain remains rule-based rather than runtime-script-based

---

## 8. Summary Table

| Decision | Benefit | Cost |
|---|---|---|
| Task is canonical | single source of truth | WorkItem used for granularity |
| Orchestration not in Domain | separation of concerns | Application must coordinate |
| Runtime context not in Domain | prevents dependency/leakage | runtime must map outcomes |
| Domain events as notifications | decoupling/extensibility | requires dispatch infrastructure |

