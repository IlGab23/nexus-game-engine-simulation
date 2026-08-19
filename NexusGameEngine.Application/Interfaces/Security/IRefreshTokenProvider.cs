namespace NexusGameEngine.Application.Interfaces.Security;

public interface IRefreshTokenProvider
{
    string GenerateRT();
    bool Verify(string plainTextToken, string hash);
    byte[] CalculateHash(string plainTextToken);
    string ConvertToString(byte[] hashBytes);
}
