using SimpleBattleSim.Services;

namespace SimpleBattleSim.Characters;

public class Cleric : BaseCharacter
{
    public Cleric(string? name, RandomService rng) : base(name, "Cleric", rng)
    {
        
    }

    public override int Attack()
    {
        return base.Attack();
    }

    public override void ClassEffects()
    {
        Health++;
    }

    public override string AttackMessage(BaseCharacter target, int dmg)
    {
        string msg = base.AttackMessage(target, dmg);
        return msg + $"{this} gains \e[0;92m{1} HP\e[0;35m, leaving them with \e[0;92m{Health+1} HP\e[0;35m.\n";
    }
}