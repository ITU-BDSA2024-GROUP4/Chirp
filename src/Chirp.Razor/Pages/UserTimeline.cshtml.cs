using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.SQLite.CheepServices;
using DataTransferClasses;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        var pageQuery = Request.Query["page"].ToString();
        if (pageQuery == null)
        {

            Cheeps = _service.GetCheepsFromAuthor(author, 0); // default to first page
        }
        else
        {
            _ = int.TryParse(pageQuery, out int page);

            
            Cheeps = _service.GetCheepsFromAuthor(author, page-1);
        }
        return Page();
    }
}
