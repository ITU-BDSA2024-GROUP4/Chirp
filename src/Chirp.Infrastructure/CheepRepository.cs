using System.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _context;
    private readonly int _pageSize = 32;
    
    public CheepRepository(ChirpDBContext context)
    {   
        _context = context;
        DbInitializer.SeedDatabase(_context);
        
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

    //Command
    public Author CreateAuthor(string name, string email)
    {
        Author author = new Author()
        {
            AuthorId = _context.Authors.Count() + 1,
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        
        _context.Authors.Add(author);
        
        _context.SaveChanges();
        
        return author;
    }

    //Command
    public Cheep CreateCheep(Author author, string text)
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
    

    public List<Author> GetAuthor(string email) {
        var query = (from Author in _context.Authors
                    where Author.Email == email
                    select Author);

        return query.ToList();
    }

    public List<Author> GetAuthorUserName(string userName)
    {
        var query = (from Author in _context.Authors
            where Author.Name == userName
            select Author);

        return query.ToList();
    }
    
    public void CreateFollow(string user, string following)
    {
        Author AuthorUser = GetAuthor(user)[0];
        Author AuthorFollowing = GetAuthor(following)[0];
        
        bool alreadyFollowing = _context.Following.Any(f =>
            f.User.AuthorId == AuthorUser.AuthorId &&
            f.Following.AuthorId == AuthorFollowing.AuthorId);

        if (alreadyFollowing)
        {
            Console.WriteLine("FUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUCK");
            throw new ApplicationException("TooManyFollows");
        }
        
        Follows follows = new Follows() { User = AuthorUser, Following = AuthorFollowing };
        
        var validationResults = new List<ValidationResult>();
        var valContext = new ValidationContext(follows);
        if (!Validator.TryValidateObject(follows, valContext, validationResults, true))
        {
            throw new ValidationException("Cheep validation failed: " + string.Join(", ", validationResults));
        }
        
        _context.Following.Add(follows);
        _context.SaveChanges();
    }

    public void UnFollow(string user, string unfollowing)
    {
        Author AuthorUser = GetAuthor(user)[0];
        Author AuthorUnfollowing = GetAuthor(unfollowing)[0];

        var query = (from Follows in _context.Following
            where Follows.User.AuthorId == AuthorUser.AuthorId && Follows.Following.AuthorId == AuthorUnfollowing.AuthorId
            select Follows);
        foreach (var follow in query)
        {
            _context.Following.Remove(follow);
        }
        _context.SaveChanges();
    }
    public bool IsFollowing(string email, string authorEmail)
    {
        List<Author> AuthorUserList = GetAuthor(email);
        List<Author> AuthorAuthorList = GetAuthor(authorEmail);
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

    public bool IsFollowingUserName(string username, string author)
    {
        List<Author> AuthorUserList = GetAuthorUserName(username);
        List<Author> AuthorAuthorList = GetAuthor(author);
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

    public List<AuthorDTO> GetFollowers(string email)
    {
        var query = (from Follows in _context.Following
            where Follows.User.Email == email
            select new AuthorDTO { Name = Follows.Following.Name, Email = Follows.Following.Email, });
        return query.ToList();
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
    
    public void CreateLike(string user, int CheepId)
    {
        Author AuthorUser = GetAuthor(user)[0];
        Cheep ThisCheep = GetCheepFromId(CheepId);
        Likes likes = new Likes() { User = AuthorUser, cheep = ThisCheep };
        
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

    public void CreateBlock(string userEmail, string blockemail)
    {
        Author AuthorUser = GetAuthor(userEmail)[0];
        Author AuthorBlocking = GetAuthor(blockemail)[0];
        
        Blocked blocked = new Blocked() { User = AuthorUser, BlockedUser = AuthorBlocking };
        
        var validationResults = new List<ValidationResult>();
        var valContext = new ValidationContext(blocked);
        if (!Validator.TryValidateObject(blocked, valContext, validationResults, true))
        {
            throw new ValidationException("Cheep validation failed: " + string.Join(", ", validationResults));
        }
        
        _context.Blocked.Add(blocked);
        _context.SaveChanges();
        Console.WriteLine("Blocked");
    }

    public bool IsBlocked(string userEmail, string blockEmail)
    {
        //Author AuthorUser = GetAuthor(userEmail)[0];
        //Author AuthorBlocking = GetAuthor(blockEmail)[0];
        var query = (from Blocked in _context.Blocked
            where Blocked.User.Email == userEmail && Blocked.BlockedUser.Email == blockEmail
            select Blocked).Count();
        Console.WriteLine("Blocked = " + (query > 0));
        return query > 0;
    }

    public bool UserBlockedSomeone(string userEmail)
    {
        var query = (from Blocked in _context.Blocked
            where Blocked.User.Email != null
            select Blocked).Count();
        Console.WriteLine("User:" + userEmail + " blocked people amount = " + query);
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