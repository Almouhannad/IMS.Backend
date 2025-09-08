using IMS.Domain.Entities;
using IMS.Infrastructure.SQLServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace IMS.Tests.Integration.Infrastructure.SQLServer;

public class CategoryRepositoryTests
{
    [Fact]
    public async Task CreateAndGetByNameAsync_NormalizesName()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<IMSDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new IMSDBContext(options);
        await context.Database.EnsureCreatedAsync();
        var repository = new CategoryRepository(context, NullLogger<CategoryRepository>.Instance);
        var category = Category.Create(Guid.NewGuid(), "  Electronics ").Value;

        // Act
        var createResult = await repository.CreateAsync(category);
        await context.SaveChangesAsync();
        var getResult = await repository.GetByNameAsync("ELECTRONICS");

        // Assert
        Assert.True(createResult.IsSuccess);
        Assert.True(getResult.IsSuccess);
        Assert.NotNull(getResult.Value);
        Assert.Equal(category.Id, getResult.Value!.Id);
        Assert.Equal("electronics", getResult.Value.Name);
    }
}