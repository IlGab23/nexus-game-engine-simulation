using System.Globalization;
using System.Text.RegularExpressions;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities.ValueObjects;

public sealed partial record Email
{
    public string Value { get; init; }
    public const byte MAX_EMAIL_LENGTH = 254;

    private Email(string email)
    {
        Value = email;
    }

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return Error.Validation("Email.Empty", "Email cannot be empty");

        if (email.Length > MAX_EMAIL_LENGTH || email.Length < 3) return Error.Validation("Email.InvalidLength", "Email cannot be under 3 char and above 254 char");

        if (!EmailRegex().IsMatch(email)) return Error.Validation("Email.InvalidFormat", "Email format is not valid");

        return new Email(email.Trim().ToLowerInvariant());
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();
}
