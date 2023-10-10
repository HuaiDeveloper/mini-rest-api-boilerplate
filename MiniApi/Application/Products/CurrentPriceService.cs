using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MiniApi.Application.Products.Request;
using MiniApi.Application.Products.Response;
using MiniApi.Common;
using MiniApi.Common.Exceptions;
using MiniApi.Model;
using MiniApi.Persistence.EntityFrameworkCore;

namespace MiniApi.Application.Products;

public class CurrentPriceService
{
    private readonly ApplicationDbContext _dbContext;
    public CurrentPriceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<CurrentPriceDetailDto> CreateCurrentPriceAsync(CreateCurrentPriceRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);
        
        var product = await _dbContext.Products.AsNoTracking()
            .Where(x => x.Id == request.ProductId)
            .FirstOrDefaultAsync();
        if (product == null)
            throw new NotFoundException($"Not found product id: {request.ProductId}");

        var currentPrice = new CurrentPrice(
            request.ProductId,
            request.Price,
            request.CurrentDate,
            request.Description);

        _dbContext.CurrentPrices.Add(currentPrice);
        await _dbContext.SaveChangesAsync();

        return new CurrentPriceDetailDto()
        {
            Id = currentPrice.Id,
            ProductId = currentPrice.ProductId,
            Price = currentPrice.Price,
            CurrentDate = currentPrice.CurrentDate,
            Description = currentPrice.Description
        };
    }

    public async Task<CurrentPriceDetailDto> UpdateCurrentPriceAsync(UpdateCurrentPriceRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);
        
        var currentPrice = await _dbContext.CurrentPrices
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();
        if (currentPrice == null)
            throw new NotFoundException($"Not found id: {request.Id}");

        currentPrice.Update(
            request.Price,
            request.CurrentDate,
            request.Description);
        await _dbContext.SaveChangesAsync();

        return new CurrentPriceDetailDto()
        {
            Id = currentPrice.Id,
            ProductId = currentPrice.ProductId,
            Price = currentPrice.Price,
            CurrentDate = currentPrice.CurrentDate,
            Description = currentPrice.Description
        };
    }

    public async Task<BasePaginationResponse<List<ProductLatestPriceDto>>> SearchProductLatestPricesAsync(
        BasePaginationRequest request)
    {
        var isValidate = CustomValidator.TryValidateObject(request, out var validationResults);
        if (isValidate == false)
            throw new BadRequestException(validationResults);

        var sql =
            " WITH product_temp AS" +
            " (" +
            "   SELECT " +
            "       \"Id\", \"Name\"" +
            "   FROM public.\"Product\"" +
            "   ORDER BY \"Id\" DESC" +
            "   OFFSET @skip" +
            "   LIMIT @take" +
            " )," +
            " current_price_temp AS" +
            " (" +
            "   SELECT" +
            "       x.\"ProductId\"," +
            "       x.\"ProductName\"," +
            "       y.\"Price\" AS \"LatestPrice\"," +
            "       y.\"CurrentDate\"" +
            "   FROM" +
            "   (" +
            "       SELECT" +
            "           Max(cp.\"Id\") AS \"Id\"," +
            "           pt.\"Id\" AS \"ProductId\"," +
            "           pt.\"Name\" AS \"ProductName\"" +
            "       FROM product_temp pt" +
            "       LEFT JOIN public.\"CurrentPrice\" cp ON pt.\"Id\" = cp.\"ProductId\"" +
            "       GROUP BY cp.\"ProductId\", pt.\"Id\", pt.\"Name\"" +
            "   ) x" +
            "   LEFT JOIN public.\"CurrentPrice\" y ON x.\"Id\" = y.\"Id\"" +
            " )" +
            " SELECT * FROM current_price_temp" +
            " ORDER BY \"ProductId\" DESC;";
        
        var parameter = new DynamicParameters();
        parameter.Add("skip", (request.Page - 1) * request.Size, DbType.Int32);
        parameter.Add("take", request.Size, DbType.Int32);

        var productLatestPrices = await _dbContext.DbConnection
            .QueryAsync<ProductLatestPriceDto>(sql, parameter);

        var countSql = "SELECT COUNT(1) FROM public.\"Product\" WHERE 1 = 1;";
        var totalCount = await _dbContext.DbConnection.QueryFirstAsync<int>(countSql);

        return new BasePaginationResponse<List<ProductLatestPriceDto>>()
        {
            Data = productLatestPrices.ToList(),
            TotalCount = totalCount
        };
    }
}