using SimpleBattleSim.Characters;

namespace SimpleBattleSim.Core;

public class Player(BaseCharacter character, int teamIdx)
{
    public readonly BaseCharacter Character = character;
    public readonly int TeamIdx = teamIdx;
}