namespace SimpleBattleSim.Services;

public static class InputService
{
    public static string GetInput(string question)
    {
        Console.WriteLine(question);
        return Console.ReadLine() ?? "";
    }
}