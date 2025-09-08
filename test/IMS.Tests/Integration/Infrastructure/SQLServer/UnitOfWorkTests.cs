using IMS.Domain.Entities;
using IMS.Infrastructure.SQLServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.Tests.Integration.Infrastructure.SQLServer;

public class UnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_PersistsData()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<IMSDBContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        await using var context = new IMSDBContext(options);
        await context.Database.EnsureCreatedAsync();

        using var loggerFactory = LoggerFactory.Create(builder => { });
        await using var unitOfWork = new UnitOfWork(context, loggerFactory);

        var category = Category.Create(Guid.NewGuid(), "Gadgets").Value;
        await unitOfWork.Categories.CreateAsync(category);

        var product = Product.Create(Guid.NewGuid(), "Phone", "001", null, 0.5, ProductStatus.InStock, category).Value;
        await unitOfWork.Products.CreateAsync(product);

        // Act
        var saveResult = await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.True(saveResult.IsSuccess);

        await using var verifyContext = new IMSDBContext(
            new DbContextOptionsBuilder<IMSDBContext>().UseInMemoryDatabase(dbName).Options);
        var savedProduct = await verifyContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(savedProduct);
    }
}