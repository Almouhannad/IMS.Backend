using IMS.Application.Products.Queries.GetAll;
using IMS.Application.Products.Queries.GetById;
using IMS.SharedKernel.API;
using IMS.SharedKernel.CQRS;
using IMS.SharedKernel.ResultPattern;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IMS.Presentation.Endpoints.Products;

public class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products", async
            ([FromQuery] string? statusFilter,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            IQueryHandler<GetAllProductsQuery, IReadOnlyList<GetProductByIdQueryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllProductsQuery(statusFilter, page ?? 1, pageSize ?? 10);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, ResultToResponseMapper.MapProblem);
        })
            .WithTags("Products");
    }
}
