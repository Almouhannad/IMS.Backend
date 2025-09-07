using IMS.Domain.Entities;
using IMS.Domain.Interfaces;
using IMS.Infrastructure.SQLServer.DAOs;
using IMS.SharedKernel.ResultPattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.Infrastructure.SQLServer;

public sealed class ProductRepository(IMSDBContext context, ILogger<ProductRepository> logger) : IProductRepository
{
    private readonly IMSDBContext _context = context;
    private readonly ILogger<ProductRepository> _logger = logger;

    public async Task<Result> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var productDao = product.ToDao();
        try
        {
            await _context.Products.AddAsync(productDao, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create product {Product}", product);
            return Result.Failure(CommonErrors.OperationFailureError("Create", "Product"));
        }
    }

    public Result Delete(Product product)
    {
        var productsDao = product.ToDao();
        try
        {
            _context.Remove(productsDao);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete product {Product}", product);
            return Result.Failure(CommonErrors.OperationFailureError("Delete", "Product"));
        }
    }

    public async Task<Result<IReadOnlyList<Product>>> GetAllAsync(ProductStatus? status = null, CancellationToken cancellationToken = default)
    {
        IQueryable<ProductDao> query = _context.Products.AsNoTracking();
        if (status.HasValue)
        {
            var statusId = (int)status.Value;
            query = query
                .Where(product => product.StatusId == statusId);
        }
        query = query
            .Include(product => product.Category)
            .Include(product => product.Status);
        try
        {
            var daoList = await query.ToListAsync(cancellationToken);
            return daoList
                .Select(p => p.ToDomain().Value) // TODO: Handle mapping to domain errors when such logic appears
                .ToList()
                .AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get products");
            return Result.Failure<IReadOnlyList<Product>>(CommonErrors.OperationFailureError("Get", "Products"));
        }

    }

    public async Task<Result<Product?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            // Select then join to improve performance
            .Include(product => product.Category)
            .Include(product => product.Status);
        try
        {
            var queryResult = await query.FirstOrDefaultAsync(cancellationToken);
            if (queryResult is null)
            {
                return Result.Success<Product?>(null);
            }
            return queryResult.ToDomain()!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get product by id {Id}", id);
            return Result.Failure<Product?>(CommonErrors.OperationFailureError("Get", "Product"));
        }

    }

    public Result Update(Product product)
    {
        var productDao = product.ToDao();
        try
        {
            _context.Update(productDao);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update product {Product}", product);
            return Result.Failure(CommonErrors.OperationFailureError("Update", "Product"));
        }
        
    }
}