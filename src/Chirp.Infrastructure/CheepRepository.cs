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

    // Query
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
    // Query
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
    
    // Query TODO: NAME CHANGE
    public List<Cheep> GetCheep(string userEmail, int cheepId)
    {
        var query = (from Cheep in _context.Cheeps
            where Cheep.Author.Email == userEmail && Cheep.CheepId == cheepId
            select Cheep);
        return query.ToList();
    }
    //COMMAND
    public void RemoveCheep(Cheep cheep)
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
    
    // Commands
    public Cheep AddCheep(Cheep cheep, Author author)
    {
        var validationResults = new List<ValidationResult>();
        var valContext = new ValidationContext(cheep);
        if (!Validator.TryValidateObject(cheep, valContext, validationResults, true))
        {
            throw new ValidationException("Cheep validation failed: " + string.Join(", ", validationResults));
        }

        _context.Cheeps.Add(cheep);
        author.Cheeps.Add(cheep);
        
        _context.SaveChanges();
        
        return cheep; // Returns so it made easier to test
    }
    
    // Query TODO: simplify names/merge methods
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

    // Query TODO: move logic to service
    public List<Cheep> GetCheepFromId(int cheepId)
    {
        var query = (from Cheep in _context.Cheeps
            where Cheep.CheepId == cheepId
            select Cheep);
       
        return query.ToList();
    }
    
    // Command
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

    // Query
    public bool IsLiked(string user, int CheepId)
    {
        var query = (from Likes in _context.Likes
            where Likes.User.Email == user && Likes.cheep.CheepId == CheepId 
            select Likes).Any();

        return query;
    }

    public List<Likes> GetLike(string user, int cheepId)
    {
        var query = (from Likes in _context.Likes
            where Likes.User.Email == user && Likes.cheep.CheepId == cheepId
            select Likes);
        return query.ToList();
    }
    // TODO: Query and Command
    // Command
    public void UnLike(Likes like)
    {
        _context.Likes.Remove(like);
        _context.SaveChanges();
    }

    // Query
    public int LikeCount(int CheepId)
    {
        var query = (from Likes in _context.Likes
            where Likes.cheep.CheepId == CheepId
                select Likes).Count();
        return query;
    }

    // Query TODO: move to author repo
    public int TotalLikeCountUser(string email)
    {
        var query = (from Cheep in _context.Cheeps
            join Likes in _context.Likes on Cheep.CheepId equals Likes.cheep.CheepId
            where Cheep.Author.Email == email
            select Likes).Count();
        return query;
    }
    // Query ?? maybe change to linq query ??
    public int AmountOfCheeps()
    {
        return _context.Cheeps.Count();
    }

    // TODO: Query and command
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

    // Query
    public bool UserBlockedSomeone(string userEmail)
    {
        var query = (from Blocked in _context.Blocked
            where Blocked.User.Email != null
            select Blocked).Count();
        return query > 0;
    }

    // Query
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

    // Query
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
    
    // Query
    public int CheepCount()
    {
        return (from Cheep in _context.Cheeps
                select 1).Count();
    }
}
