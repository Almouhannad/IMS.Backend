using IMS.Domain.Entities;
using IMS.SharedKernel.ResultPattern;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS.Infrastructure.SQLServer.DAOs;

[Table("Categories")]
[Index(nameof(Name), IsUnique = true)]
public sealed class CategoryDao
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [InverseProperty(nameof(ProductDao.Category))]
    public ICollection<ProductDao> Products { get; set; } = [];
}

public static class CategoryMappings
{
    public static Result<Category> ToDomain(this CategoryDao categoryDao)
    {
        return Category.Create(categoryDao.Id, categoryDao.Name);
    }

    public static CategoryDao ToDao(this Category categoryDomain)
    {

        return new CategoryDao
        {
            Id = categoryDomain.Id,
            Name = NormalizeName(categoryDomain.Name),
        };
    }

    public static string NormalizeName(string name)
    {
        // Normalize: lowercase + trim + collapse multiple spaces
        return string.Join(
            " ",
            name.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries)
        );

    }
}