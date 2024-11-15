#nullable disable

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;
    
public class SubmitMessageModel
{
    [BindProperty]
    [Required]
    [StringLength(160, MinimumLength = 1, ErrorMessage = "Message length must be between {2} and {1} characters.")]
    [Display(Name = "Message Text")]
    public string Message { get; set; }

}