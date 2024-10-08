using System.Data;
using Microsoft.EntityFrameworkCore;
using DataTransferClasses;

namespace Chirp.SQLite;
public class DBFacade : ChirpDBContext
{
    private readonly ChirpDBContext _context;
    private readonly int _pageSize = 32;
    
    private readonly string _sqlDBFilePath;
    public DBFacade(DbContextOptions<ChirpDBContext> options) : base(options)
    {   
        _context = new ChirpDBContext(options);

        Console.WriteLine($"Text file path not found, using default path: {_sqlDBFilePath}");

        DbInitializer.SeedDatabase(this);
    }

    public List<CheepDTO> GetCheeps(int page)
    {
        Console.WriteLine($"{_context} authors in database");
        
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