using IMS.Application.Products.Queries.GetById;
using IMS.SharedKernel.CQRS;

namespace IMS.Application.Products.Queries.GetAll;

public sealed record GetAllProductsQuery(
    string? StatusFilter,
    int Page = 1,
    int PageSize = 10) : IQuery<IReadOnlyList<GetProductByIdQueryResponse>>;