using FluentValidation;
using IMS.Domain.Entities;

namespace IMS.Application.Products.Queries.GetAll;

public sealed class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
{
    public GetAllProductsQueryValidator()
    {
        RuleFor(q => q.StatusFilter)
            .Must(BeValidStatus)
            .When(q => !string.IsNullOrWhiteSpace(q.StatusFilter))
            .WithMessage("StatusFilter must be one of the allowed product statuses (InStock, Sold, Damaged).");

        RuleFor(q => q.Page)
            .GreaterThan(0);

        RuleFor(q => q.PageSize)
            .GreaterThan(0)
            .LessThan(20);
    }

    private static bool BeValidStatus(string? statusFilter)
    {
        if (string.IsNullOrWhiteSpace(statusFilter))
            return true; // handled by When(...)

        return ProductStatuses.TryGetStatus(statusFilter, out _);
    }
}
