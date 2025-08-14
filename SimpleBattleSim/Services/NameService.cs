using System.Reflection;
using System.Text.Json;

namespace SimpleBattleSim.Services;

public struct Names
{
    public readonly List<string> Cleric;
    public readonly List<string> Warrior;
    public readonly List<string> Wizard;
}

public class NameService
{
    private static NameService? _service;
    private readonly Names _names;
    private NameService()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("SimpleBattleSim.Resources.names.json");
        using StreamReader streamReader = new StreamReader(stream!);
        string serialised = streamReader.ReadToEnd();
        
        _names = JsonSerializer.Deserialize<Names>(serialised);
    }
    
    public static NameService GetService()
    {
        return _service ??= new NameService();
    }

    public string RandomName(string type)
    {
        var names = type switch
        {
            "cleric" => _names.Cleric,
            "warrior" => _names.Cleric,
            "wizard" => _names.Cleric,
            _ => throw new NoNamesForClassException()
        };

        return names[Random.Shared.Next(names.Count)];
    }
}

public class NoNamesForClassException : Exception;