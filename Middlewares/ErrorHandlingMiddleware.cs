using System.Text.Json;
using Auth.Errors;

namespace Auth.Middlewares;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger, RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try {
            await _next(context);
        } catch (ServerException ex) {
            if (ex.InnerException != null)
                logger.LogError(ex.InnerException, "The previous user error was caused by: {}", 
                    ex.InnerException.Message);

            await HandleException(context, ex);
        } catch (Exception ex) {
            logger.LogError(ex, "Unhandled exception: {}", ex.Message);
            await HandleException(context, CommonErrors.UnknownError);
        }
    }

    private static async Task HandleException(HttpContext context, ServerException exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)exception.Status;

        var errorResponse = new { message = exception.Message };

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}