using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;
using System.Reflection;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Chirp.SQLite;

public class CheepingContext : DbContext
{
    public DbSet<Author> Author { get; set; }
    public DbSet<Cheep> Cheep { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=/tmp/chirp.db");
}

public class Author
{
    public string Name { get; set; }
    public string Email { get; set; }
} 
public class Cheep
{
    public string Text { get; set; }
    public int TimeStamp { get; set; }
}



public class DBFacade : ICheepService
{
    private readonly string _sqlDBFilePath;

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

    public List<CheepViewModel> GetCheeps()
    {
        using (SqliteConnection connection = new($"Data Source={_sqlDBFilePath}"))
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT username, text, pub_date 
                                    FROM message m JOIN user u ON 
                                    m.author_id = u.user_id
                                    ORDER BY m.pub_date DESC;";

            using SqliteDataReader reader = command.ExecuteReader();

            return ParseCheeps(reader);
        }
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        using (SqliteConnection connection = new($"Data Source={_sqlDBFilePath}"))
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT username, text, pub_date 
                                    FROM message m JOIN user u ON 
                                    m.author_id = u.user_id
                                    WHERE username = @author
                                    ORDER BY m.pub_date DESC;";
            command.Parameters.AddWithValue("@author", author);

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