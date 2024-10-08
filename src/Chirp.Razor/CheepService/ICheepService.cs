using Chirp.Razor.DataTransferClasses;

namespace Chirp.Razor.CheepService;

public interface ICheepService 
{
    DBFacade context { get; set; }
    
    public List<CheepDTO> GetCheeps(int page);

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page);
}