using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chirp.SQLite.CheepRepos;
using Chirp.SQLite.CheepClient;

namespace Chirp.SQLite;
public class DBFacade : ICheepService
{
    private readonly string _sqlDBFilePath;
    private readonly int _pageSize = 32;
    private CheepClient.CheepClient client;
    public DBFacade()
    {
        _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        CheepInjection injection = new CheepInjection();
        client = injection.ResolveClient();

        if (_sqlDBFilePath == null)
        {
            _sqlDBFilePath =  "/tmp/chirp.db";
            DbInitializer.SeedDatabase(client.GetContext());
        }
    }

    public List<CheepViewModel> GetCheeps(int page)
    {
        return client.GetCheeps(page);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        return client.GetCheepsFromAuthor(author, page);
    }
}