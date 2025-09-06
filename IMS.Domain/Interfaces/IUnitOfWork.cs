using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}