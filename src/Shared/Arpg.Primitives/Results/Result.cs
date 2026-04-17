namespace Arpg.Primitives.Results;

public abstract class ResultBase
{
    public bool IsSuccess { get; protected init; }
    public bool IsFailed => !IsSuccess;
    public List<Error> Errors { get; protected init; } = [];
}

public class Result : ResultBase
{
    protected Result() { }

    public static Result Ok() => new() { IsSuccess = true };

    public static Result Fail(Error error) => new()
    {
        IsSuccess = false,
        Errors = [error]
    };

    public static Result Fail(IEnumerable<Error> errors) => new()
    {
        IsSuccess = false,
        Errors = [.. errors]
    };

    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);
    public static Result<T> Fail<T>(Error error) => Result<T>.Fail(error);
    public static Result<T> Fail<T>(IEnumerable<Error> errors) => Result<T>.Fail(errors);

    public static implicit operator Result(Error error) => Fail(error);
    public static implicit operator Result(List<Error> errors) => Fail(errors);
}
