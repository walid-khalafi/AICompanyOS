using System.Text.Json;
using System.Text.Json.Serialization;
using AICompanyOS.Domain.Primitives;

namespace AICompanyOS.Application.Common.Outbox;

public static class DomainEventOutboxMapper
{
    public static IReadOnlyList<OutboxMessage> MapToOutboxMessages(
        IReadOnlyList<IDomainEvent> domainEvents,
        DateTime occurredOnUtc)
    {
        if (domainEvents.Count == 0)
            return Array.Empty<OutboxMessage>();

        // Use built-in JSON serialization only.
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return domainEvents.Select(e =>
        {
            var eventType = e.GetType().FullName ?? e.GetType().Name;

            // We serialize the event instance. If it contains non-serializable members,
            // this will throw and should be treated as a transaction failure.
            var payload = JsonSerializer.Serialize(e, e.GetType(), options);

            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Payload = payload,
                OccurredOnUtc = occurredOnUtc,
                Processed = false,
                ProcessedOnUtc = null
            };
        }).ToArray();
    }
}

