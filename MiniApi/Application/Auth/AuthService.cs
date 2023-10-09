using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniApi.Application.Auth.Request;
using MiniApi.Application.Auth.Response;
using MiniApi.Common;
using MiniApi.Common.Exceptions;

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
        if (string.IsNullOrEmpty(request.Name))
            throw new BadRequestException("Name required");
        
        if (string.IsNullOrEmpty(request.Email))
            throw new BadRequestException("Email required");
        
        if (string.IsNullOrEmpty(request.Password))
            throw new BadRequestException("Password required");

        var isExistStaff = await _staffManager.IsExistStaffNameAsync(request.Name);
        if (isExistStaff)
            throw new BadRequestException("User name exist!");
                    
        await _staffManager.CreateUserStaffAsync(
            request.Name,
            request.Email,
            request.Password,
            request.Description);
            
        return "Success";
    }
    
    public async Task<string> SignIn(SignInRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
            throw new BadRequestException("Staff name required");
            
        if (string.IsNullOrEmpty(request.Password))
            throw new BadRequestException("password required");

        var staff = await _staffManager.FindStaffAsync(request.Name);
        var verifyPasswordResult = await _staffManager.VerifyPasswordAsync(staff, request.Password);
        if (verifyPasswordResult == false)
            throw new NotFoundException("Verification failed");
            
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
            throw new BaseException("httpContext is null");
            
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
                    
        return "Success";
    }

    public async Task<string> SignOut()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new BaseException("httpContext is null");
            
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    
        return "Success";
    }
    
    public async Task<BasePaginationResponse<List<StaffDto>>> SearchUserStaffs(BasePaginationRequest request)
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
            Data = data,
            TotalCount = searchUserStaffsResult.TotalCount
        };
    }
}