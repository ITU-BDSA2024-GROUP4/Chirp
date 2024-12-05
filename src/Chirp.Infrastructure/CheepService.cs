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

    public void DeleteCheep(string userEmail, int cheepId)
    {
        List<Cheep> cheeps = _repository.GetCheep(userEmail, cheepId);
        if (cheeps == null || cheeps.Count > 1)
        {
            throw new ApplicationException("There are multiple cheeps with same email and cheep ID.");
        }
        _repository.DeleteCheep(cheeps[0]);
    }

    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page)
    {
        return _repository.GetCheepsFromAuthorPage(author, page);
    }

    public void AddCheep(string email, string message)
    {
        Author author = TEMP.GetAuthor(email)[0];
        _repository.AddCheep(author, message);
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
        Author author = _authorRepository.GetAuthor(user)[0];
        Cheep cheep = _repository.GetCheepFromId(CheepId);
        
        Likes likes = new Likes() { User = author, cheep = cheep };
        
        _repository.AddLike(likes);
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