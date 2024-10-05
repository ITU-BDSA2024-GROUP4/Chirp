using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.SQLite;

public class CheepDBContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=/tmp/chirp.db");
    
    private readonly DbContextOptions<CheepDBContext> _options;

    public CheepDBContext(DbContextOptions<CheepDBContext> options) : base(options)
    {
        _options = options;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().ToTable("user");
        modelBuilder.Entity<Cheep>().ToTable("message");
    }
}

public class Author
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set;}

    [Required]
    [Column("username")]
    public string Name { get; set; }

    [Required]
    [Column("email")]
    public string Email { get; set; }

    [Required]
    [Column("pw_hash")]
    public string PasswordHash { get; set; }
    //public ICollection<Cheep> Cheeps { get; set; }
}

public class Cheep
{
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; }

    [Column("author_id")]
    public int AuthorId { get; set; }

    [Column("text")]
    public string Text { get; set; } 
    
    [Column("pub_date")]
    public int TimeStamp { get; set; }
    //public Author Author { get; set; }
}

public class DBFacade : ICheepService
{
    private readonly string _sqlDBFilePath;
    private readonly int _pageSize = 32;
    public DBFacade()
    {
        _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        if (_sqlDBFilePath == null)
        {
            _sqlDBFilePath =  "/tmp/chirp.db";
            InitDB();
        }
    }

    public List<CheepViewModel> GetCheeps(int page)
    {

        using (CheepDBContext context = new CheepDBContext(new DbContextOptions<CheepDBContext>()))
        {
            var query = (from Author in context.Authors
                        join Cheeps in context.Cheeps on Author.UserId equals Cheeps.AuthorId
                        orderby Cheeps.TimeStamp descending
                        select new CheepViewModel (
                            Author.Name, 
                            Cheeps.Text, 
                            CheepService.UnixTimeStampToDateTimeString(Cheeps.TimeStamp)
                        ))
                        .Skip(_pageSize * page) // Same as SQL "OFFSET
                        .Take(_pageSize);       // Same as SQL "LIMIT"
            
            return query.ToList(); //Converts IQueryable<T> to List<T>
        }
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
         using (CheepDBContext context = new CheepDBContext(new DbContextOptions<CheepDBContext>()))
        {
            var query = (from Author in context.Authors
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
            
            return query.ToList();
        }
    }

    private void InitDB()
    {
        try
        {   
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

            var schemaReader = embeddedProvider.GetFileInfo("schema.sql").CreateReadStream();
            var dumpReader = embeddedProvider.GetFileInfo("dump.sql").CreateReadStream();

            using var schemaStreamReader = new StreamReader(schemaReader);
            using var dumpStreamReader = new StreamReader(dumpReader);

           var schemaQuery = schemaStreamReader.ReadToEnd();
            var dumpQuery = dumpStreamReader.ReadToEnd();

            using (SqliteConnection connection = new($"Data Source={_sqlDBFilePath}"))
            {
                connection.Open();

                SqliteCommand createSchema = connection.CreateCommand();
                SqliteCommand dumpSchema = connection.CreateCommand();

                createSchema.CommandText = schemaQuery;
                dumpSchema.CommandText = dumpQuery;

                int rowsCreated = createSchema.ExecuteNonQuery();
                int rowsInserted = dumpSchema.ExecuteNonQuery();

                Console.WriteLine($"{rowsCreated} rows created");
                Console.WriteLine($"{rowsInserted} rows inserted");

            }

        }
        catch (Exception e)
        {

            Console.WriteLine($"There was an error: {e}\nStacktrace: {e.StackTrace}");
        }
    }
}