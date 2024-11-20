using System.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;

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
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
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
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
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
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
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
                    TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
                })
                .Skip(_pageSize * page)
                .Take(_pageSize);
        
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
    
    public void CreateFollow(string user, string following)
    {
        Author AuthorUser = GetAuthor(user)[0];
        Author AuthorFollowing = GetAuthor(following)[0];
        
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
    public bool IsFollowing(string user, string author)
    {
        List<Author> AuthorUserList = GetAuthor(user);
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
                    TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
                })
            .Skip(_pageSize * page) // Same as SQL "OFFSET
            .Take(_pageSize);       // Same as SQL "LIMIT"
        return query.ToList();
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
    
}