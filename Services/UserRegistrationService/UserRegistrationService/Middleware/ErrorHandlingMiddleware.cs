using Serilog;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;

namespace UserRegistrationService.Api.Middleware
{

    public class ErrorHandlingMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the error using Serilog
            Log.Error(exception, "An unhandled exception occurred");

            // Create a structured error response
            var response = new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred.",
                Details = exception.Message // Can be removed in production for security
            };

            if (exception.GetType() == typeof(InvalidCredentialException))
            {
                response = new
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = exception.Message,
                    Details = exception.Message // Can be removed in production for security
                };
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

    }
}