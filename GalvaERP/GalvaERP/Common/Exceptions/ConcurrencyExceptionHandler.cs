using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GalvaERP.Common.Exceptions;

public class ConcurrencyExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ConcurrencyException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status412PreconditionFailed;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status412PreconditionFailed,
            Title = "Precondition Failed",
            Type = "https://tools.ietf.org/html/rfc7232#section-3.2",
            Detail = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
