namespace AICompanyOS.Domain.Primitives;

/// <summary>
/// Base class for all aggregate roots in the domain.
/// An aggregate root is the entry point to a consistency boundary.
/// It owns the lifecycle of all entities within its boundary and
/// is the only object that external code may hold a reference to.
/// Domain events raised inside the aggregate are collected here and
/// dispatched by the Application layer after persistence.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Read-only collection of domain events raised by this aggregate
    /// since it was last loaded or since events were last cleared.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(TId id) : base(id) { }

    // Required for ORM hydration.
#pragma warning disable CS8618
    protected AggregateRoot() { }
#pragma warning restore CS8618

    /// <summary>
    /// Raises a domain event and adds it to the pending events collection.
    /// Call this from within aggregate methods when something meaningful happens.
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    /// <summary>
    /// Clears all pending domain events. Called by the infrastructure layer
    /// after events have been dispatched.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
