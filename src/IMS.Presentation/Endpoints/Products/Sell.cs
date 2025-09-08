using IMS.Application.Products.Commands.Sell;
using IMS.SharedKernel.API;
using IMS.SharedKernel.CQRS;
using IMS.SharedKernel.ResultPattern;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IMS.Presentation.Endpoints.Products;

public class Sell : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("products/{id:guid}/sell", async
            (Guid id,
            ICommandHandler<SellProductCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SellProductCommand(id);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, ResultToResponseMapper.MapProblem);
        })
            .WithTags("Products");
    }
}
