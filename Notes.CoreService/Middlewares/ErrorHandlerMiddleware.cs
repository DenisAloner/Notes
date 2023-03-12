using System.Net;
using Notes.CoreService.Exceptions;

namespace Notes.CoreService.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "text/plain";

            string message;
            switch (error)
            {
                case DomainException exception:
                    message = exception.Message;
                    response.StatusCode = (int)exception.GetHttpStatusCode;
                    break;
                case FluentValidation.ValidationException exception:
                    message = exception.Message;
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    message = error.Message;
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            await response.WriteAsync(message);
        }
    }
}