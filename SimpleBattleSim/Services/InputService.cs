namespace SimpleBattleSim.Services;

public static class InputService
{
    private static LoggingService _logger = LoggingService.Instance;
    public static string GetInput(string question)
    {
        _logger.Log(question);
        return Console.ReadLine() ?? "";
    }
}