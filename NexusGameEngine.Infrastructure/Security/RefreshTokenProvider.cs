using System.Security.Cryptography;
using System.Text;
using NexusGameEngine.Application.Interfaces.Security;

namespace NexusGameEngine.Infrastructure.Security;

public class RefreshTokenProvider : IRefreshTokenProvider
{
    public byte[] CalculateHash(string plainTextToken)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(plainTextToken);
        byte[] hashByte = SHA512.HashData(bytes);
        return hashByte;
    }

    public string GenerateRT()
    {
        byte[] bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public bool Verify(string plainTextToken, string hash)
    {
        byte[] calculatedHashBytes = CalculateHash(plainTextToken);
        byte[] hashBytes = Convert.FromBase64String(hash);
        return CryptographicOperations.FixedTimeEquals(calculatedHashBytes, hashBytes);
    }

    public string ConvertToString(byte[] hashBytes)
    {
        return Convert.ToBase64String(hashBytes);
    }
}
