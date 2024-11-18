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
    public List<CheepDTO> GetCheepsFromAuthor(string author, int page)
    {
        return _repository.GetCheepsFromAuthor(author, page);
    }

    public void CreateCheep(string email, string message)
    {
        Author author = _repository.GetAuthor(email)[0];
        _repository.CreateCheep(author, message);
    }
    public AuthorDTO GetAuthor(string email) {
        var authors = _repository.GetAuthor(email);
        if (authors.Count > 1) {
            return null; //Error, shouldn't be longer than 1
        }

        return new AuthorDTO
                {
                    Idenitifer = authors[0].Email
                };
    }
    public AuthorDTO GetOrCreateAuthor(string name, string email) {
        var authors = _repository.GetAuthor(email);
        if (authors.Count > 1) {
            return null; //Error, shouldn't be longer than 1
        }
        if (!authors.Any()) {
            authors.Add(_repository.CreateAuthor(name, email));
        }

        return new AuthorDTO
                {
                    Idenitifer = authors[0].Email
                };
    }
    public void CreateFollow(string username, string user, string follow)
    {
        GetOrCreateAuthor(username, user);
        _repository.CreateFollow(user, follow);
    }
    public void UnFollow(string user, string unfollow) {
        
        _repository.UnFollow(user, unfollow);
    }
    public BoolDTO IsFollowing(string user, string author) {
        return new BoolDTO
                {
                    Boolean = _repository.IsFollowing(user,author)
                };
    }

    public List<CheepDTO> GetOwnTimeline(string userEmail, int page)
    {
        List<AuthorDTO> following = repository.GetFollowers(userEmail);
        List<string> followingString = new List<string>();
        foreach (var follow in following)
        {
            followingString.Add(follow.Idenitifer);
        }
        followingString.Add(userEmail);
        return repository.GetCheepsFromAuthors(followingString, page);
    }
}