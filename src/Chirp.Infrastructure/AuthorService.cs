
using Microsoft.EntityFrameworkCore;

using Chirp.Core;

namespace Chirp.Infrastructure;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _repository;

    public AuthorService(IAuthorRepository repository)
    {
        _repository = repository;
    }
    
    public void AddAuthor(string name, string email)
    {
        _repository.AddAuthor(name, email);
    }
    
    public List<AuthorDTO> GetBlockedAuthors(string userEmail)
    {
        return _repository.GetBlockedAuthors(userEmail);
    }
    #pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public AuthorDTO? GetAuthor(string username)
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).

{
        var authors = _repository.GetAuthor(username);
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
    public AuthorDTO? GetAuthorUserName(string username)
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    {
        var authors = _repository.GetAuthorUserName(username);
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

    public AuthorDTO GetOrCreateAuthor(string username, string email)
    {
        var authors = _repository.GetAuthor(username);
        if (authors.Count > 1)
        {
            throw new InvalidOperationException($"Multiple authors found for email: {email}");
        }

        if (authors.Count == 0)
        {
            _repository.AddAuthor(username, email);
            authors = _repository.GetAuthor(username);
        }

        var author = authors.First();
        return new AuthorDTO { Name = author.Name, Email = author.Email };
    }
    public bool IsFollowing(string username, string followingUsername)
    {
        return _repository.IsFollowing(username, followingUsername);
    }
    
    public void CreateFollow(string username, string user, string follow)
    {
        GetOrCreateAuthor(username, user);
        _repository.AddFollow(username, follow);
    }
    public void UnFollow(string user, string unfollow)
    {
        _repository.UnFollow(user, unfollow);
    }
    public bool IsBlocked(string userEmail, string blockEmail)
    {
        return _repository.IsBlocked(userEmail, blockEmail);
    }
    public void CreateBlock(string userEmail, string blockEmail)
    {
        string TEMPusername = "";
        string TEMPBlockusername = "";
        if (!IsBlocked(userEmail, blockEmail))
        {
            if (_repository.IsFollowing(TEMPusername, TEMPBlockusername))
            {
                _repository.UnFollow(userEmail, blockEmail);
            }

            _repository.CreateBlock(userEmail, blockEmail);
        }
    }
    
    public void ForgetMe(string email)
    {
        _repository.ForgetUser(email);
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
    public bool IsFollowingUserName(string username, string followingUsername)
    {
        return _repository.IsFollowingUserName(username, followingUsername);
    }
}