using IMS.Application.Products.Commands.ChangeStatus;
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

public class ChangeStatus : IEndpoint
{
    public sealed record ChangeStatusRequestBody(string NewStatus);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("products/{id:guid}/", async
            (Guid id,
            [FromBody] ChangeStatusRequestBody requestBody,
            ICommandHandler<ChangeProductStatusCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangeProductStatusCommand(id, requestBody.NewStatus);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, ResultToResponseMapper.MapProblem);
        })
            .WithTags("Products");
    }
}
