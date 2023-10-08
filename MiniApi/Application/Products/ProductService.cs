using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniApi.Application.Products.Request;
using MiniApi.Application.Products.Response;
using MiniApi.Model;
using MiniApi.Persistence.EntityFrameworkCore;

namespace MiniApi.Application.Products;

public class ProductService
{
    private readonly ApplicationDbContext _dbContext;
    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductDetailDto> GetProductAsync(long id)
    {
        var result = await _dbContext.Products.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProductDetailDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            })
            .FirstOrDefaultAsync();
        if (result == null)
            throw new Exception($"Not found id: {id}");

        return result;
    }

    public async Task<List<ProductDto>> SearchProductsAsync(int page, int size)
    {
        var result = await _dbContext.Products.AsNoTracking()
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new ProductDto()
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToListAsync();

        return result;
    }

    public async Task<ProductDetailDto> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product(request.Name, request.Description);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var result = new ProductDetailDto()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description
        };

        return result;
    }

    public async Task<ProductDetailDto> UpdateProductAsync(UpdateProductRequest request)
    {
        var product = await _dbContext.Products.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
        if (product == null)
            throw new Exception($"Not found id: {request.Id}");

        product.Update(request.Name, request.Description);
        await _dbContext.SaveChangesAsync();

        var result = new ProductDetailDto()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description
        };

        return result;
    }

    public async Task<string> DeleteProductAsync(long id)
    {
        var product = await _dbContext.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (product == null)
            throw new Exception($"Not found id: {id}");

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();

        return "Successfully deleted";
    }
}