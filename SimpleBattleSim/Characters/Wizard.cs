namespace SimpleBattleSim.Characters;

public class Wizard : BaseCharacter
{
    public Wizard(string? name) : base(name, "Wizard")
    {
        
    }

    public override int Attack()
    {
        Health--;
        return base.Attack() * 2;
    }
}