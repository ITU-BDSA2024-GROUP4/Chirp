namespace Chirp.SQLite.CheepRepos;
public class CheepRepo : ICheepRepo 
{
    private readonly ChirpDBContext _context;
    private readonly int _pageSize = 32;
    public CheepRepo (ChirpDBContext context) 
    {
        _context = context;
    }

    public List<CheepViewModel> GetCheeps(int page) 
    {
        var query = (from Author in _context.Authors
                    join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    select new CheepViewModel (
                        Author.Name, 
                        Cheeps.Text, 
                        ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds().ToString()
                    ))
                    .Skip(_pageSize * page) // Same as SQL "OFFSET
                    .Take(_pageSize);       // Same as SQL "LIMIT"
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        var query = (from Author in _context.Authors
                    join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    where Author.Name == author //Copied from previous SQL but is bad SQL, since name is not unique. Should use UserId
                    select new CheepViewModel (
                        Author.Name, 
                        Cheeps.Text, 
                        ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds().ToString()
                    ))
                    .Skip(_pageSize * page) // Same as SQL "OFFSET
                    .Take(_pageSize);       // Same as SQL "LIMIT"
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }
}