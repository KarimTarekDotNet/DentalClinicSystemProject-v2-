using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DentalClinicProject.API.Middleware
{
    public class GlobalMiddlewareException
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalMiddlewareException> _logger;

        public GlobalMiddlewareException(RequestDelegate next, ILogger<GlobalMiddlewareException> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var cancellationToken = context.RequestAborted;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            }
        }
    }
}
