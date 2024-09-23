using SimpleDB;

namespace Chirp.SimpleDB.Tests;
public record Cheep(string Author, string Message, long Timestamp);
public class UnitTest1
{
    [Fact]
    public void TestWriteAndRead()
    {
        var message = "big boy";
        var user = "joe";
        DateTimeOffset timestamp = DateTime.UtcNow;
        var cheep = new Cheep(user, message, timestamp.ToUnixTimeSeconds());
        CSVDatabase<Cheep>.Instance.Store(cheep);
        var read = CSVDatabase<Cheep>.Instance.Read(1);
        
        foreach (var joe in read)
        {
            Assert.Equal(joe, cheep);
        }
    }
}