using System.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _context;
    private readonly IAuthorRepository _TEMP;
    private readonly int _pageSize = 32;

    public CheepRepository(ChirpDBContext context)
    {
        _context = context;
    }

    //Query
    public List<CheepDTO> GetCheeps(int page)
    {
        var query = (from Author in _context.Authors
                    join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    select new CheepDTO
                    {
                        Author = Author.Name,
                        Email = Author.Email, 
                        Message = Cheeps.Text, 
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds(),
                        CheepId = Cheeps.CheepId
                        
                    })
                    .Skip(_pageSize * page) // Same as SQL "OFFSET
                    .Take(_pageSize);       // Same as SQL "LIMIT"
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }
    //Query
    public List<CheepDTO> GetCheepsFromAuthor(string author)
    {
        var query = (from Author in _context.Authors
                    join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    where Author.Name == author
                    select new CheepDTO
                    {
                        Author = Author.Name,
                        Email = Author.Email,  
                        Message = Cheeps.Text, 
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds(),
                        CheepId = Cheeps.CheepId
                    });
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }
    
    //Query TODO: NAME CHANGE
    public List<Cheep> GetCheepToDelete(string userEmail, int cheepId)
    {
        var query = (from Cheep in _context.Cheeps
            where Cheep.Author.Email == userEmail && Cheep.CheepId == cheepId
            select Cheep);
        return query.ToList();
    }
    //COMMAND TODO: RENAME REMOVE
    public void DeleteCheep(Cheep cheep)
    {
        _context.Cheeps.Remove(cheep);
        _context.SaveChanges();
    }
    //Query
    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page)
    {
        var query = (from Author in _context.Authors
                    join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    where Author.Name == author
                    select new CheepDTO
                    {
                        Author = Author.Name,
                        Email = Author.Email,  
                        Message = Cheeps.Text, 
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds(),
                        CheepId = Cheeps.CheepId
                    })
                    .Skip(_pageSize * page) // Same as SQL "OFFSET
                    .Take(_pageSize);       // Same as SQL "LIMIT"
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }

    //Query
    public List<CheepDTO> GetCheepsFromAuthorPageEmail(string email, int page)
    {
        var query = (from Author in _context.Authors
                join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                orderby Cheeps.TimeStamp descending
                where Author.Email == email
                select new CheepDTO
                {
                    Author = Author.Name,
                    Email = Author.Email, 
                    Message = Cheeps.Text,
                    TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds(),
                    CheepId = Cheeps.CheepId
                })
                .Skip(_pageSize * page)
                .Take(_pageSize);
        
        return query.ToList();
    }
    //QUERY TODO: Unify email and username calls to one or the other
    public List<CheepDTO> GetCheepsFromAuthorEmail(string email)
    {
        var query = (from Author in _context.Authors
                join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                orderby Cheeps.TimeStamp descending
                where Author.Email == email
                select new CheepDTO
                {
                    Author = Author.Name,
                    Email = Author.Email, 
                    Message = Cheeps.Text,
                    TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds(),
                    CheepId = Cheeps.CheepId
                });
        
        return query.ToList();
    }
    
    public Cheep AddCheep(Author author, string text)
    {
        Cheep cheep = new Cheep()
        {
            CheepId = _context.Cheeps.Count() + 1,
            AuthorId = author.AuthorId,
            Author = author,
            Text = text,
            TimeStamp = DateTime.Now
        };

        var validationResults = new List<ValidationResult>();
        var valContext = new ValidationContext(cheep);
        if (!Validator.TryValidateObject(cheep, valContext, validationResults, true))
        {
            throw new ValidationException("Cheep validation failed: " + string.Join(", ", validationResults));
        }

        _context.Cheeps.Add(cheep);
        author.Cheeps.Add(cheep);
        
        _context.SaveChanges();
        
        return cheep;
    }

    public bool IsFollowingUserName(string username, string author)
    {
        List<Author> AuthorUserList = _TEMP.GetAuthorUserName(username);
        List<Author> AuthorAuthorList = _TEMP.GetAuthor(author);
        if (AuthorUserList.Count != 1 || AuthorAuthorList.Count != 1) {
            return false;
        }
        Author AuthorUser = AuthorUserList[0];
        Author AuthorAuthor = AuthorAuthorList[0];

        var query = (from Follows in _context.Following
            where Follows.User.AuthorId == AuthorUser.AuthorId && Follows.Following.AuthorId == AuthorAuthor.AuthorId
            select Follows).Any();

        return query;
    }

    public int GetFollowerCount(string email)
    {
        var query = (from Follows in _context.Following
                where Follows.Following.Email == email
                    select Follows).Count();
        return query;

    }
    public int GetFollowerCountUserName(string username)
    {
        var query = (from Follows in _context.Following
            where Follows.Following.Name == username
            select Follows).Count();
        return query;

    }
    public int GetFollowingCount(string username)
    {
        var query = (from Follows in _context.Following
            where Follows.User.Name == username
            select Follows).Count();
        return query;

    }
    public List<CheepDTO> GetCheepsFromAuthorPages(List<string> authors, int page)
    {
        var query = (from Author in _context.Authors
                join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                orderby Cheeps.TimeStamp descending
                where authors.Contains(Author.Email)
                select new CheepDTO
                {
                    Author = Author.Name,
                    Email = Author.Email,  
                    Message = Cheeps.Text, 
                    TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds(),
                    CheepId = Cheeps.CheepId
                })
            .Skip(_pageSize * page) // Same as SQL "OFFSET
            .Take(_pageSize);       // Same as SQL "LIMIT"
        return query.ToList();
    }

    public Cheep GetCheepFromId(int cheepId)
    {
        var query = (from Cheep in _context.Cheeps
            where Cheep.CheepId == cheepId
            select Cheep);
        if (query.Count() != 1)
        {
            throw new Exception("Multiple cheeps same id or is invalid id");
        }
        return query.ToList()[0];
    }

    public void ForgetUser(string email)
    {
        var author = _context.Authors.SingleOrDefault(a => a.Email.ToLower() == email.ToLower());        if (author == null)
        {
            throw new Exception("User not found.");
        }

        
        var cheeps = _context.Cheeps.Where(c => c.AuthorId == author.AuthorId).ToList();
        _context.Cheeps.RemoveRange(cheeps);

        
        var follows = _context.Following.Where(f => f.User.AuthorId == author.AuthorId || f.Following.AuthorId == author.AuthorId).ToList();
        _context.Following.RemoveRange(follows);

        
        _context.Authors.Remove(author);

        
        _context.SaveChanges();

    }
    
    //COMMAND
    public void AddLike(Likes likes)
    {
        var validationResults = new List<ValidationResult>();
        var valContext = new ValidationContext(likes);
        if (!Validator.TryValidateObject(likes, valContext, validationResults, true))
        {
            throw new ValidationException("Cheep validation failed: " + string.Join(", ", validationResults));
        }
        
        _context.Likes.Add(likes);
        _context.SaveChanges();
    }

    public bool IsLiked(string user, int CheepId)
    {
        var query = (from Likes in _context.Likes
            where Likes.User.Email == user && Likes.cheep.CheepId == CheepId 
            select Likes).Any();

        return query;
    }
    public void UnLike(string user, int CheepId)
    {
        var query = (from Likes in _context.Likes
            where Likes.User.Email == user && Likes.cheep.CheepId == CheepId
            select Likes);
        foreach (var like in query)
        {
            _context.Likes.Remove(like);
        }
        _context.SaveChanges();
    }

    public int LikeCount(int CheepId)
    {
        var query = (from Likes in _context.Likes
            where Likes.cheep.CheepId == CheepId
                select Likes).Count();
        return query;
    }

    public int TotalLikeCountUser(string email)
    {
        var query = (from Cheep in _context.Cheeps
            join Likes in _context.Likes on Cheep.CheepId equals Likes.cheep.CheepId
            where Cheep.Author.Email == email
            select Likes).Count();
        return query;
    }
    public int AmountOfCheeps()
    {
        return _context.Cheeps.Count();
    }

    public void UnBlock(string userEmail, string blockEmail)
    {
        var query = (from Blocked in _context.Blocked
            where userEmail == Blocked.User.Email && blockEmail == Blocked.BlockedUser.Email
            select Blocked);
        
        foreach (var block in query)
        {
            _context.Blocked.Remove(block);
        }
        _context.SaveChanges();
        
    }

    public bool UserBlockedSomeone(string userEmail)
    {
        var query = (from Blocked in _context.Blocked
            where Blocked.User.Email != null
            select Blocked).Count();
        return query > 0;
    }

    public List<CheepDTO> GetCheepsNotBlocked(string userEmail)
    {
        var query = (from Author in _context.Authors
            join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
            where !_context.Blocked.Any(b => b.User.Email == userEmail && b.BlockedUser.Email == Author.Email)
            orderby Cheeps.TimeStamp descending
            select new CheepDTO
            {
                Author = Author.Name,
                Email = Author.Email,
                Message = Cheeps.Text,
                TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds(),
                CheepId = Cheeps.CheepId
            });

        return query.ToList();
    }

    public List<CheepDTO> GetLiked(string email)
    {

        var query = (from Cheep in _context.Cheeps
            join Likes in _context.Likes on Cheep.CheepId equals Likes.cheep.CheepId
            orderby Cheep.TimeStamp descending
            where Likes.User.Email == email
            select new CheepDTO
            {
                CheepId = Cheep.CheepId,
                TimeStamp = ((DateTimeOffset)Cheep.TimeStamp).ToUnixTimeSeconds(),
                Author = Cheep.Author.Name,
                Message = Cheep.Text
            });
    //where authors.Contains(Author.Email)

    return query.ToList();
    }
}
