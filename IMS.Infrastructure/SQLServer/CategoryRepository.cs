using IMS.Domain.Entities;
using IMS.Domain.Interfaces;
using IMS.Infrastructure.SQLServer.DAOs;
using IMS.SharedKernel.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure.SQLServer;

public sealed class CategoryRepository(IMSDBContext context) : ICategoryRepository
{
    private readonly IMSDBContext _context = context;

    public async Task<Result> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        var categoryDao = category.ToDao();
        try
        {
            await _context.Categories.AddAsync(categoryDao, cancellationToken);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(CommonErrors.OperationFailureError("Create", "Category"));
        }
    }

    public async Task<Result<Category?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        name = CategoryMappings.NormalizeName(name);
        var query = _context.Categories.Where(category => category.Name == name);
        try
        {
            var queryResult = await query.FirstOrDefaultAsync(cancellationToken);
            if (queryResult is null)
            {
                return Result.Success<Category?>(null);
            }
            return queryResult.ToDomain()!;
        }
        catch (Exception e)
        {
            throw;
            return Result.Failure<Category?>(CommonErrors.OperationFailureError("Get", "Category"));
        }


    }
}
