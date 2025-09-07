using IMS.SharedKernel.CQRS;

namespace IMS.Application.Products.Commands.ChangeStatus;

public sealed record ChangeProductStatusCommand (Guid Id, String NewStatus): ICommand;
