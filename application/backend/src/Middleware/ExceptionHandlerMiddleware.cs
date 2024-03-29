using System.Net;
using backend.Model.Exceptions;
using backend.Model.Rest;
using Newtonsoft.Json;

namespace backend.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            logger.LogError($"{e.Message}: \n{e.StackTrace}");
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception e)
    {
        HttpStatusCode status;
        switch (e)
        {
            case BadRequestException:
            case ArgumentException:
                status = HttpStatusCode.BadRequest;
                break;
            case DbKeyNotFoundException:
                status = HttpStatusCode.NotFound;
                break;
            case UnauthorizedException:
                status = HttpStatusCode.Unauthorized;
                break;
            case ForbiddenException:
                status = HttpStatusCode.Forbidden;
                break;
            default:
                status = HttpStatusCode.InternalServerError;
                break;
        }

        var exceptionResult = JsonConvert.SerializeObject(new ApiError { Message = e.Message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsync(exceptionResult);
    }
}