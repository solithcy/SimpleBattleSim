namespace SimpleBattleSim.Services;

public sealed class LoggingService
{
    private LoggingService() { }

    public static LoggingService Instance { get; } = new();

    public void Log(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.Write($"{msg}");
        Console.ResetColor();
    }
}