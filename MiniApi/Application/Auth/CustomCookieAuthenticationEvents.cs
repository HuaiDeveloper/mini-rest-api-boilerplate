using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MiniApi.Application.Auth;

public class CustomCookieAuthenticationEvents(StaffManager staffManager) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var claimsPrincipal = context.Principal;
        if (claimsPrincipal == null)
        {
            await Reject(context);
            return;
        }

        var claims = claimsPrincipal.Claims.ToArray();
        
        var staffIdString = claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value;
        var staffNameString = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var staffRoleString = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(staffIdString)
            || string.IsNullOrEmpty(staffNameString)
            || string.IsNullOrEmpty(staffRoleString))
        {
            await Reject(context);
            return;
        }
        
        if (long.TryParse(staffIdString, out var staffId) == false)
        {
            await Reject(context);
            return;
        }
            
        var staff = await staffManager.FindStaffAsync(staffId);
        if (staff.Name != staffNameString
            || staff.AuthRole != staffRoleString)
            await Reject(context);
    }

    private async Task Reject(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();

        await context.HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}