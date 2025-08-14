using SimpleBattleSim.Core;

class Program
{
    public static void Main(string[] args)
    {
        var game = new GameManager(2);
        while (game.Loop()) { }
        
    }
}