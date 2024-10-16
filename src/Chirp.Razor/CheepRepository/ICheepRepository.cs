using Chirp.Razor.DataTransferClasses;

namespace Chirp.Razor.CheepRepository;

public interface ICheepRepository {
    public List<CheepDTO> GetCheeps(int page);

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page);
    
    public List<CheepDTO> GetCheepsFromAuthorEmail(string email, int page);

    public void CreateAuthor(string name, string email);
} 

