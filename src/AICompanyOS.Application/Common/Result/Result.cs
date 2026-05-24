namespace AICompanyOS.Application.Common.Result;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new(true, string.Empty);

    public static Result Fail(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error must be provided.", nameof(error));

        return new(false, error);
    }
}

