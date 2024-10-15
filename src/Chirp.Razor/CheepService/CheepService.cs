using Microsoft.EntityFrameworkCore;
using Chirp.Razor.DataTransferClasses;
using Chirp.Razor.CheepRepository;

namespace Chirp.Razor.CheepService;

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
        _repository = new Chirp.Razor.CheepRepository.CheepRepository(options);
    }

    public List<CheepDTO> GetCheeps(int page) 
    {
        return _repository.GetCheeps(page);
    }
    public List<CheepDTO> GetCheepsFromAuthor(string author, int page)
    {
        return _repository.GetCheepsFromAuthor(author, page);
    }
}