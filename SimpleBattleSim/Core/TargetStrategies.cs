namespace SimpleBattleSim.Core;

// Each player will get a random targeting strategy. This determines who they attack.
public enum TargetStrategies
{
    LowestHealth, HighestHealth, Random
}