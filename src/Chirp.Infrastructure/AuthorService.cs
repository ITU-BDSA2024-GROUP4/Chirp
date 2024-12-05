
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

    public AuthorDTO GetOrCreateAuthor(string name, string email)
    {
        var authors = _repository.GetAuthor(email);
        if (authors.Count > 1)
        {
            throw new InvalidOperationException($"Multiple authors found for email: {email}");
        }

        if (authors.Count == 0)
        {
            _repository.AddAuthor(name, email);
            authors = _repository.GetAuthor(email);
        }

        var author = authors.First();
        return new AuthorDTO { Name = author.Name, Email = author.Email };
    }
    
}