using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using SQLitePCL;

namespace Chirp.Tests;

public class CheepSecurityTests : IAsyncLifetime
{
    
    private SqliteConnection _connection = null!;
    private CheepService _service = null!;
    
    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection);
        
        _service = new CheepService(builder.Options);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
    
    [Fact]
    public void Cheep_SQL_InjectionTest()
    {
        string author = _service.GetOrCreateAuthor("Hackerman", "hacker@hacker.hacker").Idenitifer;
        string injection = ";DROP TABLE IF EXISTS Cheeps;-- ";
        _service.CreateCheep(author, injection);
        
        var cheeps = _service.GetCheeps(0);
        
        Assert.Equal(injection, cheeps[0].Message);
    }
    
}