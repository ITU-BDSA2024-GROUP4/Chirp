namespace Chirp.SQLite.CheepRepos;

//REPO is synonymous with Service, but that name is taken. Should be changed to service when the other is deleted
public interface ICheepRepo 
{
    ChirpDBContext context { get; set; }
    public List<CheepViewModel> GetCheeps(int page);

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}