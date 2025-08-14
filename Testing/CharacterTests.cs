using System.Reflection;
using Moq;
using SimpleBattleSim.Characters;
using SimpleBattleSim.Core;
using SimpleBattleSim.Services;

namespace Testing;

internal class TestData
{
    public List<Player> UnorderedPlayerList;
    public List<Player> OrderedPlayerList;

    public TestData()
    {
        var mockRng = new Mock<RandomService>();
        mockRng.SetupSequence(rng => rng.Next(1, 10))
            .Returns(2) //   p1 initiative
            .Returns(2) //  p1 health
            .Returns(10) //  p2 initiative
            .Returns(10) //  p2 health
            .Returns(5) //   p3 initiative
            .Returns(2); // p3 health
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

public class Tests
{
    private TestData _data = new TestData();
    private GameManager _manager;

    [SetUp]
    public void Setup()
    {
        _manager = new GameManager(2, new InputService());
    }

    [Test]
    public void Check_That_WarriorHealth_Is_Higher()
    {
        List<int> healths = [];
        for (int i = 0; i < 100; i++)
        {
            var w = new Warrior(null, new RandomService());
            healths.Add(w.Health);
        }

        Assert.That(healths.Max(), Is.GreaterThan(10));
    }

    [Test]
    public void Check_That_WizardHealth_Decrements_AfterAttack()
    {
        var w = new Wizard(null, new RandomService());
        var health = w.Health;
        w.ClassEffects();
        Assert.That(w.Health, Is.EqualTo(health - 1));
    }

    [Test]
    public void Check_That_ClericHealth_Increments_AfterAttack()
    {
        var c = new Cleric(null, new RandomService());
        var health = c.Health;
        c.ClassEffects();
        Assert.That(c.Health, Is.EqualTo(health + 1));
    }

    [Test]
    public void Check_That_GameplayLoop_Correctly_Sorts_Players()
    {
        var mockInput = new Mock<InputService>();
        mockInput.Setup(i => i.GetInput("[ENTER] to continue")).Returns("");
        _manager = new GameManager(2, mockInput.Object);

        _manager.GetType().GetField("_teams", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(_manager, new List<Team>
            {
                new()
                {
                    Idx = 0,
                    Name = "",
                    Players = [_data.UnorderedPlayerList[0], _data.UnorderedPlayerList[2]],
                },
                new()
                {
                    Idx = 1,
                    Name = "",
                    Players = [_data.UnorderedPlayerList[1]],
                }
            });
        _manager.GetType().GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(_manager, GameState.Gameplay);
        _manager.Loop();
        var sortedPlayers = _manager.GetType()
            .GetField("_sortedPlayers", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_manager);
        Assert.That(sortedPlayers, Is.EquivalentTo(_data.OrderedPlayerList));
    }

    [Test]
    public void Check_That_TargetingStrategies_Target_Correctly()
    {
        // player 1 is of strat LowestHealth, and so should target p3 as it has the lowest health
        Assert.That(_data.UnorderedPlayerList[0].GetTarget(
                new List<Player>
                {
                    _data.UnorderedPlayerList[1],
                    _data.UnorderedPlayerList[2],
                }),
            Is.EqualTo(_data.UnorderedPlayerList[2])
        );
        // player 2 is of strat HighestHealth, and so should target p1 as it has the lowest health
        Assert.That(_data.UnorderedPlayerList[1].GetTarget(
                new List<Player>
                {
                    _data.UnorderedPlayerList[0],
                    _data.UnorderedPlayerList[2],
                }),
            Is.EqualTo(_data.UnorderedPlayerList[0])
        );
        // player 3 is of strat Random, and so should target p1 and p2
        List<Player> res = [];
        for (int i = 0; i < 100; i++)
        {
            res.Add(_data.UnorderedPlayerList[2].GetTarget(
                    new List<Player>
                    {
                        _data.UnorderedPlayerList[0],
                        _data.UnorderedPlayerList[1],
                    })!
            );
        }
        Assert.That(res.Distinct().Count(), Is.EqualTo(2));
    }
}