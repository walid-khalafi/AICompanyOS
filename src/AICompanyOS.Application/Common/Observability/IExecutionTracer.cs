namespace AICompanyOS.Application.Common.Observability;

public interface IExecutionTracer
{
    Task Start(string operationName, CancellationToken cancellationToken);
    Task Success(string operationName);
    Task Fail(string operationName, Exception exception);

    // Optional correlation support
    Task AddCorrelationId(string correlationId);
}

