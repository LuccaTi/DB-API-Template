using WebAPITemplate.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace APITemplate.Host.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unhandled exception has occurred: {ex.Message}");

                var statusCode = HttpStatusCode.InternalServerError;
                var message = "An internal server error has occured.";

                if(ex is HttpRequestException httpRequestException)
                {
                    statusCode = httpRequestException.StatusCode ?? HttpStatusCode.BadGateway;
                    message = httpRequestException.Message;
                }

                if (ex is NotFoundException notFoundException)
                {
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundException.Message;
                }

                if (ex is InvalidIdException invalidIdException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidIdException.Message;
                }

                if (ex is InvalidResourceException invalidResourceException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidResourceException.Message;
                }

                if (ex is ConflictException conflictException)
                {
                    statusCode = HttpStatusCode.Conflict;
                    message = conflictException.Message;
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = message
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
