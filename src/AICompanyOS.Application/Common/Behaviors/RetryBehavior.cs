using System.Diagnostics;
using MediatR;
using AICompanyOS.Domain.Exceptions;

namespace AICompanyOS.Application.Common.Behaviors;

/// <summary>
/// Simple retry behavior for transient failures (conceptual).
/// Avoids retrying DomainExceptions.
/// </summary>
public sealed class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    // Foundation-level defaults; production tuning can be moved to configuration later.
    private const int DefaultMaxRetryCount = 2;
    private static readonly TimeSpan DefaultBaseDelay = TimeSpan.FromMilliseconds(100);

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var maxRetryCount = DefaultMaxRetryCount;

        for (var attempt = 0; ; attempt++)
        {
            var operationName = typeof(TRequest).Name + $":RetryAttempt{attempt}";
            if (request is IExecutionTracerAccessor accessor && accessor.Tracer is not null)
            {
                await accessor.Tracer.Start(operationName, cancellationToken);
            }

            try
            {
                var response = await next();
                if (request is IExecutionTracerAccessor accessor2 && accessor2.Tracer is not null)
                {
                    await accessor2.Tracer.Success(operationName);
                }
                return response;
            }
            catch (DomainException ex)
            {
                // Rule: do not retry DomainExceptions.
                if (request is IExecutionTracerAccessor accessor3 && accessor3.Tracer is not null)
                {
                    await accessor3.Tracer.Fail(operationName, ex);
                }

                throw;
            }
            catch (Exception ex) when (attempt < maxRetryCount)
            {
                if (request is IExecutionTracerAccessor accessor3 && accessor3.Tracer is not null)
                {
                    await accessor3.Tracer.Fail(operationName, ex);
                }

                // Retry foundation: use simple exponential backoff.
                // CancellationToken is honored.
                var delay = TimeSpan.FromMilliseconds(
                    DefaultBaseDelay.TotalMilliseconds * Math.Pow(2, attempt));

                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Preserve cancellation.
                    throw;
                }

                // Keep looping and retry.
                Debug.WriteLine(ex);
            }
        }
    }
}

