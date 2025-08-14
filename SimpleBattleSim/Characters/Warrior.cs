using SimpleBattleSim.Services;

namespace SimpleBattleSim.Characters;

public class Warrior : BaseCharacter
{
    public Warrior(string? name, RandomService rng) : base(name, "Warrior", rng)
    {
        Health += 5;
    }
}