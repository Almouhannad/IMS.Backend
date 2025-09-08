using IMS.Domain.Interfaces;
using IMS.SharedKernel.ResultPattern;
using Microsoft.Extensions.Logging;

namespace IMS.Infrastructure.SQLServer;

public sealed class UnitOfWork(IMSDBContext context, ILoggerFactory loggerFactory) : IUnitOfWork
{
    private readonly IMSDBContext _context = context;
    private readonly ILogger<UnitOfWork> _logger = loggerFactory.CreateLogger<UnitOfWork>();
    public IProductRepository Products { get; } = new ProductRepository(context, loggerFactory.CreateLogger<ProductRepository>());
    public ICategoryRepository Categories { get; } = new CategoryRepository(context, loggerFactory.CreateLogger<CategoryRepository>());

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            //await _context.DisposeAsync(); // To reduce overhead in requests with multiple DB actions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes");
            return Result.Failure(new Error("Persistence.Failure", "Unable to persist data", ErrorType.Failure));
        }

        return Result.Success();
    }
}