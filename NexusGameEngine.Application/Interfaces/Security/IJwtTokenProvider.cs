using NexusGameEngine.Domain.Entities;

namespace NexusGameEngine.Application.Interfaces.Security;

public interface IJwtTokenProvider
{
    string GenerateJWT(User user);
}
