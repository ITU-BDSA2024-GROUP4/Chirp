using System.ComponentModel.DataAnnotations;

using Chirp.Core;
using Chirp.Infrastructure;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
            Author = newAuthor,
            CheepId = newAuthor.AuthorId,
            Text = "test",
            TimeStamp = DateTime.Now,
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
            Text = new string('a', 161),
            TimeStamp = DateTime.Now
        };
        // Act && Assert
        Assert.Throws<ValidationException>(() => _cheepRepository.AddCheep(newCheep, newAuthor));
    }

    //TEST if user can follow same user more than once (logical fallacy!!!)
    [Theory]
    [InlineData("VictorDuplicate@dupe.it", "victor@nodupes.it")]
    public void DuplicateFollowTest(string userEmail, string authorEmail)
    {
        var userAuthor = _authorRepository.AddAuthor("Victor Duplicate", userEmail);
        var targetAuthor = _authorRepository.AddAuthor("Victor NoDupes", authorEmail);

        _authorRepository.AddFollow("Victor Duplicate", "Victor NoDupes");
        try
        {
            _authorRepository.AddFollow("Victor Duplicate", "Victor NoDupes");
        }
        catch (Exception ex)
        {
            ex.GetBaseException();
        }
        var exception = Assert.Throws<ApplicationException>(() =>
        {
            _authorRepository.AddFollow("Victor Duplicate", "Victor NoDupes");
        });

        Assert.Equal("TooManyFollows", exception.Message);

        int followers = _authorRepository.GetFollowerCount(authorEmail); //TODO: change to username
        Assert.True(2 > followers);
    }

    [Fact]
    public void BlockTest()
    {
        IAuthorService _authorService = new AuthorService(_authorRepository);
        var usr0 = _authorService.GetOrCreateAuthor("Testusr0", "testusr0@gmail.com");
        var usr1 = _authorService.GetOrCreateAuthor("Testusr1", "testusr1@gmail.com");

        _authorService.CreateBlock(usr0.Name, usr1.Name);

        Assert.True(_authorService.IsBlocked(usr0.Name, usr1.Name));

    }
    //Test via service
    [Fact]
    public void LikeTest()
    {
        IAuthorService _authorService = new AuthorService(_authorRepository);
        ICheepService _cheepService = new CheepService(_cheepRepository, _authorRepository);

        string test1Name = "Test";
        string test1Email = "test1@gmail.com";
        string test2Name = "Test2";
        string test2Email = "test2@gmail.com";
        _authorService.AddAuthor(test1Name, test1Email);
        _authorService.AddAuthor(test2Name, test2Email);

        _cheepService.AddCheep(test2Name, "testMsg");

        var id = _cheepService.GetCheepsFromAuthor(test2Name);
        _cheepService.CreateLike(test1Name, id[0].CheepId);

        Assert.True(_cheepService.IsLiked(test1Name, id[0].CheepId));

    }
}