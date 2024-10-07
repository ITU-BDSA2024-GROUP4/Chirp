using Chirp.SQLite.CheepRepos;

namespace Chirp.SQLite.CheepClient;

public class CheepInjection 
{
    public CheepClient ResolveClient() 
    {
        CheepRepo service = new CheepRepo();
        CheepClient client = new CheepClient(service);
        return client;
    }
}