using Chirp.Razor.DataTransferClasses;
using Chirp.Razor.CheepRepository;

namespace Chirp.Razor.CheepService;

public interface ICheepService 
{
    ICheepRepository repository { get; set; }
    
    public List<CheepDTO> GetCheeps(int page);

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page);
}