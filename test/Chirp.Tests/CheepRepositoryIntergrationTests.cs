using System.Security.Cryptography;

using Chirp.Core;
using Chirp.Infrastructure;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class CheepRepositoryIntegrationTests : IAsyncLifetime

{
    private SqliteConnection _connection = null!;
    private ChirpDBContext _context = null!;
    private ICheepRepository _cheepRepository = null!;
    private IAuthorRepository _authorRepository = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection);

        _context = new ChirpDBContext(builder.Options);
        await _context.Database.EnsureCreatedAsync();

        _cheepRepository = new CheepRepository(_context);
        _authorRepository = new AuthorRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public void DatabaseInitialization()
    {
        var results = _cheepRepository.GetCheepsFromAuthorPage("Helge", 0);

        foreach (var result in results)
            Assert.Equal("Hello, BDSA students!", result.Message);
    }

    [Theory]
    [InlineData("johnDoe", "john.doe@mail.com", "some text")]
    public void CreateAuthorAndCheepTest(string author, string email, string message)
    {
        // Arrange
        // MIGHT NEED REWORK IN ASSERT
        Author newAuthor = _authorRepository.AddAuthor(author, email);
        Cheep newCheep = new Cheep()
        {
            CheepId = 114093, //Some random int
            AuthorId = newAuthor.AuthorId,
            Author = newAuthor,
            Text = message,
            TimeStamp = DateTime.Now
        };
        // Act
        _cheepRepository.AddCheep(newCheep, newAuthor);

        // Assert
        var result = _cheepRepository.GetCheepsFromAuthorPage(author, 0);

        Assert.True(result.Count > 0);

        foreach (var cheep in result)
        {
            Assert.Equal(cheep.Author, author);
        }
    }
}