# Value Objects

Value objects represent **validated domain values** that have **no identity beyond their value**.

In AICompanyOS.Domain, value objects provide:
- centralized validation rules (length constraints, non-empty requirements, invariants)
- typed semantics (reduces “stringly-typed” mistakes)
- immutability (values do not change after creation)

---

## Why Value Objects Exist in the Domain

1. **Correctness by construction**
   - A `TaskTitle` that violates constraints cannot be created.
   - A `MessageContent` that is empty cannot be created.

2. **Explicit domain language**
   - `TaskTitle` and `DecisionOutcome` carry meaning, unlike plain `string`.

3. **Testability and maintainability**
   - Validation logic is co-located with the domain type.
   - Refactors become localized.

---

## Identity vs Value Semantics

- **Identifiers** (e.g., `TaskId`, `AgentId`) are typed wrappers around `Guid`.
  - They represent identity.
  - They are immutable and validate non-empty GUID values.

- **Domain values** (e.g., `TaskTitle`, `MessageContent`, `DecisionOutcome`)
  - represent descriptive content and rationale.
  - they enforce formatting/length rules.

---

## Value Objects: Typed Identifiers (`*Id`)

All `*Id` value objects:
- are strongly typed (prevents mixing `TaskId` with `AgentId`)
- validate that the underlying GUID is not empty
- provide `New()` and sometimes `From(string)` helpers

### `AgentId`
- Wrapper around `Guid`.
- Invariant: not `Guid.Empty`.

### `TaskId`
- Wrapper around `Guid`.
- Invariant: not `Guid.Empty`.

### `MeetingId`
- Wrapper around `Guid`.
- Invariant: not `Guid.Empty`.

### `DecisionId`
- Wrapper around `Guid`.
- Invariant: not `Guid.Empty`.

### `BugReportId`
- Wrapper around `Guid`.
- Invariant: not `Guid.Empty`.

### `MessageId`
- Wrapper around `Guid`.
- Invariant: not `Guid.Empty`.

### `WorkItemId`
- Wrapper around `Guid`.
- Invariant: not `Guid.Empty`.

---

## Value Objects: Domain Values

### `TaskTitle`
- Represents a human-readable title.
- Constraints:
  - `MinLength = 3`
  - `MaxLength = 200`
  - trims whitespace

### `MessageContent`
- Represents message body.
- Constraints:
  - non-empty and non-whitespace
  - `MaxLength = 4000`
  - trims whitespace

### `DecisionOutcome`
- Encapsulates CEO verdict and reasoning.
- Constraints:
  - verdict cannot be null/whitespace
  - reasoning cannot be null/whitespace
  - `MaxReasoningLength = 2000`
  - trims both fields

### `AgentEligibility`
- Represents whether an agent can accept work.
- Fields:
  - `CanAcceptWork` (bool)
  - `Reason` (string)
- Construction:
  - `Acceptable()` returns `CanAcceptWork = true` and empty reason.
  - `NotAcceptable(reason)` requires a non-empty reason.

### `AgentCapability`
- Represents a granular capability string (e.g., `write-code`).
- Constraints:
  - non-empty
  - normalized to lowercase
  - `MaxLength = 100`
- Design purpose:
  - capacity/routing should rely on capabilities (what an agent can do), not just broad roles.

---

## Practical Notes for Developers

- Create value objects at the boundary of domain operations.
  - Example: validate `TaskTitle` before constructing a `Task`.
- Prefer typed IDs over raw `Guid` in domain signatures.
- Do not pass around unvalidated strings; use value objects to encode domain constraints.

