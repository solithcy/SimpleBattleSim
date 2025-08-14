using Moq;
using SimpleBattleSim.Characters;
using SimpleBattleSim.Core;
using SimpleBattleSim.Services;

namespace Testing;

internal class TestData
{
    public List<Player> UnorderedPlayerList;
    public List<Player> OrderedPlayerList;

    public TestData(bool defaultRandomBehaviour)
    {
        var realRng = new RandomService();
        var mockRng = new Mock<RandomService>();
        int rngCallIdx = 0;
        mockRng.Setup(rng => rng.Next(1, 10)).Returns(() =>
        {
            // p1 initiative, p1 health, p2 initiative, p2 health, p3 initiative, p3 health
            int[] overrides = [2, 2, 10, 10, 5, 2];
            if (rngCallIdx < overrides.Length)
            {
                return overrides[rngCallIdx++];
            }
            return defaultRandomBehaviour ? realRng.Next(1, 10) : 0;
        });
        var targetingTypesRng = new Mock<RandomService>();
        targetingTypesRng.SetupSequence(rng => rng.Next(0, 3))
            .Returns(0) //  LowestHealth
            .Returns(1) //  HighestHealth
            .Returns(2) //  Random
            .CallBase();
        targetingTypesRng.Setup(rng => rng.Next(0, 2)).CallBase(); // setting up Random targeting to actually b random
        var p1 = new Player(new Cleric(null, mockRng.Object), 0, targetingTypesRng.Object);
        var p2 = new Player(new Cleric(null, mockRng.Object), 1, targetingTypesRng.Object);
        var p3 = new Player(new Cleric(null, mockRng.Object), 0, targetingTypesRng.Object);

        UnorderedPlayerList = [p1, p2, p3];
        OrderedPlayerList = [p2, p3, p1];
    }
}