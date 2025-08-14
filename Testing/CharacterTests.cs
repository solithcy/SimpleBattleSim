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

            ;
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

public class Tests
{
    private TestData _data;
    private GameManager _manager;

    private void SetupGameplayLoopMocks()
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
    }

    [SetUp]
    public void Setup()
    {
        _data = new TestData(false);
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
    public void Check_That_TargetingStrategies_Target_Correctly()
    {
        _data = new TestData(true);
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

    [Test]
    public void Check_That_GameplayLoop_Correctly_Sorts_Players()
    {
        SetupGameplayLoopMocks();
        _manager.Loop();
        var sortedPlayers = _manager.GetType()
            .GetField("_sortedPlayers", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_manager);
        Assert.That(sortedPlayers, Is.EquivalentTo(_data.OrderedPlayerList));
    }

    [Test]
    public void Check_That_GameplayLoop_Eventually_Resolves()
    {
        _data = new TestData(true);
        SetupGameplayLoopMocks();
        for (int i = 0; i < 100; i++)
        {
            if (!_manager.Loop())
            {
                return;
            }
        }

        Assert.Fail();
    }

    [Test]
    public void Check_That_TeamCreationLoop_Calls_InputService()
    {
        var mockInput = new Mock<InputService>(); 
        int calls = 0;
        // this will set team names to c and all characters to clerics
        mockInput.Setup(i => i.GetInput(It.IsAny<string>())).Callback((string s) =>
        {
            // we do it this way as it's difficult to determine how much has been called during teamcreation vs gameloop
            if (s.Contains("team name") || s.Contains("character")) calls++;
        }).Returns("c");
        _manager = new GameManager(2, mockInput.Object);
        
        // loop should only need to be called 8 times, so this won't test any gameloop code.
        // the 9th call will set _state to GameState.Gameplay
        for (int i = 0; i < 9; i++)
        {
            if(!_manager.Loop()) break;
        }

        Assert.That(calls, Is.EqualTo(8));
        var teams = (List<Team>)_manager.GetType()
            .GetField("_teams", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_manager)!;
        foreach (Team t in teams)
        {
            Assert.That(t.Name, Is.EqualTo("c"));
            foreach (Player p in t.Players)
            {
                Assert.That(p.Character.Type, Is.EqualTo("Cleric"));
            }
        }

        var state = _manager.GetType()
            .GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_manager)!;
        Assert.That(state, Is.EqualTo(GameState.Gameplay));
    }
}