using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages.Partials;

namespace Chirp.Web;
public static class HelperMethods {
    public static bool IsInvalid(string input, ModelStateDictionary modelState)
    {
        foreach (var state in modelState)
        {
            if (state.Key.StartsWith(input))
            {
                foreach (var error in state.Value.Errors)
                {
                    return true; //Invalid because error :(
                }
                return false; //Its valid if exists and no error :)
            }
        }

        return true; //Invalid because not exist :(
    }

    public static string FindEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }
    public static string Follow(ModelStateDictionary modelState, ICheepService service,
        string nameOfAuthorEmail, string nameOfAuthor, string userEmail,
        string userIdentityName, string authorEmail)
    {
        if (IsInvalid(nameOfAuthor, modelState) &&
            IsInvalid(nameOfAuthorEmail, modelState))
        {
            return "Error";
        }

        service.CreateFollow(userIdentityName, userEmail, authorEmail);
        return "UserTimeline";
    }
    public static string Unfollow(ModelStateDictionary modelState, ICheepService service,
        string nameOfAuthorEmail, string nameOfAuthor, string userEmail,
        string authorEmail, SubmitMessageModel submitMessage)
    {
        if (HelperMethods.IsInvalid(nameOfAuthor, modelState) &&
            HelperMethods.IsInvalid(nameOfAuthorEmail, modelState))
        {
            if (submitMessage != null)
            {
                return "Page";
            }

            return "Error";
        }
        
        Console.WriteLine(authorEmail);
        string a0 = service.GetAuthor(userEmail).Idenitifer;
        string a1 = service.GetAuthor(authorEmail).Idenitifer;

        service.UnFollow(a0, a1);

        return "Page";
    }
}