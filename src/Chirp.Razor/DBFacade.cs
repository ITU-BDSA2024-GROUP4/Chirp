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

    public List<CheepViewModel> ParseCheeps(SqliteDataReader reader)
    {
        List<CheepViewModel> cheeps = new();

        while (reader.Read())
        {
            IDataRecord dataRecord = reader;
            string username = (string)dataRecord["username"];
            string text = (string)dataRecord["text"];
            long pub_date = (long)dataRecord["pub_date"];
            cheeps.Add(new CheepViewModel(username, text,
                CheepService.UnixTimeStampToDateTimeString(pub_date)));
        }

        return cheeps;
    }

    public List<CheepViewModel> SQLGetCheeps(SqliteCommand command)
    {
        using (SqliteConnection connection = new($"Data Source={_sqlDBFilePath}"))
        {
            connection.Open();

            using SqliteDataReader reader = command.ExecuteReader();

            return ParseCheeps(reader);
        }
    }

    public List<CheepViewModel> GetCheeps(int page)
    {
        // CheepDBContext context = new CheepDBContext(new DbContextOptions<CheepDBContext>());
        
        // // Does work!
        // var query = from UserId in context.Authors
        //             select UserId;
        
        // Console.WriteLine(query.Count());

        // foreach ( Author a in query ) {
        //     Console.WriteLine(a.UserId);
        // }

        using (CheepDBContext context = new CheepDBContext(new DbContextOptions<CheepDBContext>()))
        {
            var query = from Author in context.Authors
                        join Cheeps in context.Cheeps on Author.UserId equals Cheeps.AuthorId
                        select new { Author.UserId, Author.Email};

            foreach (var thing in query) {
                Console.WriteLine(thing);
            }

        }
        using (SqliteConnection connection = new($"Data Source={_sqlDBFilePath}"))
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT username, text, pub_date 
                                    FROM message m JOIN user u ON 
                                    m.author_id = u.user_id
                                    ORDER BY m.pub_date DESC 
                                    LIMIT @pageSize OFFSET @page;";

            command.Parameters.AddWithValue("@page", _pageSize * page);
            command.Parameters.AddWithValue("@pageSize", _pageSize);

            using SqliteDataReader reader = command.ExecuteReader();

            return ParseCheeps(reader);
        }
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        using (SqliteConnection connection = new($"Data Source={_sqlDBFilePath}"))
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT username, text, pub_date 
                                    FROM message m JOIN user u ON 
                                    m.author_id = u.user_id
                                    WHERE username = @author
                                    ORDER BY m.pub_date DESC
                                    LIMIT @pageSize OFFSET @page;";
            command.Parameters.AddWithValue("@author", author);
            command.Parameters.AddWithValue("@page", _pageSize * page);
            command.Parameters.AddWithValue("@pageSize", _pageSize);


            using SqliteDataReader reader = command.ExecuteReader();

            return ParseCheeps(reader);
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