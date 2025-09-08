using IMS.Domain.Entities;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Application.Products.Queries.GetById;

public sealed class GetProductByIdQueryResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Barcode { get; set; }
    public string? Description { get; set; }
    public decimal Weight { get; set; }
    public required string Status { get; set; }
    public required string Category { get; set; }

}

public static class ProductResponseMapper
{
    public static GetProductByIdQueryResponse ToResponse(this Product product)
    {
        return new GetProductByIdQueryResponse
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode,
            Description = product.Description ?? "",
            Weight = product.Weight,
            Status = product.Status.ToString(),
            Category = product.Category.Name
        };
    }
}
