using FluentValidation;
using System.Text.Json; 

namespace Backend.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed.");

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    Message = ex.Message
                }));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access.");

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    Message = ex.Message
                }));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found.");

            context.Response.StatusCode =
                StatusCodes.Status404NotFound;

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    Message = ex.Message
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    Message = $"An unexpected error occurred.{ex.Message}"
                }));
        }
    }
}