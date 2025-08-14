using SimpleBattleSim.Characters;

namespace SimpleBattleSim.Core;

public class Player
{
    public readonly BaseCharacter Character;
    public readonly int TeamIdx;
    public readonly TargetStrategies Strat;

    public Player(BaseCharacter character, int teamIdx)
    {
        Character = character;
        TeamIdx = teamIdx;
        Array strats = Enum.GetValues<TargetStrategies>();
        Strat = (TargetStrategies) strats.GetValue(Random.Shared.Next(strats.Length))!;
    }

    public Player? GetTarget(List<Player> targets)
    {
        if (targets.Count == 0) return null;
        return Strat switch
        {
            TargetStrategies.HighestHealth => targets.MaxBy(p => p.Character.Health),
            TargetStrategies.LowestHealth => targets.MinBy(p => p.Character.Health),
            TargetStrategies.Random => targets[Random.Shared.Next(targets.Count)],
            _ => targets[0]
        };
    }
}