using NexusGameEngine.Domain.Exceptions;

namespace NexusGameEngine.Domain.ResultPattern;

public class ResultBase
{
    protected ResultBase(bool isSuccess, List<Error> errorList)
    {
        if ((isSuccess && errorList.Count > 0) || (!isSuccess && errorList.Count == 0))
        {
            throw new InvalidResultException();
        }

        IsSuccess = isSuccess;
        ErrorList = errorList;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public readonly List<Error> ErrorList;

    public static ResultBase Success() => new(true, []);
    public static ResultBase Failure(List<Error> errors) => new(false, errors);

    public static implicit operator ResultBase(List<Error> errors) => Failure(errors);
    public static implicit operator ResultBase(Error error) => Failure([error]);
}
