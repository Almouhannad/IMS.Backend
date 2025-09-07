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
        ProductStatus? statusFilter = ParseStatus(query.StatusFilter);

        var getAllResult = await _unitOfWork.Products
            .GetAllAsync(statusFilter, cancellationToken)
            .ConfigureAwait(false);

        if (getAllResult.IsFailure)
            return Result.Failure<IReadOnlyList<GetProductByIdQueryResponse>>(getAllResult.Error);

        return getAllResult.Value
                    .Select(p => p.ToResponse())
                    .ToList()
                    .AsReadOnly();
    }

    private static ProductStatus? ParseStatus(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        return ProductStatuses.TryGetStatus(raw, out var status) ? status : null;
    }
}
