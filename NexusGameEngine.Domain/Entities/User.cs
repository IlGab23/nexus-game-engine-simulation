using NexusGameEngine.Domain.Entities.ValueObjects;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities;

public sealed class User
{
    public Guid Id { get; init; }
    public Guid? SystemRoleId { get; private set; } = null;
    public SystemRole? SystemRole { get; private set; }
    public string UserName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }

    // public Player? Player { get; private set; } // TODO: Da ripristinare nella Epic 3

    private User(Guid id, string userName, Email email, string passwordHash, Guid? systemRoleId = null)
    {
        Id = id;
        SystemRoleId = systemRoleId;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }

#pragma warning disable CS8618
    public User()
    {
    }

    public static Result<User> Create(string userName, Email email, string passwordHash, Guid? systemRoleId = null)
    {
        if (string.IsNullOrWhiteSpace(userName)) return Error.Validation("User.EmptyUserName", "UserName cannot be empty");
        if (userName.Length < 3 || userName.Length > 50) return Error.Validation("User.InvalidUserNameLength", "UserName cannot be lower than 3 char and more than 50 char");

        if (string.IsNullOrWhiteSpace(passwordHash)) return Error.Validation("User.EmptyPasswordHash", "PasswordHash cannot be empty");

        return new User(Guid.NewGuid(), userName, email, passwordHash, systemRoleId);
    }
}
