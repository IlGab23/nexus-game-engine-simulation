using NexusGameEngine.Domain.ResultPattern;
namespace NexusGameEngine.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public User? User { get; init; }

    public string TokenHash { get; init; } = string.Empty;
    public DateTimeOffset Expiry { get; init; }
    public DateTimeOffset? RevokeAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    private RefreshToken(Guid id, Guid userId, string tokenHash, DateTimeOffset expiry)
    {
        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        Expiry = expiry;
    }

    public bool IsExpired(TimeProvider timeProvider) => timeProvider.GetUtcNow() > Expiry;
    public bool IsRevoked => RevokeAt.HasValue;
    public bool IsActive(TimeProvider timeProvider) => !IsExpired(timeProvider) && !IsRevoked;

    public void Revoke(DateTimeOffset revokeDate, string? replacedBy = null)
    {
        RevokeAt = revokeDate;
        ReplacedByTokenHash = replacedBy;
    }

    public static Result<RefreshToken> Create(Guid userId, string tokenHash, DateTimeOffset expiry)
    {
        return new RefreshToken(Guid.NewGuid(), userId, tokenHash, expiry);
    }

}
