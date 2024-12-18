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
    [InlineData("test", "test@gmail.com")]
    public void CreateAuthorTest(string author, string email)
    {   
        // Arrange && Act
        Author result = _authorRepository.AddAuthor(author, email);
        
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
        Cheep newCheep = new Cheep()
        {
            Author = newAuthor, CheepId = newAuthor.AuthorId, Text = "test", TimeStamp = DateTime.Now,
        };
        
        // Act
        Cheep cheep = _cheepRepository.AddCheep(newCheep, newAuthor);
        
        // Assert
        Assert.Equal("test", cheep.Text);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public void GetCheepsFromAuthorPageTest(string author)
    {
        
        // Arrange && Assert
        var cheeps = _cheepRepository.GetCheepsFromAuthorPage(author, 0);
        
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
        var cheeps = _cheepRepository.GetCheeps(page);
        
        // Assert
        Assert.Equal(32, cheeps.Count);
    }

    [Theory]
    [InlineData("ropf@itu.dk", "Helge")]
    [InlineData("adho@itu.dk", "Adrian")]
    public void GetCheepsFromAuthorPageEmailTest(string email, string author)
    {   
        
        // Arrange && Act
        var cheeps = _cheepRepository.GetCheepsFromAuthorPageEmail(email, 0);

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
        
        Cheep newCheep = new Cheep()
        {
            CheepId = 20, //Some random int
            AuthorId = newAuthor.AuthorId,
            Author = newAuthor,
            Text = new string('a',161),
            TimeStamp = DateTime.Now
        };
        // Act && Assert
        Assert.Throws<ValidationException>( () => _cheepRepository.AddCheep(newCheep, newAuthor));
    }
    
    //TEST if user can follow same user more than once (logical fallacy!!!)
    [Theory]
    [InlineData("VictorDuplicate@dupe.it", "victor@nodupes.it")]
    public void DuplicateFollowTest(string userEmail, string authorEmail)
    {
        var userAuthor = _authorRepository.AddAuthor("Victor Duplicate", userEmail);
        var targetAuthor = _authorRepository.AddAuthor("Victor NoDupes", authorEmail);

        _authorRepository.AddFollow(userEmail, authorEmail);
        try
        {
            _authorRepository.AddFollow(userEmail, authorEmail);
        }
        catch (Exception ex)
        {
            ex.GetBaseException();
        }
        var exception = Assert.Throws<ApplicationException>(() =>
        {
            _authorRepository.AddFollow(userEmail, authorEmail);
        });

        Assert.Equal("TooManyFollows", exception.Message);

        int followers = _authorRepository.GetFollowerCount(authorEmail); //TODO: change to username
        Assert.True(2 > followers);
    }

    [Fact]
    public void canAddAuthorToDb(){
        // Arrange
       var author = _authorRepository.AddAuthor("joe", "joe@joe.com");
       
    }
    
}