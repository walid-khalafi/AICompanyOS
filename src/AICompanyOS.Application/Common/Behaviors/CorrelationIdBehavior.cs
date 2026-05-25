using MediatR;

namespace AICompanyOS.Application.Common.Behaviors;

/// <summary>
/// CorrelationId pipeline hook.
/// Generates a correlation id per request and forwards it to the optional execution tracer.
/// Infrastructure-independent (no external tracing systems).
/// </summary>
public sealed class CorrelationIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const string CorrelationIdKey = "CorrelationId";

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString("N");

        // Store on request if it opts-in via MediatR request data.
        // This does not require infrastructure; it is purely in-memory.
        if (request is IHasRequestData hasRequestData)
        {
            hasRequestData.RequestData[CorrelationIdKey] = correlationId;
        }

        // Optional tracer hook: if request provides a tracer accessor, correlate the operation.
        if (request is IExecutionTracerAccessor accessor && accessor.Tracer is not null)
        {
            await accessor.Tracer.AddCorrelationId(correlationId);
        }

        return await next();
    }

    // Minimal abstraction to avoid depending on any infrastructure.
    // If no request implements this, correlation id generation still happens.
    public interface IHasRequestData
    {
        IDictionary<string, string> RequestData { get; }
    }

    // Local re-declaration for compile-time safety without forcing request to implement it.
    // The real interface lives in Application.Common.Observability.
    private interface IExecutionTracer : AICompanyOS.Application.Common.Observability.IExecutionTracer
    {
    }
}

