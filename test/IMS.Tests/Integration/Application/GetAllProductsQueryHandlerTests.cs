using IMS.Application.Products.Queries.GetAll;
using IMS.Domain.Entities;
using IMS.Infrastructure.SQLServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.Tests.Integration.Application;

public class GetAllProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_FiltersByStatus()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<IMSDBContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        await using (var seedContext = new IMSDBContext(options))
        {
            await seedContext.Database.EnsureCreatedAsync();
            using var loggerFactory = LoggerFactory.Create(builder => { });
            await using var unitOfWork = new UnitOfWork(seedContext, loggerFactory);

            var category = Category.Create(Guid.NewGuid(), "Toys").Value;
            await unitOfWork.Categories.CreateAsync(category);

            var inStockProduct = Product.Create(Guid.NewGuid(), "Car", "123", null, 1.0m, ProductStatus.InStock, category).Value;
            var soldProduct = Product.Create(Guid.NewGuid(), "Ball", "456", null, 2.0m, ProductStatus.Sold, category).Value;
            await unitOfWork.Products.CreateAsync(inStockProduct);
            await unitOfWork.Products.CreateAsync(soldProduct);
            await unitOfWork.SaveChangesAsync();
        }

        await using var context = new IMSDBContext(options);
        using var factory = LoggerFactory.Create(builder => { });
        await using var queryUnitOfWork = new UnitOfWork(context, factory);
        var handler = new GetAllProductsQueryHandler(queryUnitOfWork);
        var query = new GetAllProductsQuery("Sold");

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Ball", result.Value[0].Name);
        Assert.Equal("Sold", result.Value[0].Status);
    }
}