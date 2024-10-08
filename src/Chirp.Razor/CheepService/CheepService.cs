using Microsoft.EntityFrameworkCore;
using DataTransferClasses;

namespace Chirp.SQLite.CheepServices;
//REPO is synonymous with Service, but that name is taken. Should be changed to service when the other is deleted
public class CheepService : ICheepService 
{
    public ChirpDBContext _context;
    private readonly int _pageSize = 32;
    public ChirpDBContext context
    { 
        get => _context; 
        set => _context = value;
    }
    public CheepService(ChirpDBContext context)
    {
        _context = context;
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