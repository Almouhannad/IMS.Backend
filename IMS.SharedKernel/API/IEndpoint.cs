using Microsoft.AspNetCore.Routing;

namespace IMS.SharedKernel.API;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
