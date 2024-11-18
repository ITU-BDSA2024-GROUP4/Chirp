using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chirp.Web.Pages.Utils;

public class StateValidator
{
    public static bool IsInvalid(string input, ModelStateDictionary modelState)
    {
        foreach (var state in modelState)
        {
            if (state.Key.StartsWith(input))
            {
                foreach (var _ in state.Value.Errors)
                {
                    return true; //Invalid because error :(
                }
                return false; //Its valid if exists and no error :)
            }
        }

        return true; //Invalid because not exist :(
    }
}