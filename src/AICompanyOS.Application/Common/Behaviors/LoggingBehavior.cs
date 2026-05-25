using MediatR;

namespace AICompanyOS.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _ = request;

        // Tracing enrichment is intentionally best-effort and kept infrastructure-free.
        // This behavior does not call tracer hooks directly; other behaviors control lifecycle.
        if (request is IExecutionTracerAccessor accessor && accessor.Tracer is not null)
        {
            // No-op enrichment point.
        }

        return await next();
    }
}


