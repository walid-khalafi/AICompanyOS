namespace AICompanyOS.Application.Common.Outbox;

public interface IOutboxWriter
{
    Task AddAsync(IReadOnlyCollection<OutboxMessage> messages, CancellationToken cancellationToken = default);
}

