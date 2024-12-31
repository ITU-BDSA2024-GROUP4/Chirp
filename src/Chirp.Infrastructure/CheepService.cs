using Chirp.Core;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class CheepService : ICheepService
{
    private ICheepRepository _repository;
    private readonly IAuthorRepository _authorRepository;

    public ICheepRepository repository
    {
        get => _repository;
        set => _repository = value;
    }

    public CheepService(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        _repository = repository;
        _authorRepository = authorRepository;
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
        Author author = _authorRepository.GetAuthor(username)[0];

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

    public List<CheepDTO> GetOwnTimeline(string username)
    {
        List<AuthorDTO> following = _authorRepository.GetFollowers(username);
        List<string> followingString = new List<string>();
        List<CheepDTO> Cheeps = new List<CheepDTO>();
        foreach (var follow in following)
        {
            followingString.Add(follow.Name);
        }

        followingString.Add(username);
        foreach (var usr in followingString)
        {
            Cheeps.AddRange(_repository.GetCheepsFromAuthor(usr));
        }

        return Cheeps;
    }

    public List<CheepDTO> GetOwnTimelinePage(string username, int page)
    {
        List<AuthorDTO> following = _authorRepository.GetFollowers(username);
        List<string> followingString = new List<string>();
        foreach (var follow in following)
        {
            followingString.Add(follow.Name);
        }

        followingString.Add(username);
        return repository.GetCheepsFromAuthorsPage(followingString, page);
    }

    public void CreateLike(string username, int cheepId)
    {
        List<Author> authors = _authorRepository.GetAuthor(username);
        List<Cheep> cheeps = _repository.GetCheepFromId(cheepId);

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

    public List<CheepDTO> GetLiked(string username)
    {
        return _repository.GetLiked(username);
    }

    public List<CheepDTO> GetOwnTimeline(string userEmail, int page)
    {
        throw new NotImplementedException();
    }

    public void UnBlock(string username, string blockUsername)
    {
        _repository.UnBlock(username, blockUsername);
    }

    public bool UserBlockedSomeone(string username)
    {
        return _repository.UserBlockedSomeone(username);
    }

    public List<CheepDTO> GetCheepsNotBlocked(string username)
    {
        return _repository.GetCheepsNotBlocked(username);
    }

    public int GetTotalCheeps(string email)
    {
        return _repository.GetCheepsFromAuthor(email).Count;
    }
}