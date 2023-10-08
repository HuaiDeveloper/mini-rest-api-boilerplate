using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MiniApi.Application.Auth;

public class CustomCookieAuthenticationEvents :  CookieAuthenticationEvents
{
    public CustomCookieAuthenticationEvents()
    {
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var validateResult = true;
        if (!validateResult)
        {
            context.RejectPrincipal();

            await context.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}