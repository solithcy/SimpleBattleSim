using System.Reflection;
using Moq;
using SimpleBattleSim.Core;
using SimpleBattleSim.Services;

namespace Testing;

public class GameManagerTests
{
    
    private TestData _data;
    private GameManager _manager;

    private void SetupGameplayLoopMocks(bool printStatus, LoggingService? logger)
    {
        var mockInput = new Mock<InputService>();
        mockInput.Setup(i => i.GetInput("[ENTER] to continue, [S + ENTER] to see team statuses")).Returns(printStatus ? "s" : "");
        _manager = new GameManager(2, mockInput.Object, logger ?? LoggingService.Instance);

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
        _manager = new GameManager(2, new InputService(), LoggingService.Instance);
    }

    [Test]
    public void Check_That_GameplayLoop_Correctly_Sorts_Players()
    {
        SetupGameplayLoopMocks(false, null);
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
        SetupGameplayLoopMocks(false, null);
        for (int i = 0; i < 100; i++)
        {
            if (!_manager.Loop())
            {
                var state = _manager.GetType()
                    .GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetValue(_manager)!;
                Assert.That(state, Is.EqualTo(GameState.GameOver));
                return;
            }
        }

        Assert.Fail();
    }

    [Test]
    public void Check_That_TeamCreationLoop_Processes_Inputs()
    {
        var mockInput = new Mock<InputService>(); 
        int calls = -1;
        mockInput.Setup(i => i.GetInput(It.IsAny<string>())).Callback((string s) =>
        {
            // we do it this way as it's difficult to determine how much has been called during teamcreation vs gameloop
            if (s.Contains("team name") || s.Contains("character")) calls++;
        }).Returns(() =>
        {
            return calls switch
            {
                0 => "team1", //  team one name
                1 => "cleric", // full names
                2 => "wizard",
                3 => "warrior",
                4 => "team2", //  team 2 name
                5 => "c", //      shortened names
                6 => "wi",
                7 => "wa",
                _ => "",
            };
        });
        _manager = new GameManager(2, mockInput.Object, LoggingService.Instance);
        
        // loop should only need to be called 8 times, so this won't test any gameloop code.
        // the 9th call will set _state to GameState.Gameplay
        for (int i = 0; i < 9; i++)
        {
            if(!_manager.Loop()) break;
        }

        Assert.That(calls, Is.EqualTo(7));
        var teams = (List<Team>)_manager.GetType()
            .GetField("_teams", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_manager)!;
        foreach (Team t in teams)
        {
            Assert.Multiple(() =>
            {
                Assert.That(t.Name!, Does.StartWith("team"));
                Assert.That(t.Players[0].Character.Type, Is.EqualTo("Cleric"));
                Assert.That(t.Players[1].Character.Type, Is.EqualTo("Wizard"));
                Assert.That(t.Players[2].Character.Type, Is.EqualTo("Warrior"));
            });
        }

        var state = _manager.GetType()
            .GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_manager)!;
        Assert.That(state, Is.EqualTo(GameState.Gameplay));
    }
}