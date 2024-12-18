using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _context;
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
            .Take(_pageSize); // Same as SQL "LIMIT"

        return query.ToList(); // Converts IQueryable<T> to List<T>
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

        return query.ToList(); // Converts IQueryable<T> to List<T>
    }
    
    // Query
    public List<Cheep> GetCheep(string username, int cheepId)
    {
        var query = (from Cheep in _context.Cheeps
            where Cheep.Author.Name == username && Cheep.CheepId == cheepId
            select Cheep);
        return query.ToList();
    }

    // Command
    public void RemoveCheep(Cheep cheep)
    {
        _context.Cheeps.Remove(cheep);
        _context.SaveChanges();
    }

    // Query
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
            .Take(_pageSize); // Same as SQL "LIMIT"

        return query.ToList(); // Converts IQueryable<T> to List<T>
    }

    // Query
        // Only used for testing
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

    // Command(s)
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

    // Query
    public List<CheepDTO> GetCheepsFromAuthorsPage(List<string> authors, int page)
    {
        var query = (from Author in _context.Authors
                     join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                     orderby Cheeps.TimeStamp descending
                     where authors.Contains(Author.Name)
                     select new CheepDTO
                     {
                         Author = Author.Name,
                         Email = Author.Email,
                         Message = Cheeps.Text,
                         TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds(),
                         CheepId = Cheeps.CheepId
                     })
            .Skip(_pageSize * page) // Same as SQL "OFFSET
            .Take(_pageSize); // Same as SQL "LIMIT"
        return query.ToList();
    }

    // Query
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
    public bool IsLiked(string username, int CheepId)
    {
        var query = (from Likes in _context.Likes
                     where Likes.User.Name == username && Likes.cheep.CheepId == CheepId
                     select Likes).Any();

        return query;
    }
    
    // Query
    public List<Likes> GetLike(string username, int cheepId)
    {
        var query = (from Likes in _context.Likes
                     where Likes.User.Name == username && Likes.cheep.CheepId == cheepId
                     select Likes);
        return query.ToList();
    }
    
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
    public int TotalLikeCountUser(string username)
    {
        var query = (from Cheep in _context.Cheeps
                     join Likes in _context.Likes on Cheep.CheepId equals Likes.cheep.CheepId
                     where Cheep.Author.Name == username
                     select Likes).Count();
        return query;
    }

    // Query
    public int AmountOfCheeps()
    {
        var query = (from Cheep in _context.Cheeps
                select 1).Count();
        return query;
    }

    // TODO: Query and command
    public void UnBlock(string username, string blockUsername)
    {
        var query = (from Blocked in _context.Blocked
                     where username == Blocked.User.Name && blockUsername == Blocked.BlockedUser.Name
                     select Blocked);

        foreach (var block in query)
        {
            _context.Blocked.Remove(block);
        }

        _context.SaveChanges();
    }

    // Query
    // TODO: Why is username even parsed here?
    public bool UserBlockedSomeone(string username)
    {
        var query = (from Blocked in _context.Blocked
                     where Blocked.User.Name != null
                     select Blocked).Count();
        return query > 0;
    }

    // Query
    public List<CheepDTO> GetCheepsNotBlocked(string username)
    {
        var query = (from Author in _context.Authors
                     join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                     where !_context.Blocked.Any(b => b.User.Name == username && b.BlockedUser.Name == Author.Name)
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
    public List<CheepDTO> GetLiked(string username)
    {
        var query = (from Cheep in _context.Cheeps
                     join Likes in _context.Likes on Cheep.CheepId equals Likes.cheep.CheepId
                     orderby Cheep.TimeStamp descending
                     where Likes.User.Name == username
                     select new CheepDTO
                     {
                         CheepId = Cheep.CheepId,
                         TimeStamp = ((DateTimeOffset)Cheep.TimeStamp).ToUnixTimeSeconds(),
                         Author = Cheep.Author.Name,
                         Message = Cheep.Text
                     });

        return query.ToList();
    }

    // Query
    public int CheepCount()
    {
        return (from Cheep in _context.Cheeps
                select 1).Count();
    }
}
