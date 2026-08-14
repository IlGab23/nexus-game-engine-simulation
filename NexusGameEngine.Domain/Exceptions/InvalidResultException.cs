namespace NexusGameEngine.Domain.Exceptions;

#pragma warning disable RCS1194
public class InvalidResultException : DomainException
{
    public InvalidResultException() : base(
        "Invalid Result state: a successful result cannot contain errors, and a failed result must contain at least one error.",
        500,
        "Result pattern value error")
    {
    }

}
