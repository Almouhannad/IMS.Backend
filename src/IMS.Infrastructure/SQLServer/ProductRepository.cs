using IMS.Domain.Entities;
using IMS.Domain.Interfaces;
using IMS.Infrastructure.SQLServer.DAOs;
using IMS.SharedKernel.ResultPattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

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
        var productDao = product.ToDao();
        try
        {
            _context.Remove(productDao);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete product {Product}", product);
            return Result.Failure(CommonErrors.OperationFailureError("Delete", "Product"));
        }
    }

    public async Task<Result<IReadOnlyList<Product>>> GetAllAsync(
        ProductStatus? status = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
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
            .Include(product => product.Status)
            .OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        List<ProductDao> daoList;
        try
        {
            daoList = await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get products");
            return Result.Failure<IReadOnlyList<Product>>(CommonErrors.OperationFailureError("Get", "Products"));
        }

        var domainList = new List<Product>(daoList.Count);
        foreach (var productDao in daoList)
        {
            var mapToProductDomainResult = productDao.ToDomain();
            if (mapToProductDomainResult.IsFailure)
            {
                return Result.Failure<IReadOnlyList<Product>>(mapToProductDomainResult.Error);
            }
            domainList.Add(mapToProductDomainResult.Value);
        }
        return domainList.AsReadOnly();

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

    public async Task<Result<IReadOnlyDictionary<ProductStatus, int>>> CountByStatusAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var counts = await _context.Products
                .AsNoTracking()
                .GroupBy(p => p.StatusId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var dictionary = counts.ToDictionary(
                x => (ProductStatus)x.Key,
                x => x.Count);

            return new ReadOnlyDictionary<ProductStatus, int>(dictionary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count products by status");
            return Result.Failure<IReadOnlyDictionary<ProductStatus, int>>(
                CommonErrors.OperationFailureError("Count", "Products"));
        }
    }
}