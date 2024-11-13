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
        
        _repository = new Chirp.Infrastructure.CheepRepository(context);
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
    
    public CheepDTO GetOrCreateAuthor(string name, string email) {
        var authors = _repository.GetAuthor(email);
        if (authors.Count > 1) {
            return null; //Error, shouldn't be longer than 1
        }
        if (!authors.Any()) {
            authors.Add(_repository.CreateAuthor(name, email));
        }

        return new CheepDTO
                {
                    Author = authors[0].Email,
                    Message = null,
                    TimeStamp = 0L
                };
    }
}