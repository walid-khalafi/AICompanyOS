namespace AICompanyOS.Domain.Exceptions;

/// <summary>
/// Base exception for all domain rule violations.
/// Throw this (or a subclass) when a business invariant is broken.
/// The Application layer should catch these and translate them into
/// appropriate error responses.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
