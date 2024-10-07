using Chirp.SQLite.CheepRepos;
using Microsoft.EntityFrameworkCore;

namespace Chirp.SQLite.CheepClient;

public class CheepClient
{
    ICheepRepo _cr;
    public CheepClient(ICheepRepo cr) 
    {
        _cr = cr;
    }
    public List<CheepViewModel> GetCheeps(int page) 
    {
        return _cr.GetCheeps(page);
    }
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        return _cr.GetCheepsFromAuthor(author, page);
    }
    public ChirpDBContext GetContext() 
    {
        return _cr.context;
    }
}