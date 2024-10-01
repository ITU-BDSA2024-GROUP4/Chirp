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

    public List<CheepViewModel> GetCheeps()
    {
        List<CheepViewModel> cheeps = new();

        using (SqliteConnection connection = new($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT username, text, pub_date 
                                    FROM message m JOIN user u ON 
                                    m.author_id = u.user_id
                                    ORDER BY m.pub_date DESC;";

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                IDataRecord dataRecord = reader;
                string username = (string)dataRecord["username"];
                string text = (string)dataRecord["text"];
                long pub_date = (long)dataRecord["pub_date"];
                cheeps.Add(new CheepViewModel(username, text,
                    CheepService.UnixTimeStampToDateTimeString(pub_date)));
            }
        }
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        List<CheepViewModel> cheeps = new();

        using (SqliteConnection connection = new($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            SqliteCommand UidCmd = connection.CreateCommand();
            UidCmd.CommandText = @"SELECT user_id FROM user WHERE username = @author";
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM message ORDER by message.pub_date desc WHERE author_id = user_id;";

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                IDataRecord dataRecord = reader;

                string text = (string)dataRecord["text"];
                string pub_date = (string)dataRecord["pub_date"];

                cheeps.Append(new CheepViewModel(author, text, pub_date));
            }
        }

        return cheeps;
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