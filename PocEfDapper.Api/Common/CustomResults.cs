using ErrorOr;

namespace PocEfDapper.Api.Common;

public static class CustomResults
{
    public static IResult Problem(List<Error> errors)
    {
        if (errors.Count == 0) return TypedResults.Problem();

        var first = errors[0];
        var statusCode = first.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return TypedResults.Problem(
            statusCode: statusCode,
            title: first.Code,
            detail: first.Description
        );
    }
}