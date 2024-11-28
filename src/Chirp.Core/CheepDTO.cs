namespace Chirp.Core;

public class CheepDTO {
    public string Author { get; set; } = null!;
    public string Email  { get; set; } = null!;
    public string Message { get; set; } = null!;
    public long TimeStamp { get; set; }
    public int CheepId {get; set; }
}
