using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.CheepRepository;

public class CheepRepositoryUnitTests : IAsyncLifetime
{
    private SqliteConnection _connection;
    private ChirpDBContext _context;
    private CheepRepository _repository;

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
    [InlineData("test", "test@gmail.com")]
    public void CreateAuthorTest(string author, string email)
    {
        Author result = _repository.CreateAuthor(author, email);

        Assert.Equal("test", result.Name);
        Assert.Equal("test@gmail.com", result.Email);
    }

    [Theory]
    [InlineData("johnDoe", "john.doe@gmail.com", 0)]
    public void CreateCheepTest(string author, string email, int authorId)
    {
        Author newAuthor = new Author()
        {
            Name = author,
            Email = email,
            AuthorId = authorId,
            Cheeps = new List<Cheep>()
        };

        Cheep cheep = _repository.CreateCheep(newAuthor, "test");

        Assert.Equal("test", cheep.Text);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public void GetCheepsFromAuthorTest(string author)
    {

        var cheeps = _repository.GetCheepsFromAuthor(author, 0);

        Assert.True(cheeps.Count > 0);
        foreach (var cheep in cheeps)
        {
            Assert.Equal(author, cheep.Author);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void GetCheepsLength32Test(int page)
    {
        var cheeps = _repository.GetCheeps(page);

        Assert.Equal(32, cheeps.Count);
    }

    [Theory]
    [InlineData("ropf@itu.dk", "Helge")]
    [InlineData("adho@itu.dk", "Adrian")]
    public void GetCheepsFromAuthorEmailTest(string email, string author)
    {
        var cheeps = _repository.GetCheepsFromAuthorEmail(email, 0);

        Assert.True(cheeps.Count > 0);
        foreach (var cheep in cheeps)
        {
            Assert.Equal(author, cheep.Author);
        }
    }


    [Theory]
    [InlineData("john.doe@email.com", 0, "test")]
    public void CreateCheepWithEmptyAuthor(string email, int authorId, string message)
    {
        Author newAuthor = new Author()
        {
            Email = email,
            AuthorId = authorId,
            Cheeps = new List<Cheep>()
        };

        Assert.Throws<DbUpdateException>(()=>_repository.CreateCheep(newAuthor, message));
    }

    [Theory]
    [InlineData("johnDoe", 0, "test")]
    public void CreateCheepWithEmptyEmail(string author, int authorId, string message)
    {
        Author newAuthor = new Author()
        {
            Name = author,
            AuthorId = authorId,
            Cheeps = new List<Cheep>()
        };

        Assert.Throws<DbUpdateException>(()=>_repository.CreateCheep(newAuthor, message));
    }

}