using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations; 
using Chirp.Infrastructure;
using Chirp.Core;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using Xunit.Abstractions;

public class CheepRepositoryUnitTests : IAsyncLifetime
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
        var results = _repository.GetCheepsFromAuthorPage("Helge", 0);

        foreach (var result in results)
            Assert.Equal("Hello, BDSA students!", result.Message);
    }

    [Theory]
    [InlineData("test", "test@gmail.com")]
    public void CreateAuthorTest(string author, string email)
    {   
        // Arrange && Act
        Author result = _repository.CreateAuthor(author, email);
        
        // Assert
        Assert.Equal("test", result.Name);
        Assert.Equal("test@gmail.com", result.Email);
    }

    [Theory]
    [InlineData("johnDoe", "john.doe@gmail.com", 0)]
    public void CreateCheepTest(string author, string email, int authorId)
    {
        // Arrange
        Author newAuthor = new Author()
        {
            Name = author,
            Email = email,
            AuthorId = authorId,
            Cheeps = new List<Cheep>()
        };
        
        // Act
        Cheep cheep = _repository.CreateCheep(newAuthor, "test");
        
        // Assert
        Assert.Equal("test", cheep.Text);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public void GetCheepsFromAuthorPageTest(string author)
    {
        
        // Arrange && Assert
        var cheeps = _repository.GetCheepsFromAuthorPage(author, 0);
        
        // Assert
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
        // Arrange && Act
        var cheeps = _repository.GetCheeps(page);
        
        // Assert
        Assert.Equal(32, cheeps.Count);
    }

    [Theory]
    [InlineData("ropf@itu.dk", "Helge")]
    [InlineData("adho@itu.dk", "Adrian")]
    public void GetCheepsFromAuthorPageEmailTest(string email, string author)
    {   
        
        // Arrange && Act
        var cheeps = _repository.GetCheepsFromAuthorPageEmail(email, 0);

        // Assert
        Assert.True(cheeps.Count > 0);
        foreach (var cheep in cheeps)
        {
            Assert.Equal(author, cheep.Author);
        }
    }


    [Theory]
    [InlineData("johnDoe", "john.doe@gmail.com", 0)]
    public void MaxLengthCheep(string author, string email, int authorId)
    {
        
        // Arrange
        Author newAuthor = new Author()
        {
            Name = author,
            Email = email,
            AuthorId = authorId,
            Cheeps = new List<Cheep>()
        };
        
        string message = new string('a',161);
        
        // Act && Assert
        Assert.Throws<ValidationException>( () => _repository.CreateCheep(newAuthor, message));
    }
    
    //TEST if user can follow same user more than once (logical fallacy!!!)
    [Theory]
    [InlineData("VictorDUplicate@dupe.it", "victor@nodupes.it")]
    public void DuplicateFollowTest(string userEmail, string authorEmail)
    {
        int i = 0;
        for (i = 0; i < 100; i++)
        { _repository.CreateFollow(userEmail, authorEmail); }
        int followers = _repository.GetFollowerCount(authorEmail);
        Assert.False(followers == i);
        Assert.False(followers > 1);
    }
    
}