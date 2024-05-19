using Dapper;
using Microsoft.EntityFrameworkCore;
using MiniApi.Application.SerialNumberCodes.Request;
using MiniApi.Application.SerialNumberCodes.Response;
using MiniApi.Common;
using MiniApi.Common.Exceptions;
using MiniApi.Model;
using MiniApi.Persistence.EntityFrameworkCore;

namespace MiniApi.Application.SerialNumberCodes;

public class SerialNumberCodeService(ApplicationDbContext dbContext, ILogger<SerialNumberCodeService> logger)
{
    public async Task<string> GenerateSerialNumberCodeAsync(GenerateSerialNumberCodeRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);

        if (await dbContext.SerialNumberCodes.AnyAsync())
            throw new BaseException("Serial number code already generated");
        
        var serialNumberCodes = new List<SerialNumberCode>();
        for (long index = 1; index <= request.TotalSerialNumberCodeCount; index++)
            serialNumberCodes.Add(new SerialNumberCode(index, $"code{index}", null));
        
        var tableName = nameof(SerialNumberCode);
        var result = await dbContext.NpgBulkInsertAsync(tableName, serialNumberCodes, logger);

        if (result == false)
            throw new BaseException("Generate Fail");
        
        return "Success generate";
    }
    
    public async Task<string> RegenerateSerialNumberCodeAsync(RegenerateSerialNumberCodeRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);
        
        var serialNumberCodes = new List<SerialNumberCode>();
        for (long index = 1; index <= request.TotalSerialNumberCodeCount; index++)
            serialNumberCodes.Add(new SerialNumberCode(index, $"code{index}", null));

        var tableName = nameof(SerialNumberCode);
        var conflictColumnName = new string[]
        {
            nameof(SerialNumberCode.Id)
        };
        var result = await dbContext.NpgBulkUpsertAsync(tableName, serialNumberCodes, conflictColumnName, logger);

        if (result == false)
            throw new BaseException("Regenerate Fail");
        
        return "Success regenerate";
    }
    
    public async Task<BasePaginationResponse<List<SerialNumberCodeDto>>> SearchSerialNumberCodesAsync(BasePaginationRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);
        
        var serialNumberCodeQuery = dbContext.SerialNumberCodes.AsNoTracking();
            
        var serialNumberCodes = await serialNumberCodeQuery
            .OrderByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(x => new SerialNumberCodeDto()
            {
                Id = x.Id,
                NumberCode = x.NumberCode,
                GenerateOn = x.GenerateOn
            })
            .ToListAsync();

        var totalCount = await serialNumberCodeQuery.CountAsync();

        return new BasePaginationResponse<List<SerialNumberCodeDto>>()
        {
            Data = serialNumberCodes,
            TotalCount = totalCount
        };
    }
    
    public async Task<string> ClearSerialNumberCodesAsync()
    {
        var clearSerialNumberCodesSql = $"DELETE FROM public.\"SerialNumberCode\" WHERE 1 = 1;";
        await dbContext.DbConnection.ExecuteAsync(clearSerialNumberCodesSql);

        return "Successfully clear";
    }
}