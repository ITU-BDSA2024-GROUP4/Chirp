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

    public List<CheepViewModel> GetCheeps()
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        
        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM message ORDER by message.pub_date desc";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dataRecord = (IDataRecord)reader;
                
                var author  = (string)(dataRecord["author"]);
                var text = (string)(dataRecord["text"]);
                var pub_date = (string)(dataRecord["pub_date"]);
                
                cheeps.Append(new CheepViewModel(author, text, pub_date));
            }
        }
        
        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        return cheeps;
    }
}