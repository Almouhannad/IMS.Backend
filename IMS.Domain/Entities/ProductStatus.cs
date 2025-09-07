using System.Collections.ObjectModel;

namespace IMS.Domain.Entities;

public enum ProductStatus
{
    InStock = 0,
    Sold = 1,
    Damaged = 2
}

public static class ProductStatuses
{
    public static readonly ReadOnlyDictionary<string, ProductStatus> Values =
        new(
            new Dictionary<string, ProductStatus>(StringComparer.OrdinalIgnoreCase)
            {
                { "InStock", ProductStatus.InStock },
                { "Sold", ProductStatus.Sold },
                { "Damaged", ProductStatus.Damaged }
            }
        );

    // Helper method to normalize input and fetch enum value
    public static bool TryGetStatus(string input, out ProductStatus status)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            status = default;
            return false;
        }

        // normalize: remove spaces, ignore case
        string normalized = input.Replace(" ", string.Empty);

        return Values.TryGetValue(normalized, out status);
    }
}
