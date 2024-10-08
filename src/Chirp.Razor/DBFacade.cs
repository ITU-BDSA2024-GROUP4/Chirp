using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chirp.SQLite.CheepServices;
using DataTransferClasses;

namespace Chirp.SQLite;
public class DBFacade : ChirpDBContext
{
    private readonly string _sqlDBFilePath;
    private readonly int _pageSize = 32;
    private ICheepService service;
    public DBFacade(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        service = new CheepService(this);

        if (_sqlDBFilePath == null)
        {
            _sqlDBFilePath =  "/tmp/chirp.db"; //Shoukd propably be imported from the json so it always is the same
            DbInitializer.SeedDatabase(service.context);
        }
    }

    public List<CheepDTO> GetCheeps(int page)
    {
        return service.GetCheeps(page);
    }

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page)
    {
        return service.GetCheepsFromAuthor(author, page);
    }
}