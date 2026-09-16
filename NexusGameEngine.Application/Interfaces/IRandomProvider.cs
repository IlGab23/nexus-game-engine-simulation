namespace NexusGameEngine.Application.Interfaces;

public interface IRandomProvider
{
    int GetRandomNumberInRange(int includedStart, int excludedEnd);
}
