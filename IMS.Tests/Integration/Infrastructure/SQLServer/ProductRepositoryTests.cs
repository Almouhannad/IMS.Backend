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
        var createResult = await repository.CreateAsync(product);
        Assert.True(createResult.IsSuccess);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var getById = await repository.GetByIdAsync(product.Id);
        Assert.True(getById.IsSuccess);
        Assert.NotNull(getById.Value);
        Assert.Equal("Car", getById.Value!.Name);

        product.ChangeStatus(ProductStatus.Sold);
        var updateResult = repository.Update(product);
        Assert.True(updateResult.IsSuccess);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var soldProducts = await repository.GetAllAsync(ProductStatus.Sold);
        Assert.True(soldProducts.IsSuccess);
        Assert.Single(soldProducts.Value);
        Assert.Equal(ProductStatus.Sold, soldProducts.Value.First().Status);

        var deleteResult = repository.Delete(product);
        Assert.True(deleteResult.IsSuccess);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var afterDelete = await repository.GetByIdAsync(product.Id);
        Assert.True(afterDelete.IsSuccess);
        Assert.Null(afterDelete.Value);
    }
}