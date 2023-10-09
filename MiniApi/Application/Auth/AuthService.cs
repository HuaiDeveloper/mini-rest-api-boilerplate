using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniApi.Application.Auth.Request;

namespace MiniApi.Application.Auth;

public class AuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly StaffService _staffService;
    public AuthService(
        IHttpContextAccessor httpContextAccessor,
        StaffService staffService)
    {
        _httpContextAccessor = httpContextAccessor;
        _staffService = staffService;
    }

    public async Task<string> SignUp(SignUpRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new Exception("Name required");
        
            if (string.IsNullOrEmpty(request.Email))
                throw new Exception("Email required");
        
            if (string.IsNullOrEmpty(request.Password))
                throw new Exception("Password required");

            var isExistStaff = await _staffService.IsExistStaffNameAsync(request.Name);
            if (isExistStaff)
                throw new Exception("User name exist!");
                    
            await _staffService.CreateUserStaffAsync(
                request.Name,
                request.Email,
                request.Password,
                request.Description);
                    
            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return "Fail";
        }
    }
    
    public async Task<string> SignIn(SignInRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new Exception("Staff name required");
            
            if (string.IsNullOrEmpty(request.Password))
                throw new Exception("password required");

            var staff = await _staffService.FindStaffByNameAsync(request.Name);
            var verifyPasswordResult = await _staffService.VerifyPasswordAsync(staff, request.Password);
            if (verifyPasswordResult == false)
                throw new Exception("Verification failed");
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, staff.Name),
                new Claim(ClaimTypes.Role, staff.AuthRole),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var validHours = 8;
            var issuedUtc = DateTimeOffset.UtcNow;
            var expiresUtc = issuedUtc.AddHours(validHours);

            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = true,
                IssuedUtc = issuedUtc,
                ExpiresUtc = expiresUtc
            };

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new Exception("httpContext is null");
            
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
                    
            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return "Fail";
        }
    }

    public async Task<string> SignOut()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new Exception("httpContext is null");
            
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    
            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return "Fail";
        }
    }
}