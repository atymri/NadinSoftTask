using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ProductManager.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        
        public ExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, "{ExceptionType} {ExceptionMessage} - InnerException: {InnerExceptionType} {InnerExceptionMessage}",
                        ex.GetType().Name,
                        ex.Message,
                        ex.InnerException.GetType().Name,
                        ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError(ex, "{ExceptionType} {ExceptionMessage}",
                        ex.GetType().Name,
                        ex.Message);
                }
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, title) = GetStatusCodeAndTitle(exception);
            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message, 
                Instance = context.Request.Path
            };

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
        }

        private static (int statusCode, string title) GetStatusCodeAndTitle(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException => (StatusCodes.Status400BadRequest, "Bad Request"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                TimeoutException => (StatusCodes.Status408RequestTimeout, "Request Timeout"),

                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}