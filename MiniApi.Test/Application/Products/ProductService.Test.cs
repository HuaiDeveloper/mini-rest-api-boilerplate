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
    [TestMethod]
    public async Task Get_SingleProduct_ReturnSameProduct()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new ApplicationDbContext(contextOptions);
        
        if (context.Database.EnsureCreated())
        {
            using var viewCommand = context.Database.GetDbConnection().CreateCommand();
            viewCommand.CommandText = @"
                CREATE VIEW AllResources AS
                SELECT Url
                FROM Product;";
            viewCommand.ExecuteNonQuery();
        }
        
        var mockProduct = new Product("TestName", "TestDesc");
        
        await context.AddAsync(mockProduct);
        await context.SaveChangesAsync();
        

        var productService = new ProductService(context);
        var product = await productService.GetProductAsync(mockProduct.Id);
        
        Assert.AreEqual(mockProduct.Id, product.Id);
        Assert.AreEqual(mockProduct.Name, product.Name);
        Assert.AreEqual(mockProduct.Description, product.Description);
        Assert.AreEqual(0, product.CurrentPrices?.Count());
    }
}