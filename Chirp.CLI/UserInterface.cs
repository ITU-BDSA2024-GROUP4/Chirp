namespace Chrip.CLI;

public static class UserInterface 
{
    public static void cheepPrinter(IEnumerable<Cheep> cheeps)
    {
        DateTimeOffset timestamp;
        
        foreach (Cheep cheep in cheeps)
        {
            timestamp = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime();
            Console.WriteLine($"{cheep.Author} @ {timestamp.DateTime}: {cheep.Message}");
        }
    }

    public static void cheep(bool error)
    {
        if (error == false)
        {
            Console.WriteLine("Cheeped!");
        }
        else
        {
            Console.WriteLine("Command not recognized!");
        }
    }
}