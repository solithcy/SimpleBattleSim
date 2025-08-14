using SimpleBattleSim.Core;
using SimpleBattleSim.Services;

class Program
{
    public static void Main(string[] args)
    {
        var game = new GameManager(2, new InputService());
        while (game.Loop()) { }
        
    }
}