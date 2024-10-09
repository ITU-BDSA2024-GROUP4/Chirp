using Microsoft.EntityFrameworkCore;
using Chirp.Razor.DataTransferClasses;
using Chirp.Razor.CheepRepository;

namespace Chirp.Razor.CheepService;

public class CheepService : ICheepService 
{
    private ICheepRepository _context;

    public ICheepRepository context
    { 
        get => _context;
        set => _context = value;
    }

    public CheepService(DbContextOptions<ChirpDBContext> options)
    {
        _context = new Chirp.Razor.CheepRepository.CheepRepository(options);
    }

    public List<CheepDTO> GetCheeps(int page) 
    {
        return _context.GetCheeps(page);
    }
    public List<CheepDTO> GetCheepsFromAuthor(string author, int page)
    {
        return _context.GetCheepsFromAuthor(author, page);
    }
}