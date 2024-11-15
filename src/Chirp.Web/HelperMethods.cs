using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure;

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
    public static string Follow(ModelStateDictionary modelState, string nameOfAuthorEmail, string userAuthor,
        string userEmail, ICheepService service, string userIdentity, string authorEmail)
    {
        if (IsInvalid(userAuthor, modelState) &&
            IsInvalid(nameOfAuthorEmail, modelState))
        {
            return "Error";
        }
        
        service.GetOrCreateAuthor(userIdentity, userEmail);
        Console.WriteLine("LUCASSSSSSSSSSS", authorEmail);
        string a0 = service.GetAuthor(userEmail).Idenitifer;
        string a1 = service.GetAuthor(authorEmail).Idenitifer;


        service.CreateFollow(a0,a1);
        return "UserTimeline";
    }
}