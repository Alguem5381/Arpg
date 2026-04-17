namespace Arpg.Primitives.Results;

public class Result<T> : ResultBase
{
    private T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value of a failed Result.");

    private Result() { }

    public static Result<T> Ok(T value) => new()
    {
        IsSuccess = true,
        _value = value
    };

    public static Result<T> Fail(Error error) => new()
    {
        IsSuccess = false,
        Errors = [error]
    };

    public static Result<T> Fail(IEnumerable<Error> errors) => new()
    {
        IsSuccess = false,
        Errors = [.. errors]
    };

    public static implicit operator Result<T>(T value) => Ok(value);
    public static implicit operator Result<T>(Error error) => Fail(error);
    public static implicit operator Result<T>(List<Error> errors) => Fail(errors);

    public static implicit operator Result<T>(Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException(
                "Cannot implicitly convert a successful Result to Result<T>. Use Result<T>.Ok(value) explicitly.");

        return Fail(result.Errors);
    }
}
