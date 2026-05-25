# AICompanyOS Application Layer TODO (UPDATED — ARCHITECTURE VIEW)

---

## Phase 1 — Architecture Setup ✔ COMPLETED
- [x] Create Application folder structure (Vertical Slice: Features/*)
- [x] Add Application pipeline scaffolding (ValidationBehavior / LoggingBehavior / TransactionBehavior - scaffold only)
- [x] Configure MediatR + FluentValidation dependencies
- [x] Add common result models (Result / Result<T>)
- [x] Add domain event dispatching abstraction (application-level)
- [x] Add Application DI registration extension (MediatR + behaviors + validators)
- [x] Enforce async + CancellationToken conventions (scaffold)

---

## Phase 2 — Task Features ✔ COMPLETED
- [x] Create CreateTask command & handler & validator
- [x] Create AssignTask command & handler & validator
- [x] Create CompleteTask command & handler & validator

---

## Phase 3 — Meeting Features ✔ COMPLETED
- [x] Create ScheduleMeeting command & handler & validator
- [x] Create StartMeeting command & handler & validator
- [x] Create ConcludeMeeting command & handler & validator

---

## Phase 4 — Decision Features ✔ COMPLETED
- [x] Create DraftDecision command & handler & validator
- [x] Create FinalizeDecision command & handler & validator

---

## Phase 5 — BugReport Features ✔ COMPLETED
- [x] Create FileBugReport command & handler & validator
- [x] Create ResolveBugReport command & handler & validator

---

## Phase 6 — Application Infrastructure Layer Hardening (IN PROGRESS)

---

## Step 6.1 — Core Abstractions
- [x] Create IUnitOfWork abstraction (Application/Abstractions)
- [x] Create IDomainEventDispatcher (Application/Common/Events)
- [x] Create IExecutionTracer (Application/Common/Observability)

---

## Step 6.2 — Domain Event Wiring
- [x] Implement domain event dispatch pipeline (Application level only)
- [x] Ensure events are dispatched AFTER persistence
- [x] Ensure aggregate domain events are cleared after dispatch

---

## Step 6.3 — Transaction Boundary
- [x] Define transaction boundary per command
- [x] Integrate UnitOfWork into handlers (interface only)
- [x] Ensure rollback behavior is supported conceptually


---

## Step 6.4 — Outbox Pattern (Application Model Only)
- [x] Create OutboxMessage model
- [x] Create IOutboxWriter abstraction
- [x] Map DomainEvents → OutboxMessages


---

## Step 6.5 — Pipeline Behaviors Enhancement
- [x] AuthorizationBehavior (skeleton)
- [x] IdempotencyBehavior (skeleton)
- [x] RetryBehavior (skeleton)
- [x] CorrelationIdBehavior (skeleton)

---

## Step 6.6 — Observability Layer
- [ ] Add ExecutionTracer implementation (Application level)
- [ ] Track command execution lifecycle
- [ ] Add structured logging hooks in behaviors

---

## Step 6.7 — Application Services Refactor
- [ ] Introduce Application Services layer
- [ ] Move cross-handler orchestration out of handlers
- [ ] Keep handlers single-responsibility only

---

## Rule
- Each step must compile independently
- Run: dotnet build src/AICompanyOS.sln -c Debug after every step
---

# 🟡 Phase 7 — Runtime Integration (FUTURE LAYER)
- [ ] Agent Runtime integration hooks
- [ ] Tool execution pipeline
- [ ] Memory snapshot integration
- [ ] AI orchestration bridge