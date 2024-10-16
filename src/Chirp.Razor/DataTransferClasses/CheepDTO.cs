namespace Chirp.Razor.DataTransferClasses;

public class CheepDTO {
    public string Author { get; set; } = null!;
    public string Message { get; set; } = null!;
    public long TimeStamp { get; set; }
}