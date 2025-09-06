namespace IMS.SharedKernel.ResultPattern;

/// <summary>
/// Immutable error descriptor used to annotate failed <see cref="Result"/> values.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Error"/> instances are lightweight value objects that carry a
/// machine-readable <see cref="Code"/>, a human-readable <see cref="Description"/>,
/// and a <see cref="Type"/> for coarse-grained categorization.
/// </para>
/// <para>
/// Favor stable, namespaced codes (e.g., "Product.Status.InvalidValue") to support
/// localization, analytics, and client-side branching.
/// </para>
/// </remarks>
public record Error
{

    /// <summary>
    /// Machine-readable, stable identifier (e.g., "Order.NotFound").
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Human-readable explanation intended for diagnostics or end-user messaging.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Coarse-grained category of this error.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Creates a new <see cref="Error"/>.
    /// </summary>
    /// <param name="code">Stable, machine-readable error code.</param>
    /// <param name="description">Human-readable description, safe to surface to logs/UX.</param>
    /// <param name="type">Coarse classification of the error.</param>
    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>
    /// Representing “no error”. Used exclusively for successful results.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    /// <summary>
    /// Standardized error for null inputs where a value was required.
    /// Classified as <see cref="ErrorType.Validation"/>.
    /// </summary>
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided.",
        ErrorType.Validation);

}
