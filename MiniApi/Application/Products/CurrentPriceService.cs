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
}