using IMS.SharedKernel.CQRS;

namespace IMS.Application.Products.Queries.CountByStatus;

public sealed record CountProductsByStatusQuery() : IQuery<CountProductsByStatusQueryResponse>;
