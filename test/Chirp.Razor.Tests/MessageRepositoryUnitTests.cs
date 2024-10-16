using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.CheepRepository;

public class MessageRepositoryUnitTests
{
	[Fact]
    public async void DatabaseInitialization()
    {
        using var connection  = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        
        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync();

        ICheepRepository repository = new CheepRepository(context);

        var results = repository.GetCheepsFromAuthor("Helge", 0);
        
        foreach (var result in results)
            Assert.Equal("Hello, BDSA students!", result.Message);
    }
}