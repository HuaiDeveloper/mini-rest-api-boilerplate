using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniApi.Common;
using MiniApi.Common.Exceptions;
using MiniApi.Model;
using MiniApi.Persistence.EntityFrameworkCore;

namespace MiniApi.Application.Auth;

public class StaffManager
{
    private readonly ApplicationDbContext _dbContext;
    public StaffManager(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Staff> CreateUserStaffAsync(
        string name,
        string email,
        string password,
        string? description)
    {
        if (string.IsNullOrEmpty(name))
            throw new BadRequestException("Name required");
        
        if (string.IsNullOrEmpty(email))
            throw new BadRequestException("Email required");
        
        if (string.IsNullOrEmpty(password))
            throw new BadRequestException("Password required");
        
        var staff = new Staff(
            name,
            email,
            "not hash",
            description,
            AuthRole.User);

        staff.UpdatePassword(HashPassword(staff, password));
        
        _dbContext.Staffs.Add(staff);

        await _dbContext.SaveChangesAsync();

        return staff;
    }
    
    public async Task<Staff> FindStaffAsync(long id)
    {
        var staff = await _dbContext.Staffs.AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (staff == null)
            throw new NotFoundException("Staff not found!");
        
        if (VerifyAuthRole(staff.AuthRole) == false)
            throw new NotFoundException("Role not found!");

        return staff;
    }
    
    public async Task<Staff> FindStaffAsync(string name)
    {
        var staff = await _dbContext.Staffs.AsNoTracking()
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync();

        if (staff == null)
            throw new NotFoundException("Staff not found!");

        if (VerifyAuthRole(staff.AuthRole) == false)
            throw new NotFoundException("Role not found!");

        return staff;
    }
    
    public async Task<bool> IsExistStaffNameAsync(string name)
    {
        return await _dbContext.Staffs.AnyAsync(x => x.Name == name);
    }
    
    public async Task<bool> VerifyPasswordAsync(Staff staff, string password)
    {
        var staffPasswordHasher = new PasswordHasher<MiniApi.Model.Staff>();
        
        var verifyHashedPasswordResult = staffPasswordHasher.VerifyHashedPassword(
            staff, staff.Password, password);

        if (verifyHashedPasswordResult == PasswordVerificationResult.Failed)
            return false;

        if (verifyHashedPasswordResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            var staffContext = await _dbContext.Staffs
                .Where(x => x.Id == staff.Id)
                .FirstOrDefaultAsync();

            if (staffContext == null)
                return false;

            staffContext.UpdatePassword(staffPasswordHasher.HashPassword(staffContext, password));

            await _dbContext.SaveChangesAsync();
        }

        return true;
    }
    
    public async Task<(List<Staff> Data, int TotalCount)> SearchUserStaffs(int page, int size)
    {
        var staffQuery = _dbContext.Staffs.AsNoTracking()
            .Where(x => x.AuthRole == AuthRole.User);

        var staffs = await staffQuery
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var totalCount = await staffQuery.CountAsync();

        return (staffs, totalCount);
    }

    private string HashPassword(Staff staff, string password)
    {
        var staffPasswordHasher = new PasswordHasher<MiniApi.Model.Staff>();

        return staffPasswordHasher.HashPassword(staff, password);
    }

    private bool VerifyAuthRole(string staffAuthRole)
    {
        return AuthRole.GetAuthRoles().Any(x => x == staffAuthRole);
    }
}