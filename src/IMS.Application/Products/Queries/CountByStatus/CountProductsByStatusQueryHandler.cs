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
        var countResult = await _unitOfWork.Products.CountByStatusAsync(cancellationToken);
        if (countResult.IsFailure)
        {
            return Result.Failure<CountProductsByStatusQueryResponse>(countResult.Error);
        }

        var counts = countResult.Value;
        counts.TryGetValue(ProductStatus.InStock, out var inStock);
        counts.TryGetValue(ProductStatus.Sold, out var sold);
        counts.TryGetValue(ProductStatus.Damaged, out var damaged);

        return new CountProductsByStatusQueryResponse
        {
            InStock = inStock,
            Sold = sold,
            Damaged = damaged
        };
    }
}
