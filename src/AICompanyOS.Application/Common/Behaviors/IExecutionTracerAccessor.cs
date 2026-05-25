using AICompanyOS.Application.Common.Observability;

namespace AICompanyOS.Application.Common.Behaviors;

/// <summary>
/// Helper abstraction allowing pipeline behaviors to locate a tracer.
/// Implemented by the tracer itself (via request-carry) or by request types
/// that wish to pass an execution tracer reference along.
/// </summary>
public interface IExecutionTracerAccessor
{
    IExecutionTracer? Tracer { get; }
}

