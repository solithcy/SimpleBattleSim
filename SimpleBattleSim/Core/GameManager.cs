using Microsoft.VisualBasic;
using SimpleBattleSim.Characters;
using SimpleBattleSim.Services;

namespace SimpleBattleSim.Core;

public class GameManager
{
    private GameState _state;
    private List<Team> _teams = [];
    private int _teamCount;

    public GameManager(int teamCount)
    {
        _state = GameState.TeamCreation;
        _teamCount = teamCount;
        if (_teamCount <= 0) throw new TeamCountLessThanZeroException();
    }

    // Loop does an iteration of the state machine and returns false if the state is final.
    public bool Loop()
    {
        switch (_state)
        {
            case GameState.TeamCreation:
                return TeamCreationLoop() && Loop();
            case GameState.Gameplay:
                return GameplayLoop() && Loop();
        }

        return false;
    }

    // TeamCreationLoop takes user input until all teams are ready to battle. If something goes wrong, returns false to
    // exit state machine loop. Otherwise, returns true. Sets state to GameState.Gameplay when teams are ready.
    public bool TeamCreationLoop()
    {
        int teamIdx = Math.Max(0, _teams.Count - 1);
        if (_teams.Count >= teamIdx+1 && _teams[teamIdx].Players.Count == 3) teamIdx++;
        if (teamIdx >= _teamCount)
        {
            _state = GameState.Gameplay;

            Console.WriteLine("=== TEAMS ===");
            foreach (Team t in _teams)
            {
                Console.WriteLine($" - Team {t.Name}");
                foreach (Player p in t.Players)
                {
                    Console.WriteLine($"  - {p.Character}");
                    Console.WriteLine($"   - Initiative: {p.Character.Initiative}");
                }
            }
            
            return true;
        }

        Team team;
        if (_teams.Count < teamIdx+1)
        {
            team = new Team
            {
                Idx = _teams.Count
            };
            _teams.Add(team);
        }
        else
        {
            team = _teams[teamIdx];
        }

        if (team.Name is null)
        {
            team.Name = "";
            while (Strings.Trim(team.Name) == "")
            {
                team.Name = InputService.GetInput($"Please enter a team name for team {team.Idx}: ");
            }
        }
        else
        {
            BaseCharacter? character = null;
            while (character is null)
            {
                string c = InputService.GetInput(
                    $"Team {team.Name} please choose a class for character {team.Players.Count + 1}\n[Wi]zard, [Wa]rrior or [C]leric: ");
                if (c.Length <= 2)
                {
                    character = c.PadRight(2, ' ')[..2].ToLower() switch
                    {
                        "wi" => new Wizard(null),
                        "wa" => new Warrior(null),
                        "c " => new Cleric(null),
                        _ => null,
                    };
                }
                else
                {
                    character = c.ToLower() switch
                    {
                        "wizard" => new Wizard(null),
                        "warrior" => new Warrior(null),
                        "cleric" => new Cleric(null),
                        _ => null,
                    };
                }
            }

            Player p = new Player(character, teamIdx);
            team.Players.Add(p);
        }

        return true;
    }

    public bool GameplayLoop()
    {
        return false;
    }
}

class TeamCountLessThanZeroException() : Exception("Team count must be above 0");