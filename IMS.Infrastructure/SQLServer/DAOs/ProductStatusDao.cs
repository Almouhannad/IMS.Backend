using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS.Infrastructure.SQLServer.DAOs;

[Table("ProductStatuses")]
public sealed class ProductStatusDao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [InverseProperty(nameof(ProductDao.Status))]
    public ICollection<ProductDao> Products { get; set; } = [];
}