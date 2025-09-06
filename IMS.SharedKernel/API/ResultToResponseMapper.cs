using Microsoft.AspNetCore.Http;
using IMS.SharedKernel.ResultPattern;

namespace IMS.SharedKernel.API;

/// <summary>
/// Maps <see cref="Result"/> to standard <see cref="IResult"/> responses.
/// </summary>
public static class ResultToResponseMapper
{
    /// <summary>
    /// Converts a failed <see cref="Result"/> into a standardized Problem Details response.
    /// </summary>
    public static IResult MapProblem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create problem details from a successful result.");
        }

        return Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetValidationErrors(result));
    }

    private static string GetTitle(Error error) =>
        string.IsNullOrWhiteSpace(error.Code) ? "Server failure" : error.Code;

    private static string GetDetail(Error error) =>
        string.IsNullOrWhiteSpace(error.Description)
            ? "An unexpected error occurred"
            : error.Description;

    private static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "https://httpwg.org/specs/rfc9110.html#status.422",
            ErrorType.NotFound => "https://httpwg.org/specs/rfc9110.html#status.404",
            ErrorType.Conflict => "https://httpwg.org/specs/rfc9110.html#status.409",
            ErrorType.Infrastructure => "https://httpwg.org/specs/rfc9110.html#status.503",
            _ => "https://httpwg.org/specs/rfc9110.html#status.500"
        };

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Infrastructure => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

    static Dictionary<string, object?>? GetValidationErrors(Result result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?>
            {
                { "errors", validationError.Errors }
            };
    }
}
