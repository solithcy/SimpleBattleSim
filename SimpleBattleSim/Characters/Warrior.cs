namespace SimpleBattleSim.Characters;

public class Warrior : BaseCharacter
{
    public Warrior(string? name) : base(name, "warrior")
    {
        Health += 5;
    }
}