namespace AICompanyOS.Runtime.Events;

/// <summary>
/// Publishes runtime events to registered consumers.
///
/// The primary consumer is the Application layer, which listens for
/// <see cref="RuntimeExecutionCompletedEvent"/> and <see cref="RuntimeExecutionStartedEvent"/>
/// to issue the corresponding domain commands.
///
/// This is an in-process pub/sub contract. No message broker is involved yet.
/// External event publishing (e.g., to a message bus) is a future concern.
/// </summary>
public interface IRuntimeEventPublisher
{
    /// <summary>Publishes a runtime execution started event.</summary>
    Task PublishAsync(RuntimeExecutionStartedEvent runtimeEvent, CancellationToken cancellationToken = default);

    /// <summary>Publishes a runtime execution completed event.</summary>
    Task PublishAsync(RuntimeExecutionCompletedEvent runtimeEvent, CancellationToken cancellationToken = default);
}
