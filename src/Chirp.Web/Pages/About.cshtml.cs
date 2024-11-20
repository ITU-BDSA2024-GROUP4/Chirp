

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
        SetInformation(); // Seems it needs to be set again

        var files = new Dictionary<string, string>
        {
            { "Email.txt", CreateEmailFile() },
            { "Name.txt", CreateNameFile() },
            { "Following.txt", CreateFollowingFile() },
            { "Cheeps.txt", CreateCheepFile() }
        };

        var memoryStream = new MemoryStream();

        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var entry = zipArchive.CreateEntry(file.Key);
                using (var writer = new StreamWriter(entry.Open()))
                {
                    writer.Write(file.Value);
                }
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
        return "Your email:\n" + GetEmail();
    }
    public string CreateNameFile()
    {
        return "Your name:\n" + GetName();
    }
    public string CreateFollowingFile()
    {
        string returner = "People that you follow:";
        foreach (AuthorDTO author in Following)
        {
            returner += "\n" + author.Name;
        }
        return returner;
    }
    public string CreateCheepFile()
    {
        string returner = "Cheeps that you have chirped:";
        foreach (CheepDTO cheep in Cheeps)
        {
            returner += "\n" + cheep.Message;
        }
        return returner;
    }

    public IActionResult OnGetLogout()
    {
        return Authentication.HandleLogout(this);
    }

    public IActionResult OnPostForgetMe()
    {
        UserEmail = UserHandler.FindEmail(User);
        Console.WriteLine("DEBUUUUUUUUG");
        Console.WriteLine(UserEmail +" UserEmail IS DEBUUUUG");
        _service.ForgetMe(UserEmail);
        Console.WriteLine("USEREMAIL: (CSHTML) " + UserEmail);
        return Authentication.HandleLogout(this);
    }
    
}