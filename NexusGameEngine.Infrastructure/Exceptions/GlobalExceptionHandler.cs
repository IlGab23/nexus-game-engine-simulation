using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NexusGameEngine.Domain.Exceptions;

namespace NexusGameEngine.Infrastructure.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled error occured: {message}", exception.Message);

        var problemDetail = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Generic_PlaceHolder",
            Detail = "Generic_PlaceHolder"
        };

        if (exception is DomainException domainException)
        {
            problemDetail.Status = domainException.StatusCode;
            problemDetail.Title = domainException.Title;
            problemDetail.Detail = domainException.Message;
        }

        httpContext.Response.StatusCode = problemDetail.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetail, cancellationToken);

        return true;
    }

}
