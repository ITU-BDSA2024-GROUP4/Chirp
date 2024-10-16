using System.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Razor.DataTransferClasses;

namespace Chirp.Razor.CheepRepository;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _context;
    private readonly int _pageSize = 32;
    
    public CheepRepository(ChirpDBContext context)
    {   
        _context = context;
        
        DbInitializer.SeedDatabase(_context);
    }

    public List<CheepDTO> GetCheeps(int page)
    {
        var query = (from Author in _context.Authors
                    join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                    orderby Cheeps.TimeStamp descending
                    select new CheepDTO
                    {
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
                    select new CheepDTO
                    {
                        Author = Author.Name, 
                        Message = Cheeps.Text, 
                        TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
                    })
                    .Skip(_pageSize * page) // Same as SQL "OFFSET
                    .Take(_pageSize);       // Same as SQL "LIMIT"
        
        return query.ToList(); //Converts IQueryable<T> to List<T>
    }

    public List<CheepDTO> GetCheepsFromAuthorEmail(string email, int page)
    {
        var query = (from Author in _context.Authors
                join Cheeps in _context.Cheeps on Author.AuthorId equals Cheeps.AuthorId
                orderby Cheeps.TimeStamp descending
                where Author.Email == email
                select new CheepDTO
                {
                    Author = Author.Name,
                    Message = Cheeps.Text,
                    TimeStamp = ((DateTimeOffset)Cheeps.TimeStamp).ToUnixTimeSeconds()
                })
                .Skip(_pageSize * page)
                .Take(_pageSize);
        
        return query.ToList();
    }

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
        
        _context.Cheeps.Add(cheep);
        author.Cheeps.Add(cheep);
        
        _context.SaveChanges();
        
        return cheep;
    }
}