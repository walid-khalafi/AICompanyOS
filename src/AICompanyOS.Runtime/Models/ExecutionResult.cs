namespace AICompanyOS.Runtime.Models;

/// <summary>
/// Represents the outcome of a completed runtime execution unit.
///
/// Ephemeral — produced at the end of an execution step or session.
/// May be used by the Application layer to issue domain commands
/// (e.g., Task.Complete, Decision.Finalize) but is NOT itself a domain object.
/// </summary>
public sealed class ExecutionResult
{
    /// <summary>Whether the execution succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>The output produced by the agent, if any.</summary>
    public string? Output { get; }

    /// <summary>Error message if execution failed.</summary>
    public string? ErrorMessage { get; }

    /// <summary>Final status of the execution.</summary>
    public ExecutionStatus Status { get; }

    /// <summary>UTC timestamp when the result was produced.</summary>
    public DateTime ProducedAtUtc { get; }

    /// <summary>Optional structured metadata produced during execution.</summary>
    public IReadOnlyDictionary<string, object> Metadata { get; }

    private ExecutionResult(
        bool isSuccess,
        ExecutionStatus status,
        string? output,
        string? errorMessage,
        IReadOnlyDictionary<string, object>? metadata)
    {
        IsSuccess    = isSuccess;
        Status       = status;
        Output       = output;
        ErrorMessage = errorMessage;
        ProducedAtUtc = DateTime.UtcNow;
        Metadata     = metadata ?? new Dictionary<string, object>();
    }

    /// <summary>Creates a successful execution result.</summary>
    public static ExecutionResult Success(string? output = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new(true, ExecutionStatus.Completed, output, null, metadata);

    /// <summary>Creates a failed execution result.</summary>
    public static ExecutionResult Failure(string errorMessage, IReadOnlyDictionary<string, object>? metadata = null)
        => new(false, ExecutionStatus.Failed, null, errorMessage, metadata);

    /// <summary>Creates a cancelled execution result.</summary>
    public static ExecutionResult Cancelled()
        => new(false, ExecutionStatus.Cancelled, null, "Execution was cancelled.", null);

    /// <summary>Creates a timed-out execution result.</summary>
    public static ExecutionResult TimedOut()
        => new(false, ExecutionStatus.TimedOut, null, "Execution timed out.", null);
}
