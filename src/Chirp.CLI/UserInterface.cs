using SimpleDB;

namespace Chirp.CLI;

public class UserInterface
{
    public void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        DateTimeOffset timestamp;

        foreach (Cheep cheep in cheeps)
        {
            timestamp = ConvertTime(cheep.Timestamp);
            Console.WriteLine($"{cheep.Author} @ {timestamp.DateTime}: {cheep.Message}");
        }
    }
    
    public DateTimeOffset ConvertTime(long unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).ToLocalTime();
    }
}