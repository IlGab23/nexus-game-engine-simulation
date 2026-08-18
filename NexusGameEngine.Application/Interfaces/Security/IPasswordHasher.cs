namespace NexusGameEngine.Application.Interfaces.Security;

public interface IPasswordHasher
{
    Task<string> HashPassword(string password);
    Task<bool> VerifyPassword(string password, string hashedPassword);
}
