using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Infrastructure;


public class ChirpDBContext : IdentityDbContext<ChirpUser>
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Follows> Following { get; set; }
    private readonly DbContextOptions<ChirpDBContext> _options;

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        _options = options;
        Database.EnsureCreated();
    }
}