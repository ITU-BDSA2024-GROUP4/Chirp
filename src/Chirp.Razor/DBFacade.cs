using System.Data;
using Microsoft.EntityFrameworkCore;
using DataTransferClasses;

namespace Chirp.SQLite;
public class DBFacade : ChirpDBContext
{
    private ChirpDBContext _context;
    private readonly int _pageSize = 32;
    public ChirpDBContext context
    { 
        get => _context; 
        set => _context = value;
    }
    private readonly string _sqlDBFilePath;
    //private ICheepService service;
    public DBFacade(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        _sqlDBFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        //service = new CheepService(this);
        Console.WriteLine($"Text file path not found, using default path: {_sqlDBFilePath}");

        if (_sqlDBFilePath == null)
        {

            _sqlDBFilePath = "/tmp/chirp.db"; //Shoukd propably be imported from the json so it always is the same

        }
        
        DbInitializer.SeedDatabase(this);
    }

    public List<CheepDTO> GetCheeps(int page)
    {
        
        var query = (from Author in _context.Authors
                    join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    select new CheepDTO {
                        Author = Author.Name, 
                        Message = Cheeps.Text, 
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
                    })
                    .Skip(_pageSize * page) // Same as SQL "OFFSET
                    .Take(_pageSize);       // Same as SQL "LIMIT"
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page)
    {
        var query = (from Author in _context.Authors
                    join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    where Author.Name == author //Copied from previous SQL but is bad SQL, since name is not unique. Should use UserId
                    select new CheepDTO {
                        Author = Author.Name, 
                        Message = Cheeps.Text, 
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
                    })
                    .Skip(_pageSize * page) // Same as SQL "OFFSET
                    .Take(_pageSize);       // Same as SQL "LIMIT"
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }
}