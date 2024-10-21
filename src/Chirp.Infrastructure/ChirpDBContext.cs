using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using Chirp.Core;

namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }
    private readonly DbContextOptions<ChirpDBContext> _options;

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        _options = options;
        Database.EnsureCreated();
    }
}

// If any changes are made to the "schema" then you need to run following commands to update the migration
// 1: dotnet ef migrations add RemovePasswordHashColumn - where "RemovePasswordHashColumn" is what has happend, can be any string
// 2: dotnet ef database update