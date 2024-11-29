using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class CheepService : ICheepService
{
    private ICheepRepository _repository;

    public ICheepRepository repository
    {
        get => _repository;
        set => _repository = value;
    }

    public CheepService(DbContextOptions<ChirpDBContext> options)
    {
        var context = new ChirpDBContext(options);
        _repository = new CheepRepository(context);
    }

    public List<CheepDTO> GetCheeps(int page)
    {
        return _repository.GetCheeps(page);
    }
    public List<CheepDTO> GetCheepsFromAuthor(string author)
    {
        return _repository.GetCheepsFromAuthor(author);
    }
    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page)
    {
        return _repository.GetCheepsFromAuthorPage(author, page);
    }

    public void CreateCheep(string email, string message)
    {
        Author author = _repository.GetAuthor(email)[0];
        _repository.CreateCheep(author, message);
    }
    public AuthorDTO GetAuthor(string email)
    {
        var authors = _repository.GetAuthor(email);
        if (authors.Count > 1)
        {
            return null; //Error, shouldn't be longer than 1
        }
        if (authors.Count == 0)
        {
            return null; //Error, should exist
        }

        return new AuthorDTO
        {
            Name = authors[0].Name,
            Email = authors[0].Email
        };
    }

    public void CreateAuthor(string name, string email)
    {
        _repository.CreateAuthor(name, email);
       
    }

    public AuthorDTO GetOrCreateAuthor(string name, string email)
    {
        var authors = _repository.GetAuthor(email);
        if (authors.Count > 1)
        {
            throw new InvalidOperationException($"Multiple authors found for email: {email}");
        }
        if (authors.Count == 0)
        {
            _repository.CreateAuthor(name, email);
            authors = _repository.GetAuthor(email);

            
        }

        var author = authors.First();
        return new AuthorDTO
        {
            Name = author.Name,
            Email = author.Email
        };
    }

    public void CreateFollow(string username, string user, string follow)
    {
        GetOrCreateAuthor(username, user);
        _repository.CreateFollow(user, follow);
    }
    public void UnFollow(string user, string unfollow)
    {
        _repository.UnFollow(user, unfollow);
    }
    public bool IsFollowing(string user, string author)
    {
        return _repository.IsFollowing(user, author);
    }
    public List<AuthorDTO> GetFollowers(string email)
    {
        return _repository.GetFollowers(email);
    }

    public List<CheepDTO> GetOwnTimeline(string userEmail, int page)
    {
        List<AuthorDTO> following = repository.GetFollowers(userEmail);
        List<string> followingString = new List<string>();
        foreach (var follow in following)
        {
            followingString.Add(follow.Email);
        }
        followingString.Add(userEmail);
        return repository.GetCheepsFromAuthorPages(followingString, page);
    }

    public void ForgetMe(string email)
    {
        _repository.ForgetUser(email);
        Console.WriteLine("VICTORRRR");
        Console.WriteLine("Forget User: " + email);
    }

    public void CreateLike(string user, int CheepId)
    {
        _repository.CreateLike(user, CheepId);
    }

    public bool IsLiked(string user, int CheepId)
    {
        return _repository.IsLiked(user, CheepId);
    }

    public void UnLike(string user, int CheepId)
    {
        _repository.UnLike(user, CheepId);
    }

    public int LikeCount(int CheepId)
    {
        return _repository.LikeCount(CheepId);
    }

    public List<CheepDTO> GetLiked(string email)
    {
        return _repository.GetLiked(email);
    }
    
}