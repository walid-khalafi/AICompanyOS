namespace AICompanyOS.Application.Common.Result;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public string Error { get; }

    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(true, value, string.Empty);

    public static Result<T> Fail(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error must be provided.", nameof(error));

        return new(false, default!, error);
    }
}

