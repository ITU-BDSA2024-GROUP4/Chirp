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

    public void CreateCheep(Author author, string message)
    {
        _repository.CreateCheep(author, message);
    }
    
    public Author GetOrCreateAuthor(string name, string email) {
        var author = _repository.GetAuthor(email);

        if (!author.Any()) {
            author.Add(_repository.CreateAuthor(name, email));
        }

        return author[0];
    }
}