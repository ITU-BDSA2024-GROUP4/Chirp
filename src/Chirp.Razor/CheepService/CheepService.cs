using Microsoft.EntityFrameworkCore;
using Chirp.Razor.DataTransferClasses;

namespace Chirp.Razor.CheepService;

public class CheepService : ICheepService 
{
    private DBFacade _context;

    public DBFacade context
    { 
        get => _context;
        set => _context = value;
    }

    public CheepService(DbContextOptions<ChirpDBContext> options)
    {
        _context = new DBFacade(options);
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