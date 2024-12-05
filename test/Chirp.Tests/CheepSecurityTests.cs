using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using SQLitePCL;

namespace Chirp.Tests;

public class CheepSecurityTests : IAsyncLifetime
{
    
    private SqliteConnection _connection = null!;
    private ICheepService _cheepService = null!;
    private IAuthorService _authorService = null!;
    
    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection);

        var _context = new ChirpDBContext(builder.Options);
        await _context.Database.EnsureCreatedAsync();
        
        //var _cheepRepository = new CheepRepository(_context);
        var _authorRepository = new AuthorRepository(_context);
        
        //_cheepService = new CheepService(_cheepRepository, _authorRepository);
        _authorService = new AuthorService(_authorRepository);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
    
    [Fact]
    public void Cheep_SQL_InjectionTest()
    {
        // Arrange
        string author = _authorService.GetOrCreateAuthor("Hackerman", "hacker@hacker.hacker").Email;
        string injection = ";DROP TABLE IF EXISTS Cheeps;-- ";
        
        // Act
        _authorService.AddAuthor(author, injection);
        
        var cheeps = _cheepService.GetCheeps(0);
        
        // Assert
        Assert.Equal(injection, cheeps[0].Message);
    }
    
}