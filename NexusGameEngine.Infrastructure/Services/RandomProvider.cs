using NexusGameEngine.Application.Interfaces;

namespace NexusGameEngine.Infrastructure.Services;

public class RandomProvider : IRandomProvider
{
    public int GetRandomNumberInRange(int includedStart, int excludedEnd)
    {
        return Random.Shared.Next(includedStart, excludedEnd);
    }

}
