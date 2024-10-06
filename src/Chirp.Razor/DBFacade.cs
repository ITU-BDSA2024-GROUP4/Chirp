using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.SQLite;

public class ChirpDBContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=/tmp/chirp.db");
    
    private readonly DbContextOptions<ChirpDBContext> _options;

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        _options = options;

        Database.Migrate();
    }
}
// If any changes are made to the "schema" then you need to run following commands to update the migration
// 1: dotnet ef migrations add RemovePasswordHashColumn - where "RemovePasswordHashColumn" is what has happend 
// 2: dotnet ef database update
public class Author
{
    [Key]
    public int AuthorId { get; set;}

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    // [Required]
    //public string PasswordHash { get; set; }

    [Required]
    public List<Cheep> Cheeps { get; set; }
}

public class Cheep
{
    [Key]
    public int CheepId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public Author Author { get; set; }

    [Required]
    public string Text { get; set; } 

    [Required]
    public System.DateTime TimeStamp { get; set; }
}

public class DBFacade : ICheepService
{
    private readonly string _sqlDBFilePath;
    private readonly int _pageSize = 32;
    private ChirpDBContext context = new ChirpDBContext(new DbContextOptions<ChirpDBContext>());
    public DBFacade()
    {
        _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        if (_sqlDBFilePath == null)
        {
            _sqlDBFilePath =  "/tmp/chirp.db";
            DbInitializer.SeedDatabase(context);
        }
    }

    public List<CheepViewModel> GetCheeps(int page)
    {
        var query = (from Author in context.Authors
                    join Cheeps in context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    select new CheepViewModel (
                        Author.Name, 
                        Cheeps.Text, 
                        ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds().ToString()
                    ))
                    .Skip(_pageSize * page) // Same as SQL "OFFSET
                    .Take(_pageSize);       // Same as SQL "LIMIT"
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
         using (ChirpDBContext context = new ChirpDBContext(new DbContextOptions<ChirpDBContext>()))
        {
            /* var query = (from Author in context.Authors
                        join Cheeps in context.Cheeps on Author.UserId equals Cheeps.AuthorId 
                        orderby Cheeps.TimeStamp descending
                        where Author.Name == author //Copied from previous SQL but is bad SQL, since name is not unique. Should use UserId
                        select new CheepViewModel (
                            Author.Name, 
                            Cheeps.Text, 
                            CheepService.UnixTimeStampToDateTimeString(Cheeps.TimeStamp)
                        ))
                        .Skip(_pageSize * page) // Same as SQL "OFFSET
                        .Take(_pageSize);       // Same as SQL "LIMIT"
            
            return query.ToList(); */
            return new List<CheepViewModel>();
        }
    }
}