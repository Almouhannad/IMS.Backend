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
    /// Domain/business rule violation.
    /// Typical HTTP: 409 or 422.
    /// </summary>
    BusinessRule = 4,

    /// <summary>
    /// Authentication required or failed.
    /// Typical HTTP: 401.
    /// </summary>
    Unauthorized = 5,

    /// <summary>
    /// Authenticated but not allowed to perform the action.
    /// Typical HTTP: 403.
    /// </summary>
    Forbidden = 6,

    /// <summary>
    /// Too many requests / throttling.
    /// Typical HTTP: 429.
    /// </summary>
    RateLimited = 7,

    /// <summary>
    /// Infrastructure or persistence issue (e.g., DB unavailable).
    /// Typical HTTP: 500 or 503.
    /// </summary>
    Infrastructure = 8
}
