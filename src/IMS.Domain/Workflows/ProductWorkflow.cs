using IMS.Domain.Entities;
using IMS.Domain.Errors;
using IMS.SharedKernel.ResultPattern;

namespace IMS.Domain.Workflows;

/// <summary>
/// Centralizes the allowed <see cref="ProductStatus"/> transitions and their
/// corresponding business rules. This avoids scattering conditional logic
/// throughout the domain and makes future state changes easier to manage.
/// </summary>
public static class ProductWorkflow
{
    // Defines the set of valid transitions for each state.
    private static readonly IReadOnlyDictionary<ProductStatus, IReadOnlySet<ProductStatus>> AllowedTransitions =
        new Dictionary<ProductStatus, IReadOnlySet<ProductStatus>>
        {
            [ProductStatus.InStock] = new HashSet<ProductStatus>
            {
                ProductStatus.Sold,
                ProductStatus.Damaged
            },
            [ProductStatus.Sold] = new HashSet<ProductStatus> { ProductStatus.InStock }, // Renew inventory
            [ProductStatus.Damaged] = new HashSet<ProductStatus>() { ProductStatus.InStock }, // Fix damaged items in inventory
        };

    // Holds specific errors for well-known invalid transitions.
    private static readonly IReadOnlyDictionary<(ProductStatus Current, ProductStatus Next), Error> TransitionErrors =
        new Dictionary<(ProductStatus, ProductStatus), Error>
        {
            {(ProductStatus.Sold, ProductStatus.Sold), ProductErrors.NotInStock},
            {(ProductStatus.Damaged, ProductStatus.Sold), ProductErrors.SellDamagedProduct},
        };

    /// <summary>
    /// Validates whether a transition between two statuses is allowed.
    /// </summary>
    public static Result Validate(ProductStatus current, ProductStatus next)
    {
        if (current == next)
        {
            return Result.Failure(ProductErrors.AlreadyInStatus(current));
        }

        if (TransitionErrors.TryGetValue((current, next), out var specific))
        {
            return Result.Failure(specific);
        }

        if (AllowedTransitions.TryGetValue(current, out var allowed) && allowed.Contains(next))
        {
            return Result.Success();
        }

        return Result.Failure(ProductErrors.InvalidStatusTransition(current, next));
    }
}