using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniApi.Application.Products.Request;
using MiniApi.Application.Products.Response;
using MiniApi.Common;
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

    public async Task<BaseResponse<ProductDetailDto>> GetProductAsync(long id)
    {
        try
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
                return new BaseResponse<ProductDetailDto>()
                {
                    IsSuccess = false,
                    Message = $"Not found id: {id}"
                };

            return new BaseResponse<ProductDetailDto>()
            {
                IsSuccess = true,
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<ProductDetailDto>()
            {
                IsSuccess = false,
                Message = "Error"
            };
        }
    }

    public async Task<BasePaginationResponse<List<ProductDto>>> SearchProductsAsync(BasePaginationRequest request)
    {
        try
        {
            var productQuery = _dbContext.Products.AsNoTracking();
            
            var products = await productQuery
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .Select(x => new ProductDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();

            var totalCount = await productQuery.CountAsync();

            return new BasePaginationResponse<List<ProductDto>>()
            {
                IsSuccess = true,
                Data = products,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            return new BasePaginationResponse<List<ProductDto>>()
            {
                IsSuccess = false,
                Message = "Error",
            };
        }
    }

    public async Task<BaseResponse<ProductDetailDto>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var product = new Product(request.Name, request.Description);

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return new BaseResponse<ProductDetailDto>()
            {
                IsSuccess = true,
                Data = new ProductDetailDto()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description
                }
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<ProductDetailDto>()
            {
                IsSuccess = false,
                Message = "Error"
            };
        }
    }

    public async Task<BaseResponse<ProductDetailDto>> UpdateProductAsync(UpdateProductRequest request)
    {
        try
        {
            var product = await _dbContext.Products.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (product == null)
                return new BaseResponse<ProductDetailDto>()
                {
                    IsSuccess = false,
                    Message = $"Not found id: {request.Id}"
                };

            product.Update(request.Name, request.Description);
            await _dbContext.SaveChangesAsync();

            return new BaseResponse<ProductDetailDto>()
            {
                IsSuccess = true,
                Data = new ProductDetailDto()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description
                }
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<ProductDetailDto>()
            {
                IsSuccess = false,
                Message = "Error"
            };
        }
    }

    public async Task<BaseResponse<string>> DeleteProductAsync(long id)
    {
        try
        {
            var product = await _dbContext.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (product == null)
                return new BaseResponse<string>()
                {
                    IsSuccess = false,
                    Message = $"Not found id: {id}"
                };

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return new BaseResponse<string>()
            {
                IsSuccess = true,
                Data = "Successfully deleted"
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<string>()
            {
                IsSuccess = false,
                Message = "Error"
            };
        }
    }
}