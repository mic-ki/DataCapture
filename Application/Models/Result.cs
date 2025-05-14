namespace Application.Models;

public sealed class Result<T>
{
    public bool Succeeded { get; }
    public string[] Errors { get; }
    public T? Data { get; }

    private Result(bool succeeded, T? data, string[] errors)
    {
        Succeeded = succeeded;
        Data = data;
        Errors = errors;
    }

    public static Result<T> Ok(T? data)
        => new Result<T>(true, data, Array.Empty<string>());

    public static Result<T> Error(params string[] errors)
        => new Result<T>(false, default, errors);

    public static Task<Result<T>> OkAsync(T? data)
        => Task.FromResult(Ok(data));

    public static Task<Result<T>> ErrorAsync(params string[] errors)
        => Task.FromResult(Error(errors));
}