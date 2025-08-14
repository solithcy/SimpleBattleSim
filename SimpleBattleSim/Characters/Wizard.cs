namespace SimpleBattleSim.Characters;

public class Wizard : BaseCharacter
{
    public Wizard(string name) : base(name, "wizard")
    {
        
    }

    public override int Attack()
    {
        Health--;
        return base.Attack() * 2;
    }
}