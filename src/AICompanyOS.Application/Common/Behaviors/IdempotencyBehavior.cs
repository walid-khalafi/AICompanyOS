using MediatR;

namespace AICompanyOS.Application.Common.Behaviors;

/// <summary>
/// Idempotency pipeline hook.
/// Architecture scaffold only (no persistence/cache).
/// Conceptually detects duplicate request identifiers.
/// </summary>
public sealed class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Skeleton only.
        // In a production implementation, this behavior would:
        // - Extract an idempotency key from the request (if present)
        // - Check a persistence/cache store for duplicates
        // - Short-circuit with the previous result if a duplicate is detected
        // This phase intentionally does not integrate any storage.
        _ = request;

        return await next();
    }
}

