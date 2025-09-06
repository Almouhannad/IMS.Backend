using IMS.Domain.Entities;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Interfaces;

// CRUD
public interface IProductRepository
{
    Task<Result> CreateAsync(Product product, CancellationToken cancellationToken = default);

    // Read
    Task<Result<Product>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Product>>> GetAllAsync(ProductStatus? status = null, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Product product, CancellationToken cancellationToken = default);
}