using Chirp.Core;

namespace Chirp.Infrastructure;


public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _context;

    public AuthorRepository(ChirpDBContext context)
    {
        _context = context;
    }
    
    //COMMAND
    public Author AddAuthor(string username, string email)
    {
        Author author = new Author()
        {
            AuthorId = _context.Authors.Count() + 1,
            Name = username,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        
        _context.Authors.Add(author);
        
        _context.SaveChanges();
        
        return author;
    }
    
    //QUERY
    public List<Author> GetAuthor(string email) {
        var query = (from Author in _context.Authors
            where Author.Email == email
            select Author);

        return query.ToList();
    }
    
    //COMMAND
    public List<Author> GetAuthorUserName(string userName)
    {
        var query = (from Author in _context.Authors
            where Author.Name == userName
            select Author);

        return query.ToList();
    }
    
    //QUERY
    public List<AuthorDTO> GetFollowers(string email)
    {
        var query = (from Follows in _context.Following
            where Follows.User.Email == email
            select new AuthorDTO { Name = Follows.Following.Name, Email = Follows.Following.Email, });
        return query.ToList();
    }
    
    //QUERY
    public List<AuthorDTO> GetBlockedAuthors(string userEmail)
    {
        var query = (from Author in _context.Authors
            join Blocked in _context.Blocked on Author.Email equals Blocked.User.Email
            select new AuthorDTO
            {
                Name = Blocked.BlockedUser.Name, Email = Blocked.BlockedUser.Email,
            });
        return query.ToList();
    }
}