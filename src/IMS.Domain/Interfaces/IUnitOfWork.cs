using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task Dispose(CancellationToken cancellationToken = default);
}