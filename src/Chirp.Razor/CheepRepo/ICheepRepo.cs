namespace Chirp.SQLite.CheepRepos;
public interface ICheepRepo 
{
    ChirpDBContext context { get; set; }
    public List<CheepViewModel> GetCheeps(int page);

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}