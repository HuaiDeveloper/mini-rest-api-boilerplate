using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniApi.Application.Auth.Request;
using MiniApi.Application.Auth.Response;
using MiniApi.Common;
using MiniApi.Common.Exceptions;

namespace MiniApi.Application.Auth;

public class AuthService(
    IHttpContextAccessor httpContextAccessor,
    StaffManager staffManager)
{
    public async Task<string> SignUp(SignUpRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);

        var isExistStaff = await staffManager.IsExistStaffNameAsync(request.Name);
        if (isExistStaff)
            throw new BadRequestException("User name exist!");
                    
        await staffManager.CreateUserStaffAsync(
            request.Name,
            request.Email,
            request.Password,
            request.Description);
            
        return "Success";
    }
    
    public async Task<string> SignIn(SignInRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);

        var staff = await staffManager.FindStaffAsync(request.Name);
        var verifyPasswordResult = await staffManager.VerifyPasswordAsync(staff, request.Password);
        if (verifyPasswordResult == false)
            throw new NotFoundException("Verification failed");
            
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.UserData, staff.Id.ToString()),
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

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new BaseException("httpContext is null");
            
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
                    
        return "Success";
    }

    public async Task<string> SignOut()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new BaseException("httpContext is null");
            
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    
        return "Success";
    }
    
    public async Task<BasePaginationResponse<List<StaffDto>>> SearchUserStaffs(BasePaginationRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);
        
        var searchUserStaffsResult = await staffManager.SearchUserStaffs(
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