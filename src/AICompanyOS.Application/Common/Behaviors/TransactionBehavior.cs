using MediatR;

namespace AICompanyOS.Application.Common.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Scaffold only; real transaction boundary comes from persistence layer.
        _ = request;

        var operationName = typeof(TRequest).Name;
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
        catch (Exception ex)
        {
            if (request is IExecutionTracerAccessor accessor3 && accessor3.Tracer is not null)
            {
                await accessor3.Tracer.Fail(operationName, ex);
            }
            throw;
        }
    }
}


