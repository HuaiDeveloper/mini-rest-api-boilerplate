using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniApi.Application.Auth.Request;
using MiniApi.Application.Auth.Response;
using MiniApi.Common;

namespace MiniApi.Application.Auth;

public class AuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly StaffManager _staffManager;
    public AuthService(
        IHttpContextAccessor httpContextAccessor,
        StaffManager staffManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _staffManager = staffManager;
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

            var isExistStaff = await _staffManager.IsExistStaffNameAsync(request.Name);
            if (isExistStaff)
                throw new Exception("User name exist!");
                    
            await _staffManager.CreateUserStaffAsync(
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

            var staff = await _staffManager.FindStaffByNameAsync(request.Name);
            var verifyPasswordResult = await _staffManager.VerifyPasswordAsync(staff, request.Password);
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
    
    public async Task<BasePaginationResponse<List<StaffDto>>> SearchUserStaffs(BasePaginationRequest request)
    {
        try
        {
            var searchUserStaffsResult = await _staffManager.SearchUserStaffs(
                request.Page, request.Size);

            var data = searchUserStaffsResult.Data
                .Select(x => new StaffDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email
                })
                .ToList();

            return new BasePaginationResponse<List<StaffDto>>()
            {
                IsSuccess = true,
                Data = data,
                TotalCount = searchUserStaffsResult.TotalCount
            };
        }
        catch (Exception ex)
        {
            return new BasePaginationResponse<List<StaffDto>>()
            {
                IsSuccess = false,
                Data = null,
                TotalCount = 0
            };
        }
    }
}