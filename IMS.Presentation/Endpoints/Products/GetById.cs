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

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products/{id:guid}/", async
            (Guid id,
            IQueryHandler<GetProductByIdQuery, GetProductByIdQueryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetProductByIdQuery(id);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, ResultToResponseMapper.MapProblem);
        })
            .WithTags("Products");
    }
}
