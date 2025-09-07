using IMS.Domain.Entities;
using IMS.SharedKernel.ResultPattern;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS.Infrastructure.SQLServer.DAOs;

[Table("Products")]
[Index(nameof(Id), IsUnique = true)]
[Index(nameof(Barcode), IsUnique = true)]
public sealed class ProductDao
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Barcode { get; set; } = string.Empty;

    public string? Description { get; set; }

    public double Weight { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public CategoryDao Category { get; set; } = null!;

    [Required]
    public int StatusId { get; set; }

    [ForeignKey(nameof(StatusId))]
    public ProductStatusDao Status { get; set; } = null!;
}


public static class ProductMappings
{
    public static Result<Product> ToDomain(this ProductDao productDao)
    {
        var createCategoryDomainResult = Category.Create(productDao.CategoryId, productDao.Category.Name);
        if (createCategoryDomainResult.IsFailure)
        {
            return Result.Failure<Product>(createCategoryDomainResult.Error);
        }
        var categoryDomain = createCategoryDomainResult.Value;
        
        var statusDomain = (ProductStatus)productDao.StatusId;

        var createProductDomainResult = Product.Create(
            productDao.Id,
            productDao.Name,
            productDao.Barcode,
            productDao.Description,
            productDao.Weight,
            statusDomain,
            categoryDomain
        );
        return createProductDomainResult;
    }

    public static ProductDao ToDao(this Product productDomain)
    {
        return new ProductDao
        {
            Id = productDomain.Id,
            Name = productDomain.Name,
            Barcode = productDomain.Barcode,
            Description = productDomain.Description,
            Weight = productDomain.Weight,
            CategoryId = productDomain.Category.Id,
            StatusId = (int)productDomain.Status
        };
    }
}