using IMS.Domain.Entities;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Result> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<Result<Category?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}