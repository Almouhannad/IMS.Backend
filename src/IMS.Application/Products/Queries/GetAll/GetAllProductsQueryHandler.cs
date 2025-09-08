using IMS.Application.Products.Queries.GetById;
using IMS.Domain.Entities;
using IMS.Domain.Interfaces;
using IMS.SharedKernel.CQRS;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Application.Products.Queries.GetAll;

public sealed class GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllProductsQuery, IReadOnlyList<GetProductByIdQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<GetProductByIdQueryResponse>>> Handle(
        GetAllProductsQuery query,
        CancellationToken cancellationToken)
    {
        // Parse/normalie the incoming status (null if not valid/provided)
        ProductStatus? statusFilter = ProductStatuses.TryGetStatus(query.StatusFilter, out var status)
            ? status
            : null;

        var getAllResult = await _unitOfWork.Products
            .GetAllAsync(statusFilter, query.Page, query.PageSize, cancellationToken)
            .ConfigureAwait(false);

        if (getAllResult.IsFailure)
            return Result.Failure<IReadOnlyList<GetProductByIdQueryResponse>>(getAllResult.Error);

        return getAllResult.Value
                    .Select(p => p.ToResponse())
                    .ToList();
    }
}
