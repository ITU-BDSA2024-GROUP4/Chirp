using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Chirp.Razor.CheepRepository;

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

// " = null!;" dont gives warnings, since it says that the value is not null at runtime

public class Author
{
    [Key]
    public int AuthorId { get; set;}

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public List<Cheep> Cheeps { get; set; }  = null!;
}

// Look above Author class before making any changes

public class Cheep
{
    [Key]
    public int CheepId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public Author Author { get; set; } = null!;

    [Required]
    public string Text { get; set; } = null!;

    [Required]
    public System.DateTime TimeStamp { get; set; }
}