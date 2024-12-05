namespace Chirp.Core;

public interface IAuthorService
{
    public void AddAuthor(string name, string email);
    public List<AuthorDTO> GetBlockedAuthors(string userEmail);
    public AuthorDTO GetAuthor(string email);
    public AuthorDTO GetAuthorUserName(string userName);
    public AuthorDTO GetOrCreateAuthor(string name, string email);

}