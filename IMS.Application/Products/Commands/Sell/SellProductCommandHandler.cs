using IMS.Application.Products.Commands.ChangeStatus;
using IMS.Domain.Interfaces;
using IMS.SharedKernel.CQRS;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Application.Products.Commands.Sell;

public sealed class SellProductCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<SellProductCommand>
{
    private readonly ChangeProductStatusCommandHandler _changeProductStatusCommandHandler = new(unitOfWork);

    public async Task<Result> Handle(SellProductCommand command, CancellationToken cancellationToken)
    {
        var changeStatusCommand = new ChangeProductStatusCommand(command.Id, "Sold");
        return await _changeProductStatusCommandHandler.Handle(changeStatusCommand, cancellationToken);
    }
}
