using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO.Compression;
using System.Text;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages.Utils;

namespace Chirp.Web.Pages;

public class AboutModel : PageModel {
    private readonly ICheepService _service;
    public string Author;
    public string UserEmail;
    public bool UserIsAuthor;
    public List<AuthorDTO> Following;
    public List<CheepDTO> Cheeps;

    public AboutModel(ICheepService service)
    {
        _service = service;
    }
    public ActionResult OnGet(string author)    
    {
        UserIsAuthor = author.Equals(UserHandler.FindName(User));
        Author = author;
        SetInformation();
        return Page();
    }
    public void SetInformation()
    {
        UserEmail = UserHandler.FindEmail(User);
        Following = GetFollowers();
        Cheeps = GetCheeps();
    }
    public ActionResult OnPostDownload()
    {
        SetInformation(); // Ensure data is up-to-date

        var csvContent = CreateCsvContent();

        var memoryStream = new MemoryStream();
        try
        {
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var entry = zipArchive.CreateEntry("Userdata.csv");
                using (var entryStream = entry.Open())
                using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                {
                    writer.Write(csvContent);
                }
            }

            memoryStream.Position = 0;

            return File(memoryStream, "application/zip", $"Userdata-{DateTime.Now:yyyy-MMM-dd-HHmmss}.zip");
        }
        catch
        {
            memoryStream.Dispose();
            throw;
        }
    }

    public string GetEmail() 
    { 
        return _service.GetAuthor(UserEmail).Email;
    }
    public string GetName() 
    {
        return _service.GetAuthor(UserEmail).Name;
    }
    public List<AuthorDTO> GetFollowers()
    {
        return _service.GetFollowers(UserEmail);
    }
    public List<CheepDTO> GetCheeps()
    {
        return _service.GetCheepsFromAuthor(_service.GetAuthor(UserEmail).Name);
    }
    public string CreateCsvContent()
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("Category,Data");
        sb.AppendLine($"Email,{GetEmail()}");
        sb.AppendLine($"Name,{GetName()}");

        sb.AppendLine("Following");
        foreach (var author in Following)
        {
            sb.AppendLine($",{author.Name}");
        }

        sb.AppendLine("Cheeps");
        foreach (var cheep in Cheeps)
        {
            sb.AppendLine($",{cheep.Message}");
        }

        return sb.ToString();
    }

    public IActionResult OnGetLogout()
    {
        return Authentication.HandleLogout(this);
    }

    public IActionResult OnPostForgetMe()
    {
        UserEmail = UserHandler.FindEmail(User);
        _service.ForgetMe(UserEmail);
        return Authentication.HandleLogout(this);
    }
}
