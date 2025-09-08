using IMS.Domain.Errors;
using IMS.Domain.Interfaces;
using IMS.SharedKernel.CQRS;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Application.Products.Queries.GetById;

public sealed class GetProductByIdQueryHandler(IUnitOfWork unitOfWork) : IQueryHandler<GetProductByIdQuery, GetProductByIdQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<GetProductByIdQueryResponse>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var getProductResult = await _unitOfWork.Products
            .GetByIdAsync(query.Id, cancellationToken)
            .ConfigureAwait(false);

        if (getProductResult.IsFailure)
        {
            return Result.Failure<GetProductByIdQueryResponse>(getProductResult.Error);
        }
        if (getProductResult.Value is null)
        {
            return Result.Failure<GetProductByIdQueryResponse>(ProductErrors.NotFound);
        }
        return getProductResult.Value.ToResponse();
    }
}
