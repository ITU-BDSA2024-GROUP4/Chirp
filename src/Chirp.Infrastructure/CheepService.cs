using Microsoft.EntityFrameworkCore;

using Chirp.Core;

namespace Chirp.Infrastructure;

public class CheepService : ICheepService
{
    private ICheepRepository _repository;
    private IAuthorRepository _authorRepository;
    private IAuthorRepository _TEMP;
    public IAuthorRepository TEMP
    {
        get => _TEMP;
        set => _TEMP = value;
    }

    public ICheepRepository repository
    {
        get => _repository;
        set => _repository = value;
    }

    public CheepService(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        _repository = repository;
        _authorRepository = authorRepository;
        _TEMP = _authorRepository; //IDK -\_O_/- temp
    }

    public List<CheepDTO> GetCheeps(int page)
    {
        return _repository.GetCheeps(page);
    }

    public List<CheepDTO> GetCheepsFromAuthor(string author)
    {
        return _repository.GetCheepsFromAuthor(author);
    }

    public void RemoveCheep(string username, int cheepId)
    {
        List<Cheep> cheeps = _repository.GetCheep(username, cheepId);
        if (cheeps == null || cheeps.Count > 1)
        {
            throw new ApplicationException("There are multiple cheeps with same email and cheep ID.");
        }
        _repository.RemoveCheep(cheeps[0]);
    }

    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page)
    {
        return _repository.GetCheepsFromAuthorPage(author, page);
    }

    public void AddCheep(string username, string message)
    {
        Author author = TEMP.GetAuthor(username)[0];
        
        Cheep cheep = new Cheep()
        {
            CheepId = _repository.CheepCount() + 1,
            AuthorId = author.AuthorId,
            Author = author,
            Text = message,
            TimeStamp = DateTime.Now
        };
        
        _repository.AddCheep(cheep, author);
    }

    public List<AuthorDTO> GetFollowers(string email)
    {
        return TEMP.GetFollowers(email);
    }

    public List<CheepDTO> GetOwnTimeline(string userEmail)
    {
        List<AuthorDTO> following = TEMP.GetFollowers(userEmail);
        List<string> followingString = new List<string>();
        List<CheepDTO> Cheeps = new List<CheepDTO>();
        foreach (var follow in following)
        {
            followingString.Add(follow.Email);
        }

        followingString.Add(userEmail);
        foreach (var email in followingString)
        {
            Cheeps.AddRange(_repository.GetCheepsFromAuthorEmail(email));
        }

        return Cheeps;
    }

    public List<CheepDTO> GetOwnTimelinePage(string userEmail, int page)
    {
        List<AuthorDTO> following = TEMP.GetFollowers(userEmail);
        List<string> followingString = new List<string>();
        foreach (var follow in following)
        {
            followingString.Add(follow.Email);
        }

        followingString.Add(userEmail);
        return repository.GetCheepsFromAuthorPages(followingString, page);
    }

    public void CreateLike(string user, int CheepId)
    {
        List<Author> authors = _authorRepository.TEMPgetAUTHORwithEMAIL(user);
        List<Cheep> cheeps = _repository.GetCheepFromId(CheepId);

        if (cheeps.Count != 1)
        { 
            throw new Exception("Multiple cheeps with same id");
        }
        if (authors.Count != 1)
        { 
            throw new Exception("Multiple authors with same user");
        }
        
        Author author = authors[0];
        Cheep cheep = cheeps[0];
        
        Likes likes = new Likes() { User = author, cheep = cheep };
        
        _repository.AddLike(likes);
    }

    public bool IsLiked(string username, int CheepId)
    {
        return _repository.IsLiked(username, CheepId);
    }

    public void UnLike(string username, int CheepId)
    {
        List<Likes> likes = _repository.GetLike(username, CheepId);
        if (likes.Count() != 1)
        {
            throw new Exception("Multiple likes with same user on same cheepid");
        }

        Likes like = likes[0];
        
        _repository.UnLike(like);
    }

    public int LikeCount(int CheepId)
    {
        return _repository.LikeCount(CheepId);
    }

    public int TotalLikeCountUser(string username)
    {
        return _repository.TotalLikeCountUser(username);
    }

    public int AmountOfCheeps()
    {
        return _repository.AmountOfCheeps();
    }

    public List<CheepDTO> GetLiked(string email)
    {
        return _repository.GetLiked(email);
    }

    public List<CheepDTO> GetOwnTimeline(string userEmail, int page)
    {
        throw new NotImplementedException();
    }

    public void UnBlock(string userEmail, string blockEmail)
    {
        _repository.UnBlock(userEmail, blockEmail);
    }

    public bool UserBlockedSomeone(string userEmail)
    {
        return _repository.UserBlockedSomeone(userEmail);
    }

    public List<CheepDTO> GetCheepsNotBlocked(string userEmail)
    {
        return _repository.GetCheepsNotBlocked(userEmail);
    }
    
    public int GetTotalCheeps(string email)
    {
        return _repository.GetCheepsFromAuthor(email).Count;
    }
}