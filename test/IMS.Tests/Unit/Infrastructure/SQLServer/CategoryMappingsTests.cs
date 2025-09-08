using IMS.Domain.Entities;
using IMS.Infrastructure.SQLServer.DAOs;

namespace IMS.Tests.Unit.Infrastructure.SQLServer;

public class CategoryMappingsTests
{
    [Theory]
    [InlineData("  Electronics ", "electronics")]
    [InlineData(" home  appliances ", "home appliances")]
    public void NormalizeName_ReturnsLowercaseTrimmed(string input, string expected)
    {
        // Act
        var result = CategoryMappings.NormalizeName(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToDao_NormalizesCategoryName()
    {
        // Arrange
        var category = Category.Create(Guid.NewGuid(), "  Electronics ").Value;

        // Act
        var dao = category.ToDao();

        // Assert
        Assert.Equal("electronics", dao.Name);
    }
}

