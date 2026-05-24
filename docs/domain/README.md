# AICompanyOS — Domain Layer

## Overview

The Domain Layer is the core of AICompanyOS. It defines what the system *is* — the business concepts, rules, and invariants that govern how AI agents operate, collaborate, and produce outcomes within a simulated company structure.

This layer is written in pure C# with no external dependencies. It does not know about databases, HTTP, AI runtimes, or orchestration frameworks. It knows only about business logic.

---

## Purpose

The Domain Layer exists to answer one question: **what are the rules of this system?**

- What makes an agent eligible to accept a task?
- Who is authorized to finalize a decision?
- When is a task considered terminal and immutable?
- What must be true before a meeting can start?

These rules live here, enforced unconditionally, regardless of how the system is invoked.

---

## Responsibilities

| Responsibility | Owned By |
|---|---|
| Aggregate lifecycle management | Domain |
| Business invariant enforcement | Domain |
| Domain event production | Domain |
| Role-based authorization rules | Domain |
| State transition validation | Domain |
| Repository contract definitions | Domain |
| Value object validation | Domain |

---

## What the Domain Layer Does NOT Do

| Concern | Where It Lives |
|---|---|
| Persisting aggregates | Persistence layer |
| Dispatching domain events | Application layer |
| Orchestrating multi-step workflows | Application / Orchestration layer |
| AI reasoning and prompt execution | Agents / Runtime layer |
| Memory, context, and conversation history | Agents / Runtime layer |
| Tool execution (code runners, web search) | Agents / Runtime layer |
| HTTP request/response handling | API layer |

---

## Relationship with Other Layers

```
┌─────────────────────────────────────────────┐
│                  API Layer                  │  ← HTTP, Controllers, DTOs
├─────────────────────────────────────────────┤
│             Application Layer               │  ← Use cases, event handlers, orchestration
├─────────────────────────────────────────────┤
│              Domain Layer  ◄────────────────┼── You are here
│  Aggregates · Events · Rules · Value Objects│
├─────────────────────────────────────────────┤
│         Persistence / Infrastructure        │  ← EF Core, repositories, migrations
├─────────────────────────────────────────────┤
│              Agents / Runtime               │  ← AI execution, LLM calls, tool use
└─────────────────────────────────────────────┘
```

Dependency direction is strictly inward. The Domain Layer has zero dependencies on any other layer. All other layers depend on the Domain.

---

## Design Philosophy

**Domain-Driven Design (DDD)** is applied throughout:

- Aggregates protect their own consistency boundaries.
- Entities have identity; value objects have equality by value.
- Domain events communicate what happened without coupling aggregates together.
- Repository interfaces are defined in the Domain and implemented in Persistence.
- The Application layer coordinates aggregates — the Domain never reaches across aggregate boundaries.

**Clean Architecture** governs the dependency structure:

- The Domain is the innermost ring.
- Nothing in the Domain imports from Application, Infrastructure, or Runtime.
- The Domain project has zero NuGet dependencies.

---

## Project Structure

```
AICompanyOS.Domain/
├── Primitives/          # Base classes: Entity<T>, AggregateRoot<T>, IDomainEvent
├── Entities/            # Aggregate roots and child entities
├── ValueObjects/        # Immutable typed values and identifiers
├── Enums/               # Domain enumerations (status, role, priority)
├── Events/              # Domain event records
├── Exceptions/          # Domain rule violation exceptions
├── Repositories/        # Repository interface contracts
└── Obsolete/            # Removed concepts (kept for historical reference)
```

---

## Documentation Index

| Document | Description |
|---|---|
| [architecture.md](./architecture.md) | Clean Architecture positioning, aggregate model, dependency rules |
| [aggregates.md](./aggregates.md) | Per-aggregate documentation: invariants, lifecycle, methods |
| [business-rules.md](./business-rules.md) | All enforced business rules and where each is enforced |
| [domain-events.md](./domain-events.md) | All domain events: purpose, trigger, business meaning |
| [value-objects.md](./value-objects.md) | All value objects: identity semantics, validation, usage |
| [boundaries.md](./boundaries.md) | What belongs in Domain vs Application vs Runtime |
| [lifecycle-flows.md](./lifecycle-flows.md) | State transition diagrams for all major aggregates |
| [decisions-and-tradeoffs.md](./decisions-and-tradeoffs.md) | Architectural decisions, removed concepts, and rationale |

---

## Notes for reviewers

- The Domain Layer docs describe the **current code** in `AICompanyOS.Domain`.
- Runtime orchestration, memory/context models, and AI execution details are intentionally excluded and belong to other layers.

