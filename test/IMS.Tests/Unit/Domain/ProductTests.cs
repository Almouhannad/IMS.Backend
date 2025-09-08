using IMS.Domain.Entities;
using IMS.Domain.Errors;

namespace IMS.Tests.Unit.Domain;

public class ProductTests
{
    [Fact]
    public void Create_WithoutCategory_ReturnsFailure()
    {
        // Act
        var result = Product.Create(Guid.NewGuid(), "Name", "123", null, 1.0m, ProductStatus.InStock, null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ProductErrors.InvalidCategory, result.Error);
    }

    [Fact]
    public void ChangeStatus_SameStatus_ReturnsFailure()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "Toys").Value;
        var product = Product.Create(Guid.NewGuid(), "Car", "123", null, 1.0m, ProductStatus.InStock, category).Value;

        // Act
        var result = product.ChangeStatus(ProductStatus.InStock);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ProductErrors.AlreadyInStatus(ProductStatus.InStock), result.Error);
    }
}