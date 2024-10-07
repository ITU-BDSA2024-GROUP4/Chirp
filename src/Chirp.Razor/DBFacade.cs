using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chirp.SQLite.CheepRepos;

namespace Chirp.SQLite;
public class DBFacade : ICheepService
{
    private readonly string _sqlDBFilePath;
    private readonly int _pageSize = 32;
    private ICheepRepo cheepRepo;
    public DBFacade()
    {
        _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        cheepRepo = new CheepRepo();

        if (_sqlDBFilePath == null)
        {
            _sqlDBFilePath =  "/tmp/chirp.db";
            DbInitializer.SeedDatabase(cheepRepo.context);
        }
    }

    public List<CheepViewModel> GetCheeps(int page)
    {
        return cheepRepo.GetCheeps(page);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        return cheepRepo.GetCheepsFromAuthor(author, page);
    }
}