namespace Application.Models;

public interface IResult
{
    string[] Errors { get; init; }
    bool Succeeded { get; init; }
}

public interface IResult<out T> : IResult
{
    T? Data { get; }
}

public class Result : IResult
{
    internal Result()
    {
        Errors = [];
    }

    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public string ErrorMessage => string.Join(", ", Errors ?? []);

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }

    public static Result Success() => new Result(true, Array.Empty<string>());
    public static Task<Result> SuccessAsync() => Task.FromResult(Success());
    public static Result Failure(params string[] errors) => new Result(false, errors);
    public static Task<Result> FailureAsync(params string[] errors) => Task.FromResult(Failure(errors));
}

public class Result<T> : Result, IResult<T>
{
    public Result(object dto)
    {
        throw new NotImplementedException();
    }

    private Result()
    {
        throw new NotImplementedException();
    }

    public T? Data { get; private init; }
    private static Result<T> Success(T data) => new Result<T> { Succeeded = true, Data = data };
    public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));
    public new static Result<T> Failure(params string[] errors) => new Result<T> { Succeeded = false, Errors = errors.ToArray() };
    public new static Task<Result<T>> FailureAsync(params string[] errors) => Task.FromResult(new Result<T> { Succeeded = false, Errors = errors.ToArray() });
    public static Result Failure(string error) => new Result(false, [error]);


}