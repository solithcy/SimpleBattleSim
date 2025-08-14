using SimpleBattleSim.Services;

namespace SimpleBattleSim.Characters;

public abstract class BaseCharacter
{
    public readonly string Name;
    public readonly string Type;
    public int Health;
    public readonly int Initiative;
    
    public BaseCharacter(string name, string type)
    {
        Name = name;
        Type = type;
        Initiative = Rand(1, 10);
        Health = Rand(1, 10);
        if (Name == "")
        {
            Name = NameService.GetService().RandomName(type);
        }
    }

    public int Rand(int min, int max)
    {
        return Random.Shared.Next(max) + min;
    }

    public virtual int Attack()
    {
        return Rand(1, 10);
    }
}