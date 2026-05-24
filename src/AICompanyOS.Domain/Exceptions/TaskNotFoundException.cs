using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Exceptions;

/// <summary>
/// Thrown when a task lookup fails — task does not exist in the system.
/// </summary>
public sealed class TaskNotFoundException(TaskId taskId) : DomainException($"Task with ID '{taskId}' was not found.")
{
}
