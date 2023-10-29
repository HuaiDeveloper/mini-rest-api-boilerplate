using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniApi.Application.Products;
using MiniApi.Application.Products.Request;
using MiniApi.Common;
using MiniApi.Common.Exceptions;
using MiniApi.Model;
using MiniApi.Persistence.EntityFrameworkCore;
using Moq;

namespace MiniApi.Test.Application.Products;

[TestClass]
public class ProductService_Test
{
    private static DbContextOptions<ApplicationDbContext> _mockContextOptions;
    
    [ClassInitialize]
    public static void Init(TestContext testcontext)
    {
        _mockContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(MockDataUtility.SQLiteConnectionString)
            .Options;
    }
    
    [ClassCleanup]
    public static async Task Cleanup()
    {
        // TODO: after ProductService_Test...
    }
    
    [TestMethod]
    public async Task GetProductAsync_Success_ReturnSingleProduct()
    {
        
        var mockProduct = new Product("Apple", "Test apple desc");
        
        await using (var mockDataContext = new ApplicationDbContext(_mockContextOptions))
        {
            await mockDataContext.AddAsync(mockProduct);
            await mockDataContext.SaveChangesAsync();
        }

        await using var dbContext = new ApplicationDbContext(_mockContextOptions);

        var productService = new ProductService(dbContext);
        var product = await productService.GetProductAsync(mockProduct.Id);
        
        Assert.AreEqual(mockProduct.Id, product.Id);
        Assert.AreEqual(mockProduct.Name, product.Name);
        Assert.AreEqual(mockProduct.Description, product.Description);
        Assert.AreEqual(0, product.CurrentPrices?.Count);
    }
    
    [TestMethod]
    public async Task GetProductAsync_Fail_ThrowNotFoundException()
    {
        await using var dbContext = new ApplicationDbContext(_mockContextOptions);

        var productService = new ProductService(dbContext);
        
        await Assert.ThrowsExceptionAsync<NotFoundException>(async ()
            => await productService.GetProductAsync(9999));
    }
    
    [TestMethod]
    public async Task SearchProductsAsync_Success_ReturnPaginationProducts()
    {
        
        var mockProductOne = new Product("PS5", "Test desc 1");
        var mockProductTwo = new Product("Switch", "Test desc 2");
        var mockProductThree = new Product("XBox", "Test desc 3");

        var mockProducts = new List<Product>
        {
            mockProductOne,
            mockProductTwo,
            mockProductThree
        };
        
        await using (var mockDataContext = new ApplicationDbContext(_mockContextOptions))
        {
            await mockDataContext.AddRangeAsync(mockProducts);
            await mockDataContext.SaveChangesAsync();
        }

        await using var dbContext = new ApplicationDbContext(_mockContextOptions);

        var productService = new ProductService(dbContext);
        
        var pageSizeTestRequest = new BasePaginationRequest() { Page = 1, Size = 2 };
        var pageSizeTestProducts = await productService.SearchProductsAsync(pageSizeTestRequest);
        Assert.AreNotEqual(null, pageSizeTestProducts.Data);
        Assert.AreEqual(2, pageSizeTestProducts.Data!.Count);
        Assert.IsTrue(pageSizeTestProducts.TotalCount >= 3);
        Assert.IsTrue(pageSizeTestProducts.Data!.Select(x => x.Id).Distinct().Count() == 2);
    }
    
    [TestMethod]
    public async Task SearchProductsAsync_Fail_ThrowBadRequestException()
    {
        await using var dbContext = new ApplicationDbContext(_mockContextOptions);

        var productService = new ProductService(dbContext);
        
        var pageErrorRequest = new BasePaginationRequest() { Page = 0, Size = 2 };
        await Assert.ThrowsExceptionAsync<BadRequestException>(async ()
            => await productService.SearchProductsAsync(pageErrorRequest));
        
        var sizeErrorTestRequest = new BasePaginationRequest() { Page = 1, Size = 100 };
        await Assert.ThrowsExceptionAsync<BadRequestException>(async ()
            => await productService.SearchProductsAsync(sizeErrorTestRequest));
    }
    
    [TestMethod]
    public async Task CreateProductAsync_Success_ReturnTrue()
    {
        var createProductRequest = new CreateProductRequest()
        {
            Name = "Sony Xperia",
            Description = "Create new product desc"
        };

        long createProductId;

        await using (var dbContext = new ApplicationDbContext(_mockContextOptions))
        {
            var productService = new ProductService(dbContext);
        
            var product = await productService.CreateProductAsync(createProductRequest);
        
            Assert.AreEqual(createProductRequest.Name, product.Name);
            Assert.AreEqual(createProductRequest.Description, product.Description);

            createProductId = product.Id;
        }
        
        await using var afterDbContext = new ApplicationDbContext(_mockContextOptions);
        var afterProductService = new ProductService(afterDbContext);
        
        var afterProduct = await afterProductService.GetProductAsync(createProductId);
        Assert.AreEqual(createProductRequest.Name, afterProduct.Name);
        Assert.AreEqual(createProductRequest.Description, afterProduct.Description);
        Assert.AreEqual(0, afterProduct.CurrentPrices?.Count);
    }
    
    [TestMethod]
    public async Task CreateProductAsync_Fail_ThrowBadRequestException()
    {
        await using var dbContext = new ApplicationDbContext(_mockContextOptions);

        var productService = new ProductService(dbContext);
        
        var nameLargeErrorCreateProductRequest = new CreateProductRequest()
        {
            Name = "LargeLargeLargeLarge1"
        };
        await Assert.ThrowsExceptionAsync<BadRequestException>(async ()
            => await productService.CreateProductAsync(nameLargeErrorCreateProductRequest));
        
        var nameNullErrorCreateProductRequest = new CreateProductRequest()
        {
            Description = "desc"
        };
        await Assert.ThrowsExceptionAsync<BadRequestException>(async ()
            => await productService.CreateProductAsync(nameNullErrorCreateProductRequest));
        
        var descriptionLargeErrorCreateProductRequest = new CreateProductRequest()
        {
            Name = "Desc large",
            Description = "LargeLargeLargeLargeLargeLargeLargeLargeLargeLarge1"
        };
        await Assert.ThrowsExceptionAsync<BadRequestException>(async ()
            => await productService.CreateProductAsync(descriptionLargeErrorCreateProductRequest));
    }
}