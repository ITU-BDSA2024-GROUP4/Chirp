using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.CheepRepository;

public class CheepRepositoryIntegrationTests : IAsyncLifetime

{
    private SqliteConnection _connection = null!;
    private ChirpDBContext _context = null!;
    private CheepRepository _repository = null!;
    
    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection);

        _context = new ChirpDBContext(builder.Options);
        await _context.Database.EnsureCreatedAsync();

        _repository = new CheepRepository(_context);
    }
    
    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _context.DisposeAsync();
    }

	[Fact]
    public void DatabaseInitialization()
    {
        var results = _repository.GetCheepsFromAuthor("Helge", 0);
        
        foreach (var result in results)
            Assert.Equal("Hello, BDSA students!", result.Message);
    }

    [Theory]
    [InlineData("johnDoe", "john.doe@mail.com", "some text")]
    public void CreateAuthorAndCheepTest(string author, string email, string message){
        Author newAuthor = _repository.CreateAuthor(author, email);
        
        _repository.CreateCheep(newAuthor, message);

        var result = _repository.GetCheepsFromAuthor(author, 0);

        Assert.True(result.Count>0);

        foreach (var cheep in result)
        {
            Assert.Equal(cheep.Author, author);
        }
    }
}