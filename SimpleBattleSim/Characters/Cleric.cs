namespace SimpleBattleSim.Characters;

public class Cleric : BaseCharacter
{
    public Cleric(string? name) : base(name, "Cleric")
    {
        
    }

    public override int Attack()
    {
        Health++;
        return base.Attack();
    }
}