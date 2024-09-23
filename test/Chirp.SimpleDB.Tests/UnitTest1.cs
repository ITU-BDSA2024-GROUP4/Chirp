using SimpleDB;

namespace Chirp.SimpleDB.Tests;
public record Cheep(string Author, string Message, long Timestamp);
public class UnitTest1
{
    [Fact]
    public void TestWriteAndRead()
    {
        var db = new CSVDatabase<Cheep>();
        var message = "big boy";
        var user = "joe";
        DateTimeOffset timestamp = DateTime.UtcNow;
        var cheep = new Cheep(user, message, timestamp.ToUnixTimeSeconds());
        db.Store(cheep);
        var read = db.Read(1);
        
        foreach (var joe in read)
        {
            Assert.Equal(joe, cheep);
        }
    }
}