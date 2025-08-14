using SimpleBattleSim.Services;

namespace SimpleBattleSim.Characters;

public abstract class BaseCharacter
{
    public readonly string Name;
    public readonly string Type;
    private RandomService _rng;
    public int Health;
    public int Initiative { get; init; }
    
    public BaseCharacter(string? name, string type, RandomService rng)
    {
        if (name is null or "")
        {
            Name = NameService.GetService().RandomName(type.ToLower());
            Name = Name[0].ToString().ToUpper() + Name[1..];
        }
        else
        {
            Name = name;
        }

        Type = type;
        _rng = rng;
        Initiative = Rand(1, 11);
        Health = Rand(1, 11);
    }

    public override string ToString()
    {
        return $"\e[0;96m{Name} the {Type}\e[0;35m (\e[0;92m{Math.Max(Health, 0)} HP\e[0;35m)";
    }

    public int Rand(int min, int max)
    {
        return _rng.Next(min, max);
    }

    public virtual int Attack()
    {
        return Rand(1, 11);
    }
    
    public virtual string AttackMessage(BaseCharacter target, int dmg)
    {
        return $"{this} attacks {target}, dealing \e[0;92m{dmg} damage\e[0;35m and leaving them with \e[0;92m{Math.Max(target.Health-dmg, 0)} HP\e[0;35m.\n";
    }

    public virtual void ClassEffects()
    {
    }
}