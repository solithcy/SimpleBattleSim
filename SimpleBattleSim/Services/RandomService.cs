namespace SimpleBattleSim.Services;

public class RandomService
{
    private readonly Random _random = new();
    public virtual int Next(int min, int max) => _random.Next(min, max);
}