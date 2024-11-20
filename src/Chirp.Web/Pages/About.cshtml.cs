

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO.Compression;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages.Partials;
using Chirp.Web.Pages.Utils;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
        UserEmail = UserHandler.FindEmail(User);

        Following = GetFollowers();
        Cheeps = GetCheeps();

        return Page();
    }
    public ActionResult OnPostDownload()
    {
        string emailFile = CreateEmailFile();
        string nameFile = CreateNameFile();
        var memoryStream = new MemoryStream();

        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var emailEntry = zipArchive.CreateEntry("Email.txt");
            using (var writer = new StreamWriter(emailEntry.Open()))
            {
                writer.Write(emailFile);
            }
            var nameEntry = zipArchive.CreateEntry("Name.txt");
            using (var writer = new StreamWriter(nameEntry.Open()))
            {
                writer.Write(nameFile);
            }

        }
        memoryStream.Position = 0;
        return File(memoryStream, "application/zip", $"archive-{DateTime.Now:yyyy-MMM-dd-HHmmss}.zip");
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
        return _service.GetCheepsFromAuthor(Author);
    }
    public string CreateEmailFile()
    {
        return "Your email:\n" + UserEmail;
    }
    public string CreateNameFile()
    {
        return "Your name:\n" + "REPLACE";
    }
}