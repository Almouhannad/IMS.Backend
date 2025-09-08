using IMS.Application.Products.Queries.CountByStatus;
using IMS.SharedKernel.API;
using IMS.SharedKernel.CQRS;
using Microsoft.AspNetCore.Routing;
using IMS.SharedKernel.ResultPattern;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace IMS.Presentation.Endpoints.Products;

public class CountByStatus : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products/count", async
            (IQueryHandler<CountProductsByStatusQuery, CountProductsByStatusQueryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new CountProductsByStatusQuery();
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, ResultToResponseMapper.MapProblem);
        })
            .WithTags("Products");
    }
}
