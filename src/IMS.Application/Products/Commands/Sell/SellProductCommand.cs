using IMS.SharedKernel.CQRS;

namespace IMS.Application.Products.Commands.Sell;

public sealed record class SellProductCommand (Guid Id) : ICommand;
