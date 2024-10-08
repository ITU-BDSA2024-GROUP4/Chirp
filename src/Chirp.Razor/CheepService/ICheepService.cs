namespace Chirp.SQLite.CheepServices;

//REPO is synonymous with Service, but that name is taken. Should be changed to service when the other is deleted
public interface ICheepService 
{
    ChirpDBContext context { get; set; }
    public List<CheepViewModel> GetCheeps(int page);

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}