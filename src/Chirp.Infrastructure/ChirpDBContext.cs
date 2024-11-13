using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

// If any changes are made to the "schema" then you need to run following commands to update the migration
// 1: Be in Chirp/src directory
// 2: dotnet ef migrations add "name of change" --project Chirp.Infrastructure --startup-project Chirp.Web
// 2.5 the "name of change" should not be a in " when typing command, could be: intialCreate 

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