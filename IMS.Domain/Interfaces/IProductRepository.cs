using IMS.Domain.Entities;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Interfaces;

// CRUD
public interface IProductRepository
{
    Task<Result> CreateAsync(Product product, CancellationToken cancellationToken = default);

    // Read
    Task<Result<Product?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Product>>> GetAllAsync(ProductStatus? status = null, CancellationToken cancellationToken = default);

    Result Update(Product product);
    Result Delete(Product product);
}