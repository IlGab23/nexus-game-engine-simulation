using System.ComponentModel;

namespace NexusGameEngine.Domain.ResultPattern;

public enum ErrorType { Failure, Validation, NotFount, Conflict, None }

public record Error(string Name, string Description, ErrorType Type, string[]? Details = null)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    public static Error Validation(string Name, string Description, string[]? Details = null) => new(Name, Description, ErrorType.Validation, Details);
    public static Error NotFound(string Name, string Description, string[]? Details = null) => new(Name, Description, ErrorType.NotFount, Details);
    public static Error Conflict(string Name, string Description, string[]? Details = null) => new(Name, Description, ErrorType.Conflict, Details);
    public static Error RegistrationEmailExist => new("User.DuplicateEmail", "This email is arleady registered", ErrorType.Conflict);
    public static Error LoginWrongCredentials => new("User.WrongCredentials", "The email or password are wrong", ErrorType.Conflict);
    public static Error RefreshTokenCompromised => new("RefreshToken.Stolen", "RefreshToken is already been revoked, all tokens have been revoked", ErrorType.Failure);
    public static Error RefreshTokenExpired => new("RefreshToken.Expired", "RefreshToken is expired", ErrorType.Failure);
}
