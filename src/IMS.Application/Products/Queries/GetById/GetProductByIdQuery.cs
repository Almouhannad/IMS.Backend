using IMS.SharedKernel.CQRS;

namespace IMS.Application.Products.Queries.GetById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdQueryResponse>;