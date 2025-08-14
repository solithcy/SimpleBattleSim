using SimpleBattleSim.Services;

namespace SimpleBattleSim.Characters;

public abstract class BaseCharacter
{
    public readonly string Name;
    public readonly string Type;
    public int Health;
    public readonly int Initiative;
    
    public BaseCharacter(string? name, string type)
    {
        if (name is null or "")
        {
            Name = NameService.GetService().RandomName(type);
        }
        else
        {
            Name = name;
        }

        Type = type;
        Initiative = Rand(1, 10);
        Health = Rand(1, 10);
    }

    public override string ToString()
    {
        return $"\e[0;96m{Name} the {Type}\e[0;35m (\e[0;92m{Math.Max(Health, 0)} HP\e[0;35m)";
    }

    public int Rand(int min, int max)
    {
        return Random.Shared.Next(max) + min;
    }

    public virtual int Attack()
    {
        return Rand(1, 10);
    }
    
    public virtual string AttackMessage(BaseCharacter target, int dmg)
    {
        return $"{this} attacks {target}, dealing \e[0;92m{dmg} damage\e[0;35m and leaving them with \e[0;92m{Math.Max(target.Health-dmg, 0)} HP\e[0;35m.";
    }
}