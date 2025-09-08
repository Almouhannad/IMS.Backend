using IMS.Domain.Entities;
using IMS.Domain.Errors;
using IMS.Domain.Interfaces;
using IMS.SharedKernel.CQRS;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Application.Products.Commands.ChangeStatus;

public sealed class ChangeProductStatusCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<ChangeProductStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ChangeProductStatusCommand command, CancellationToken cancellationToken)
    {
        #region Get Product from Persistence
        var getProductResult = await _unitOfWork.Products.GetByIdAsync(command.Id, cancellationToken)
            .ConfigureAwait(false);
        if (getProductResult.IsFailure)
        {
            return Result.Failure(getProductResult.Error);
        }
        if (getProductResult.Value is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        var product = getProductResult.Value;

        #endregion

        #region Change status
        ProductStatuses.TryGetStatus(command.NewStatus, out ProductStatus newStatus);
        var changeStatusResult = product.ChangeStatus(newStatus);
        if (changeStatusResult.IsFailure)
        {
            return Result.Failure(changeStatusResult.Error);
        }
        #endregion

        #region Update in persistence
        var updateResult = _unitOfWork.Products.Update(product);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }
        #endregion

        #region Persist changes
        var persistResult = await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        return persistResult;
        #endregion

    }
}
