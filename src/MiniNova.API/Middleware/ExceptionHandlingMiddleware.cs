using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MiniNova.BLL.Exceptions;

namespace MiniNova.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _logger.LogInformation("[MIDDLEWARE] Starting request: {Method} {Path}",  context.Request.Method, context.Request.Path);
            await _next(context);
            _logger.LogInformation("[MIDDLEWARE] Request completed with status: {Status}", context.Response.StatusCode);
        } catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statuscode = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,

            _ => HttpStatusCode.InternalServerError
        };
        
        context.Response.StatusCode = (int)statuscode;

        var details = new ProblemDetails
        {
            Status = (int)statuscode,
            Title = GetTitle(exception),
            Detail = exception.Message,
            Type = exception.GetType().Name,
        };

        if (exception is ValidationException ex)
        {
            details.Extensions["field"] = ex.FieldName;
        }
        
        await context.Response.WriteAsJsonAsync(details);
    }

    private static string GetTitle(Exception exception) => exception switch
    {
        ValidationException => "Validation Error",
        KeyNotFoundException => "Resource not found",
        UnauthorizedAccessException => "Unauthorized",
        _ => "Server error"
    };
}

