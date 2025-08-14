using SimpleBattleSim.Characters;
using SimpleBattleSim.Services;

namespace SimpleBattleSim.Core;

public class Player
{
    public readonly BaseCharacter Character;
    public readonly int TeamIdx;
    private RandomService _rng;
    public TargetStrategies Strat { get; init; }

    public Player(BaseCharacter character, int teamIdx, RandomService rng)
    {
        _rng = rng;
        Character = character;
        TeamIdx = teamIdx;
        Array strats = Enum.GetValues<TargetStrategies>();
        Strat = (TargetStrategies) strats.GetValue(_rng.Next(0, strats.Length))!;
    }

    public Player? GetTarget(List<Player> targets)
    {
        if (targets.Count == 0) return null;
        return Strat switch
        {
            TargetStrategies.HighestHealth => targets.MaxBy(p => p.Character.Health),
            TargetStrategies.LowestHealth => targets.MinBy(p => p.Character.Health),
            TargetStrategies.Random => targets[_rng.Next(0, targets.Count)],
            _ => targets[0]
        };
    }
}