using MediatR;

namespace AICompanyOS.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Placeholder scaffold; FluentValidation integration comes in Phase 1.
        _ = request;
        return await next();
    }
}


