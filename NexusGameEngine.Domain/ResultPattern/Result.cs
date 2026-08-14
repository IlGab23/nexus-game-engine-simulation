namespace NexusGameEngine.Domain.ResultPattern;

public class Result<TValue> : ResultBase
{
    private readonly TValue? _value;
    private Result(TValue value) : base(true, [])
    {
        _value = value;
    }
    private Result(List<Error> errors) : base(false, errors)
    {
        _value = default;
    }

    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot take value of a failed property");

    public static Result<TValue> Success(TValue value) => new(value);
    public static new Result<TValue> Failure(List<Error> errors) => new(errors);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(List<Error> errors) => Failure(errors);
    public static implicit operator Result<TValue>(Error error) => Failure([error]);
}
