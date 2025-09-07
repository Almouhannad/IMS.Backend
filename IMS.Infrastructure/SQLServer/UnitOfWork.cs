using IMS.Domain.Interfaces;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Infrastructure.SQLServer;

public sealed class UnitOfWork(IMSDBContext context) : IUnitOfWork
{
    private readonly IMSDBContext _context = context;
    public IProductRepository Products { get; } = new ProductRepository(context);
    public ICategoryRepository Categories { get; } = new CategoryRepository(context);

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            return Result.Failure(new Error("Persistence.Failure", "Unable to persist data", ErrorType.Failure));
        }
        
        return Result.Success();
    }
}