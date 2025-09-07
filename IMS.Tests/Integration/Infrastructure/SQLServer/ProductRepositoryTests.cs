using IMS.Domain.Entities;
using IMS.Infrastructure.SQLServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace IMS.Tests.Integration.Infrastructure.SQLServer;

public class ProductRepositoryTests
{
    [Fact]
    public async Task CreateUpdateDeleteProduct_Workflow()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<IMSDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new IMSDBContext(options);
        await context.Database.EnsureCreatedAsync();

        var categoryRepo = new CategoryRepository(context, NullLogger<CategoryRepository>.Instance);
        var category = Category.Create(Guid.NewGuid(), "Toys").Value;
        await categoryRepo.CreateAsync(category);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);
        var product = Product.Create(Guid.NewGuid(), "Car", "123", null, 1.0, ProductStatus.InStock, category).Value;

        // Act
        var createResult = await repository.CreateAsync(product);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var getById = await repository.GetByIdAsync(product.Id);

        product.ChangeStatus(ProductStatus.Sold);
        var updateResult = repository.Update(product);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var soldProducts = await repository.GetAllAsync(ProductStatus.Sold);

        var deleteResult = repository.Delete(product);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var afterDelete = await repository.GetByIdAsync(product.Id);

        // Assert
        Assert.True(createResult.IsSuccess);
        Assert.True(getById.IsSuccess);
        Assert.NotNull(getById.Value);
        Assert.Equal("Car", getById.Value!.Name);

        Assert.True(updateResult.IsSuccess);
        Assert.True(soldProducts.IsSuccess);
        Assert.Single(soldProducts.Value);
        Assert.Equal(ProductStatus.Sold, soldProducts.Value.First().Status);

        Assert.True(deleteResult.IsSuccess);
        Assert.True(afterDelete.IsSuccess);
        Assert.Null(afterDelete.Value);
    }
}