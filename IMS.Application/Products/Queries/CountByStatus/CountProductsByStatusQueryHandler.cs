using IMS.Domain.Entities;
using IMS.Domain.Interfaces;
using IMS.SharedKernel.CQRS;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Application.Products.Queries.CountByStatus;

public sealed class CountProductsByStatusQueryHandler(IUnitOfWork unitOfWork) : IQueryHandler<CountProductsByStatusQuery, CountProductsByStatusQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CountProductsByStatusQueryResponse>> Handle(CountProductsByStatusQuery query, CancellationToken cancellationToken)
    {
        var getAllResult = await _unitOfWork.Products.GetAllAsync(null, cancellationToken);
        if (getAllResult.IsFailure)
        {
            return Result.Failure<CountProductsByStatusQueryResponse>(getAllResult.Error);
        }
        var products = getAllResult.Value;

        return new CountProductsByStatusQueryResponse
        {
            InStock = products.Count(p => p.Status == ProductStatus.InStock),
            Sold = products.Count(p => p.Status == ProductStatus.Sold),
            Damaged = products.Count(p => p.Status == ProductStatus.Damaged)
        };
    }
}
