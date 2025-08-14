using SimpleBattleSim.Services;

namespace SimpleBattleSim.Characters;

public class Wizard : BaseCharacter
{
    public Wizard(string? name, RandomService rng) : base(name, "Wizard", rng)
    {
        
    }

    public override int Attack()
    {
        return base.Attack() * 2;
    }

    public override void ClassEffects()
    {
        Health--;
    }

    public override string AttackMessage(BaseCharacter target, int dmg)
    {
        string msg = base.AttackMessage(target, dmg);
        return msg + $"{this} takes \e[0;92m{1} damage\e[0;35m, leaving them with \e[0;92m{Health-1} HP\e[0;35m.\n";
    }
}