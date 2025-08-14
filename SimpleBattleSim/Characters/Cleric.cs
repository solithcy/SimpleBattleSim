namespace SimpleBattleSim.Characters;

public class Cleric : BaseCharacter
{
    public Cleric(string? name) : base(name, "cleric")
    {
        
    }

    public override int Attack()
    {
        Health++;
        return base.Attack();
    }
}