namespace Chirp.SQLite.CheepRepos;
public interface ICheepRepo 
{
    public List<CheepViewModel> GetCheeps(int page);

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}