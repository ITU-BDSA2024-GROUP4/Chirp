namespace SimpleDB;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        DateTimeOffset timestamp;

        foreach (Cheep cheep in cheeps)
        {
            timestamp = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime();
            Console.WriteLine($"{cheep.Author} @ {timestamp.DateTime}: {cheep.Message}");
        }
    }
}