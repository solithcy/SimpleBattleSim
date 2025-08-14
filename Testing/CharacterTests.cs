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
}