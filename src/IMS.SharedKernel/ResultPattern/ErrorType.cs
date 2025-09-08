namespace IMS.SharedKernel.ResultPattern;

public enum ErrorType
{
    /// <summary>
    /// Unknown or unclassified server failure.
    /// Typical HTTP: 500.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Request was well-formed but contains invalid data.
    /// Typical HTTP: 400.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Target resource does not exist.
    /// Typical HTTP: 404.
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// State conflict with the current resource.
    /// Typical HTTP: 409.
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// Infrastructure or persistence issue (e.g., DB unavailable).
    /// Typical HTTP: 500 or 503.
    /// </summary>
    Infrastructure = 4
}
