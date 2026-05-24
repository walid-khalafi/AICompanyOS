# Domain Layer Architecture

This document explains how the **Domain Layer** of **AICompanyOS** is structured using **Clean Architecture** and **Domain-Driven Design (DDD)**.

> Scope: production-grade architecture and documentation for the code that exists in `AICompanyOS.Domain`.

---

## 1. Clean Architecture Positioning

In Clean Architecture, dependencies point **inward** toward the most stable and policy-rich core.

- **Domain Layer** (`AICompanyOS.Domain`): owns business rules, invariants, lifecycle transitions, and domain event production.
- **Application Layer** (`AICompanyOS.Application`): orchestrates use-cases, loads aggregates, persists them, and dispatches domain events.
- **Infrastructure / Persistence**: implements repository interfaces and event dispatch plumbing.
- **AI Runtime / Agents / Orchestration**: performs AI reasoning, tool execution, memory, and routing at execution time.

### Dependency Direction (conceptual)

```mermaid
flowchart LR
  API[API Layer (HTTP)] --> App[Application Layer]
  App --> Domain[Domain Layer]
  App --> Infra[Infrastructure/Persistence]
  App --> Runtime[AI Runtime/Orchestration]

  Domain ---|no outbound deps| Runtime
  Domain ---|no outbound deps| Infra
  Domain ---|no outbound deps| App
```

The Domain Layer is intentionally isolated so that business rules remain correct and testable even as the runtime, persistence, or orchestration strategies evolve.

---

## 2. DDD Building Blocks

### 2.1 Aggregate Roots

The Domain Layer uses **Aggregate Roots** as the entry point for enforcing consistency boundaries.

- `Agent` (aggregate root)
- `Task` (aggregate root, canonical work unit)
- `Meeting` (aggregate root)
- `Decision` (aggregate root)
- `BugReport` (aggregate root)

Each aggregate:
- encapsulates state
- protects invariants via method-level guards
- raises domain events for meaningful state changes
- does not coordinate with other aggregates directly (cross-aggregate orchestration lives in Application)

---

### 2.2 Entities vs Value Objects

- **Entities** (`WorkItem`, `Message`) have identity and exist within an aggregate boundary.
- **Value Objects** (`TaskTitle`, `DecisionOutcome`, typed IDs, etc.) are immutable and validated at creation.

This separation makes the domain model:
- explicit about identity vs value semantics
- resilient to runtime-specific representations
- safer for persistence mapping (typed IDs and constraints)

---

### 2.3 Domain Events

Domain events are **immutable notifications** emitted by aggregates through `AggregateRoot<TId>.RaiseDomainEvent(...)`.

Key principle:
- **Domain events do not contain orchestration logic.**
- The Application layer is responsible for dispatching events and reacting to them.

---

## 3. Why the Domain Is Isolated

The Domain Layer is isolated to guarantee correctness under change:

1. **Business rules remain stable**
   - authorization rules (e.g., CEO/QA constraints)
   - lifecycle state machines (e.g., task terminal immutability)
   - invariants (e.g., meeting participant minimum)

2. **Runtime leakage is prevented**
   - the Domain does not depend on AI reasoning primitives, tool execution APIs, or memory/context models

3. **Maintainability improves**
   - developers can reason about the system by reading domain policies and invariants, not orchestration scripts

---

## 4. Aggregate Consistency Boundary Diagram

Conceptually, each aggregate root defines a consistency boundary.

```mermaid
flowchart TD
  subgraph Domain[Domain Layer]
    A[Agent]
    T[Task]
    M[Meeting]
    D[Decision]
    B[BugReport]

    T --> WI[WorkItem (contained entity)]
    T --> MsgTask[Message (contained entity)
(optional relation via identifiers)]
    M --> MsgMeeting[Message (contained entity)]
  end

  App[Application Layer]
  App --> A
  App --> T
  App --> M
  App --> D
  App --> B
```

Note: `WorkItem` and `Message` are modeled as **domain entities**, but they are controlled by their owning aggregate(s). Cross-aggregate coordination is performed in Application.

