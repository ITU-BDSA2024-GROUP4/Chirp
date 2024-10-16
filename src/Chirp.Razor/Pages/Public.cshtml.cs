using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Razor.CheepService;
using Chirp.Razor.DataTransferClasses;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {

        var pageQuery = Request.Query["page"].ToString();
        
        if (pageQuery == null)
        {
            Cheeps = _service.GetCheeps(0); // default to first page
        }
        else
        {
            _ = int.TryParse(pageQuery, out int page);            

            Cheeps = _service.GetCheeps(page-1); // minus 1 because pages are 0 indexed
            
        }

        return Page();
    }
}