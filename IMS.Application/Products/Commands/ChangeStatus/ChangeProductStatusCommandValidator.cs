using FluentValidation;
using IMS.Domain.Entities;

namespace IMS.Application.Products.Commands.ChangeStatus;

public sealed class ChangeProductStatusCommandValidator : AbstractValidator<ChangeProductStatusCommand>
{
    public ChangeProductStatusCommandValidator()
    {
        RuleFor(c => c.NewStatus)
            .Must(BeValidStatus)
            .When(c => !string.IsNullOrWhiteSpace(c.NewStatus))
            .WithMessage("NewStatus must be one of the allowed product statuses (InStock, Sold, Damaged).");
    }
    private static bool BeValidStatus(string? statusFilter)
    {
        if (string.IsNullOrWhiteSpace(statusFilter))
            return true; // handled by When(...)

        return ProductStatuses.TryGetStatus(statusFilter, out _);
    }
}
