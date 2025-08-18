namespace Minicommerce.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors, bool isConflict = false)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
        IsConflict = isConflict;
    }

    public bool Succeeded { get; init; }
    public string[] Errors { get; init; }
    public bool IsConflict { get; init; }

    public static Result Success() => new(true, Array.Empty<string>());
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    public static Result Failure(params string[] errors) => new(false, errors);
    public static Result Conflict(IEnumerable<string> errors) => new(false, errors, true);
    public static Result Conflict(params string[] errors) => new(false, errors, true);
}

public class Result<T> : Result
{
    internal Result(bool succeeded, T? data, IEnumerable<string> errors, bool isConflict = false) 
        : base(succeeded, errors, isConflict)
    {
        Data = data;
    }

    public T? Data { get; init; }

    public static Result<T> Success(T data) => new(true, data, Array.Empty<string>());
    public static new Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
    public static new Result<T> Failure(params string[] errors) => new(false, default, errors);
    public static new Result<T> Conflict(IEnumerable<string> errors) => new(false, default, errors, true);
    public static new Result<T> Conflict(params string[] errors) => new(false, default, errors, true);
}