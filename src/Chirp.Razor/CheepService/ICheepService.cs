namespace Chirp.SQLite.CheepServices;
using DataTransferClasses;

//REPO is synonymous with Service, but that name is taken. Should be changed to service when the other is deleted
public interface ICheepService 
{
    DBFacade context { get; set; }
    public List<CheepDTO> GetCheeps(int page);

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page);
}