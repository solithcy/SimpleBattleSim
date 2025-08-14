using SimpleBattleSim.Characters;
using SimpleBattleSim.Core;
using SimpleBattleSim.Services;

namespace Testing;

public class CharacterTests
{
    private TestData? _data;

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
}