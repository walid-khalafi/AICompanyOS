using AICompanyOS.Domain.Primitives;

namespace AICompanyOS.Application.Common.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken);
}

