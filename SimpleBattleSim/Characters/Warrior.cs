namespace SimpleBattleSim.Characters;

public class Warrior : BaseCharacter
{
    public Warrior(string? name) : base(name, "Warrior")
    {
        Health += 5;
    }
}