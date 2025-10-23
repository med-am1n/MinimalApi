using System.Runtime.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExceptionHandlers;

[Serializable]
public class ValidationException : Exception
{
    public string Error { get; }

    public string Message { get; }

    public ValidationException(string error, string message)
    {
        Error = error;
        Message = message;
    }
}

public class ValidationExceptionHandler : IExceptionHandler
{

    private readonly IProblemDetailsService _problemDetailsService;


    public ValidationExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = validationException.Error,
            Detail = validationException.Message,
            Type = "Bad Request"
        }, Exception = exception
        });

    }
}