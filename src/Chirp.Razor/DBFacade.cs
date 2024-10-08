using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chirp.SQLite.CheepServices;

namespace Chirp.SQLite;
public class DBFacade : ChirpDBContext
{
    private readonly string _sqlDBFilePath;
    private readonly int _pageSize = 32;
    private ICheepService context;
    public DBFacade(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        context = new CheepService(options);

        if (_sqlDBFilePath == null)
        {
            _sqlDBFilePath =  "/tmp/chirp.db";
            DbInitializer.SeedDatabase(context.context);
        }
    }

    public List<CheepViewModel> GetCheeps(int page)
    {
        return context.GetCheeps(page);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        return context.GetCheepsFromAuthor(author, page);
    }
}