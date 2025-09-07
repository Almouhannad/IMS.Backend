using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Errors;

public static class ProductErrors
{
    public static readonly Error InvalidName = CommonErrors.InvalidFieldError("Product", "Name");

    public static readonly Error InvalidBarcode = CommonErrors.InvalidFieldError("Product", "Barcode");

    public static readonly Error InvalidWeight = CommonErrors.InvalidFieldError("Product", "Weight");

    public static readonly Error InvalidCategory = CommonErrors.InvalidFieldError("Product", "Category");

    public static readonly Error NotFound = CommonErrors.EntityNotFoundError("Product");

    public static readonly Error NotInStock = new(
        "Product.NotInStock",
        "Product is not available in stock.",
        ErrorType.Conflict);
}