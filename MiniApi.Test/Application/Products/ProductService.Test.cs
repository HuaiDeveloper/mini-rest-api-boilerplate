using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniApi.Application.Products;
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
        var mockDataContext = new ApplicationDbContext(_mockContextOptions);

        await mockDataContext.Products.ExecuteDeleteAsync();
    }
    
    [TestMethod]
    public async Task GetProductAsync_ReturnSingleProduct()
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
}