using IMS.Domain.Entities;

namespace IMS.Tests.Unit.Domain;

public class ProductStatusTests
{
    [Theory]
    [InlineData(" InStock ", ProductStatus.InStock)]
    [InlineData("sold", ProductStatus.Sold)]
    [InlineData(" DAMAGED", ProductStatus.Damaged)]
    public void TryGetStatus_ReturnsTrue_ForValidInputs(string input, ProductStatus expected)
    {
        var result = ProductStatuses.TryGetStatus(input, out var status);

        Assert.True(result);
        Assert.Equal(expected, status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void TryGetStatus_ReturnsFalse_ForInvalidInputs(string? input)
    {
        var result = ProductStatuses.TryGetStatus(input, out var status);

        Assert.False(result);
        Assert.Equal(default, status);
    }
}