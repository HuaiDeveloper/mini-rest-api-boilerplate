using Dapper;
using Microsoft.EntityFrameworkCore;
using MiniApi.Application.SerialNumberCodes.Request;
using MiniApi.Application.SerialNumberCodes.Response;
using MiniApi.Common;
using MiniApi.Common.Exceptions;
using MiniApi.Model;
using MiniApi.Persistence.EntityFrameworkCore;

namespace MiniApi.Application.SerialNumberCodes;

public class SerialNumberCodeService
{
    private readonly ApplicationDbContext _dbContext;
    private ILogger<SerialNumberCodeService> _logger;
    public SerialNumberCodeService(ApplicationDbContext dbContext, ILogger<SerialNumberCodeService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<string> GenerateSerialNumberCodeAsync(GenerateSerialNumberCodeRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);

        if (await _dbContext.SerialNumberCodes.AnyAsync())
            throw new BaseException("Serial number code already generated");
        
        var serialNumberCodes = new List<SerialNumberCode>();
        for (long index = 1; index <= request.TotalSerialNumberCodeCount; index++)
            serialNumberCodes.Add(new SerialNumberCode(index, $"code{index}", null));
        
        var tableName = nameof(SerialNumberCode);
        var result = await _dbContext.NpgBulkInsertAsync(tableName, serialNumberCodes, _logger);

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
        var result = await _dbContext.NpgBulkUpsertAsync(tableName, serialNumberCodes, conflictColumnName, _logger);

        if (result == false)
            throw new BaseException("Regenerate Fail");
        
        return "Success regenerate";
    }
    
    public async Task<BasePaginationResponse<List<SerialNumberCodeDto>>> SearchSerialNumberCodesAsync(BasePaginationRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);
        
        var serialNumberCodeQuery = _dbContext.SerialNumberCodes.AsNoTracking();
            
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
        await _dbContext.DbConnection.ExecuteAsync(clearSerialNumberCodesSql);

        return "Successfully clear";
    }
}