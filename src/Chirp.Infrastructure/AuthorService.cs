
using Microsoft.EntityFrameworkCore;

using Chirp.Core;

namespace Chirp.Infrastructure;

public class AuthorService : IAuthorService
{
    private IAuthorRepository _repository;

    public IAuthorRepository repository
    {
        get => _repository;
        set => _repository = value;
    }

    public AuthorService(DbContextOptions<ChirpDBContext> options)
    {
        var context = new ChirpDBContext(options);
        _repository = new AuthorRepository(context);
    }
    
    public void AddAuthor(string name, string email)
    {
        _repository.AddAuthor(name, email);
    }
    
}