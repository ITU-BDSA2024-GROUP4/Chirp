using System.Data;
using System.Diagnostics;

using Microsoft.Data.Sqlite;

namespace Chirp.SQLite;

public class DBFacade : ICheepService
{
    private readonly string sqlDBFilePath;

    public DBFacade()
    {
        sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        if (sqlDBFilePath == null)
        {
            sqlDBFilePath = Path.Combine(Path.GetTempPath(), "chirp.db");
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
        using (SqliteConnection connection = new($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            using SqliteDataReader reader = command.ExecuteReader();

            return ParseCheeps(reader);
        }
    }

    public List<CheepViewModel> GetCheeps()
    {
        using (SqliteConnection connection = new($"Data Source={sqlDBFilePath}"))
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
        using (SqliteConnection connection = new($"Data Source={sqlDBFilePath}"))
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
            Process.Start("../../../../../scripts/initDB.sh");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}