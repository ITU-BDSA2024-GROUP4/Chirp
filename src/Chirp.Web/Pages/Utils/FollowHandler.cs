using Chirp.Core;
using Chirp.Web.Pages.Partials;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chirp.Web.Pages.Utils;

public static class FollowHandler
{
    public static string Follow(ModelStateDictionary modelState, IAuthorService service,
        string nameOfAuthorUsername, string nameOfAuthor, string userEmail,
        string username, string authorUsername)
    {
        if (StateValidator.IsInvalid(nameOfAuthor, modelState) &&
            StateValidator.IsInvalid(nameOfAuthorUsername, modelState))
        {
            return "Error";
        }

        service.CreateFollow(username, userEmail, authorUsername);
        return "UserTimeline";
    }
    // TODO: FIX THIS
    public static string Unfollow(ModelStateDictionary modelState, IAuthorService service,
        string nameOfAuthorUsername, string nameOfAuthor, string username,
        string authorUsername, SubmitMessageModel submitMessage)
    {
        if (StateValidator.IsInvalid(nameOfAuthorUsername, modelState) &&
            StateValidator.IsInvalid(nameOfAuthor, modelState))
        {
            if (submitMessage != null)
            {
                return "Page";
            }

            return "Error";
        }
        
        service.UnFollow(username, authorUsername);
        return "Page";
    }
    
}