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

    public void CreateCheep(string email, string message)
    {
        Author author = _repository.GetAuthor(email)[0];
        _repository.CreateCheep(author, message);
    }

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public AuthorDTO? GetAuthor(string email)
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

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

        return new AuthorDTO { Name = authors[0].Name, Email = authors[0].Email };
    }

#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public AuthorDTO? GetAuthorUserName(string userName)
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    {
        var authors = _repository.GetAuthorUserName(userName);
        if (authors.Count > 1)
        {
            return null; //Error, shouldn't be longer than 1
        }

        if (authors.Count == 0)
        {
            return null; //Error, should exist
        }

        return new AuthorDTO { Name = authors[0].Name, Email = authors[0].Email };
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
        return new AuthorDTO { Name = author.Name, Email = author.Email };
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
        return _repository.GetFollowers(email);
    }

    public List<CheepDTO> GetOwnTimeline(string userEmail)
    {
        List<AuthorDTO> following = repository.GetFollowers(userEmail);
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

    public List<AuthorDTO> GetBlockedAuthors(string userEmail)
    {
        return _repository.GetBlockedAuthors(userEmail);
    }

    public int GetTotalCheeps(string email)
    {
        return _repository.GetCheepsFromAuthor(email).Count;
    }
}