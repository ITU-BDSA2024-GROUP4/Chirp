using Microsoft.EntityFrameworkCore;

using Chirp.Core;

namespace Chirp.Infrastructure;

public class CheepService : ICheepService
{
    private ICheepRepository _repository;
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

    public CheepService(DbContextOptions<ChirpDBContext> options)
    {
        var context = new ChirpDBContext(options);
        _repository = new CheepRepository(context);
        _TEMP = new AuthorRepository(context);
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
        foreach (var cheep in _repository.GetCheepToDelete(userEmail, cheepId))
        {
            _repository.DeleteCheep(cheep);
        }
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

    public void CreateFollow(string username, string user, string follow)
    {
        /*
        _TEMP.GetOrCreateAuthor(username, user);
        _repository.CreateFollow(user, follow);
        */
    }

    public void UnFollow(string user, string unfollow)
    {
        _repository.UnFollow(user, unfollow);
    }

    public bool IsFollowing(string email, string authorEmail)
    {
        return _repository.IsFollowing(email, authorEmail);
    }

    public bool IsFollowingUserName(string username, string author)
    {
        return _repository.IsFollowingUserName(username, author);
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

    public int GetFollowerCount(string email)
    {
        return _repository.GetFollowerCount(email);
    }

    public int GetFollowerCountUserName(string username)
    {
        return _repository.GetFollowerCountUserName(username);
    }

    public int GetFollowingCount(string username)
    {
        return _repository.GetFollowingCount(username);
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

    public void ForgetMe(string email)
    {
        _repository.ForgetUser(email);
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

    public void CreateBlock(string userEmail, string blockEmail)
    {
        if (!IsBlocked(userEmail, blockEmail))
        {
            if (IsFollowing(userEmail, blockEmail))
            {
                _repository.UnFollow(userEmail, blockEmail);
            }

            _repository.CreateBlock(userEmail, blockEmail);
        }
    }

    public void UnBlock(string userEmail, string blockEmail)
    {
        _repository.UnBlock(userEmail, blockEmail);
    }

    public bool IsBlocked(string userEmail, string blockEmail)
    {
        return _repository.IsBlocked(userEmail, blockEmail);
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