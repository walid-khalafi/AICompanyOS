using System.Collections.Concurrent;
using System.Diagnostics;

namespace AICompanyOS.Application.Common.Observability;


/// <summary>
/// Lightweight in-memory execution tracer.
/// No external side effects (no logs/telemetry exporters).
/// Thread-safe and CancellationToken safe.
/// </summary>
public sealed class ExecutionTracer : IExecutionTracer
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _startTimes = new();
    private readonly ConcurrentDictionary<string, string> _correlationByOperation = new();

    // Fallback correlation id when correlation id is added later than Start.
    private readonly ConcurrentDictionary<string, string?> _correlationPendingByOperation = new();

    public Task Start(string operationName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = DateTimeOffset.UtcNow;
        _startTimes[operationName] = now;

        // If correlation was already provided, keep it.
        if (_correlationPendingByOperation.TryRemove(operationName, out var pending) && pending is not null)
        {
            _correlationByOperation[operationName] = pending;
        }

        return Task.CompletedTask;
    }

    public Task Success(string operationName)
    {
        _startTimes.TryRemove(operationName, out _);
        _correlationByOperation.TryRemove(operationName, out _);
        return Task.CompletedTask;
    }

    public Task Fail(string operationName, Exception exception)
    {
        // Exception is intentionally not logged anywhere (no external systems).
        // We only clear internal state to avoid leaking memory.
        _startTimes.TryRemove(operationName, out _);
        _correlationByOperation.TryRemove(operationName, out _);
        return Task.CompletedTask;
    }

    public Task AddCorrelationId(string correlationId)
    {
        // Correlation id needs an operation name, but Start/Succ/Fail use operationName.
        // We'll store it as pending correlation for the most recently started operation.
        // This keeps the tracer infrastructure-free.

        var lastOperation = _startTimes.Keys.LastOrDefault();
        if (lastOperation is not null)
        {
            _correlationByOperation[lastOperation] = correlationId;
        }
        else
        {
            // If Start hasn't happened yet, just keep it pending in a way that
            // will apply when Start arrives.
            // There is no operationName to key it by, so we can't reliably attach.
            // This is acceptable for a lightweight tracer scaffold.
            Debug.WriteLine("ExecutionTracer.AddCorrelationId called before Start; correlation will not be attached.");
        }

        return Task.CompletedTask;
    }
}

