namespace SimpleBattleSim.Services;

public class InputService
{
    private static LoggingService _logger = LoggingService.Instance;
    public virtual string GetInput(string question)
    {
        _logger.Log(question);
        return Console.ReadLine() ?? "";
    }
}