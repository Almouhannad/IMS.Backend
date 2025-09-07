using IMS.Domain.Errors;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Entities;

public sealed class Product
{
    private Product(
        Guid id,
        string name,
        string barcode,
        string? description,
        double weight,
        ProductStatus status,
        Category category)
    {
        Id = id;
        Name = name;
        Barcode = barcode;
        Description = description;
        Weight = weight;
        Status = status;
        Category = category;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Barcode { get; private set; }
    public string? Description { get; private set; }
    public double Weight { get; private set; }
    public ProductStatus Status { get; private set; }
    public Category Category { get; private set; }

    public static Result<Product> Create(
        Guid id,
        string name,
        string barcode,
        string? description,
        double weight,
        ProductStatus status,
        Category category)
    {
        // Add business rules (Not validation ones) here if needed
        
        return Result.Success(new Product(id, name, barcode, description, weight, status, category));
    }

    public Result ChangeStatus(ProductStatus newStatus)
    {
        // Other business logic here if needed
        Status = newStatus;
        return Result.Success();
    }

    public Result Sell()
    {
        if (Status != ProductStatus.InStock)
        {
            return Result.Failure(ProductErrors.NotInStock);
        }
        ChangeStatus(ProductStatus.Sold);
        return Result.Success();
    }
}