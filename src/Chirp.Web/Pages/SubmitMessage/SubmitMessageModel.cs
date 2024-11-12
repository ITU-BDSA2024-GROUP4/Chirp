#nullable disable

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;
    
public class SubmitMessageModel
{
    [BindProperty]
    [Required]
    [StringLength(160, ErrorMessage = "Maximum length is {1}")]
    [Display(Name = "Message Text")]
    public string Message { get; set; }

}