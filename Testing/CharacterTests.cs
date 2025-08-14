using SimpleBattleSim.Characters;

namespace Testing;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Check_That_WarriorHealth_Is_Higher()
    {
        List<int> healths = [];
        for (int i = 0; i < 100; i++)
        {
            var w = new Warrior(null);
            healths.Add(w.Health);
        }
        Assert.That(healths.Max(), Is.GreaterThan(10));
    }

    [Test]
    public void Check_That_WizardHealth_Decrements_AfterAttack()
    {
        var w = new Wizard(null);
        var health = w.Health;
        w.Attack();
        Assert.That(w.Health, Is.EqualTo(health-1));
    }

    [Test]
    public void Check_That_ClericHealth_Increments_AfterAttack()
    {
        var c = new Cleric(null);
        var health = c.Health;
        c.Attack();
        Assert.That(c.Health, Is.EqualTo(health+1));
    }
}