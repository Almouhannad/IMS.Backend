using IMS.Domain.Entities;
using IMS.Domain.Errors;

namespace IMS.Tests.Unit.Domain;

public class ProductWorkflowTests
{
    [Fact]
    public void SellProduct_SucceedsOnce_ThenFails()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Toys").Value;
        var product = Product.Create(Guid.NewGuid(), "Car", "123", null, 1.0, ProductStatus.InStock, category).Value;

        // Act
        var firstSell = product.Sell();
        var secondSell = product.Sell();

        // Assert
        Assert.True(firstSell.IsSuccess);
        Assert.Equal(ProductStatus.Sold, product.Status);
        Assert.True(secondSell.IsFailure);
        Assert.Equal(ProductErrors.NotInStock, secondSell.Error);
    }

    [Fact]
    public void SellDamagedProduct_ReturnsFailure()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Toys").Value;
        var product = Product.Create(Guid.NewGuid(), "Car", "123", null, 1.0, ProductStatus.Damaged, category).Value;

        // Act
        var result = product.Sell();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ProductErrors.SellDamagedProduct, result.Error);
    }
}