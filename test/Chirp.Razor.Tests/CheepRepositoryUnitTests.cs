using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.CheepRepository;

public class CheepRepositoryUnitTests : IAsyncLifetime
{
    private SqliteConnection connection;
    private ChirpDBContext context;
    private CheepRepository repository;
    
    public async Task InitializeAsync()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync();

        repository = new CheepRepository(context);
    }
    
    public async Task DisposeAsync()
    {
        await connection.DisposeAsync();
        await context.DisposeAsync();
    }

	[Fact]
    public void DatabaseInitialization()
    {
        var results = repository.GetCheepsFromAuthor("Helge", 0);
        
        foreach (var result in results)
            Assert.Equal("Hello, BDSA students!", result.Message);
    }
}