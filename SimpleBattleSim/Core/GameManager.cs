using Microsoft.VisualBasic;
using SimpleBattleSim.Characters;
using SimpleBattleSim.Services;

namespace SimpleBattleSim.Core;

public class GameManager
{
    private GameState _state;
    private List<Team> _teams = [];
    private int _teamCount;
    private int _turnIndex = 0;
    private int? _winningTeam;
    private List<Player>? _sortedPlayers = null;
    private LoggingService _logger = LoggingService.Instance;

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
                return TeamCreationLoop();
            case GameState.Gameplay:
                return GameplayLoop();
            case GameState.GameOver:
                _logger.Log($"\n=== Team {_teams[_winningTeam ?? 0].Name} has won!! ===");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    // TeamCreationLoop takes user input until all teams are ready to battle. If something goes wrong, returns false to
    // exit state machine loop. Otherwise, returns true. Sets state to GameState.Gameplay when teams are ready.
    private bool TeamCreationLoop()
    {
        int teamIdx = Math.Max(0, _teams.Count - 1);
        if (_teams.Count >= teamIdx+1 && _teams[teamIdx].Players.Count == 3) teamIdx++;
        if (teamIdx >= _teamCount)
        {
            _state = GameState.Gameplay;

            _logger.Log("=== TEAMS ===\n");
            foreach (Team t in _teams)
            {
                _logger.Log($" - Team {t.Name}\n");
                foreach (Player p in t.Players)
                {
                    _logger.Log($"  - {p.Character}\n");
                    _logger.Log($"   - Initiative: {p.Character.Initiative}\n");
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
                team.Name = InputService.GetInput($"Please enter a team name for team {team.Idx+1}: ");
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
            if(team.Players.Count == 3) Console.WriteLine();
        }

        return true;
    }

    // GameplayLoop makes players attack eachother until a team wins.
    private bool GameplayLoop()
    {
        _ = InputService.GetInput("[ENTER] to continue");
        Console.WriteLine();
        if (_sortedPlayers is null)
        {
            _sortedPlayers = new List<Player>();
            foreach (var tp in _teams.SelectMany(t => t.Players))
            {
                _sortedPlayers.Add(tp);
            }

            _sortedPlayers.Sort(((p1, p2) => 
                p1.Character.Initiative < p2.Character.Initiative ? 1 : -1
            ));
        }

        Player p = _sortedPlayers[_turnIndex % _sortedPlayers.Count];
        Team playerTeam = _teams.Find(t => t.Idx == p.TeamIdx)!;
        List<Player> targets = _teams.Where(t=>t.Idx != p.TeamIdx).SelectMany(t => t.Players).ToList();
        Player? target = p.GetTarget(targets);
        if (target is null)
        {
            _winningTeam = playerTeam.Idx;
            _state = GameState.GameOver;
            return true;
        }

        _logger.Log($"[Team {playerTeam.Name}] ");
        int dmg = p.Character.Attack();
        string msg = p.Character.AttackMessage(target.Character, dmg);
        p.Character.ClassEffects();
        target.Character.Health -= dmg;
        _logger.Log(msg);

        foreach(Player player_ in new[]{target, p})
            if (player_.Character.Health <= 0)
            {
                Team targetTeam = _teams.Find(t => t.Idx == player_.TeamIdx)!;
                targetTeam.Players.Remove(player_);
                _logger.Log($"{player_.Character} dies.. Team {targetTeam.Name} has {targetTeam.Players.Count} players left\n");
                _sortedPlayers.Remove(player_);
            }

        _turnIndex++;
        return true;
    }
}

class TeamCountLessThanZeroException() : Exception("Team count must be above 0");