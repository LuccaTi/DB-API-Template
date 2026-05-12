using DBAPITemplate.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace DBAPITemplate.Api.Middleware
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
                var statusCode = HttpStatusCode.InternalServerError;
                var message = "An internal server error has occured.";
                object? errors = null;
                var logLevel = LogLevel.Error;

                if (ex is OperationCanceledException)
                {
                    statusCode = (HttpStatusCode)499;
                    message = "Client closed the request.";
                    logLevel = LogLevel.Information;
                }
                else if (ex is FluentValidation.ValidationException validationException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = "One or more validation errors occurred.";
                    errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    logLevel = LogLevel.Warning;
                }
                else if (ex is HttpRequestException httpRequestException)
                {
                    statusCode = httpRequestException.StatusCode ?? HttpStatusCode.BadGateway;
                    message = httpRequestException.Message;
                }
                else if (ex is NotFoundException notFoundException)
                {
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundException.Message;
                }
                else if (ex is InvalidIdException invalidIdException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidIdException.Message;
                    logLevel = LogLevel.Warning;
                }
                else if (ex is InvalidResourceException invalidResourceException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidResourceException.Message;
                    logLevel = LogLevel.Warning;
                }
                else if (ex is ConflictException conflictException)
                {
                    statusCode = HttpStatusCode.Conflict;
                    message = conflictException.Message;
                    logLevel = LogLevel.Warning;
                }

                if (logLevel == LogLevel.Error)
                {
                    _logger.LogError(ex, "An unhandled exception has occured: {Message}", ex.Message);
                }
                else if (logLevel == LogLevel.Warning)
                {
                    _logger.LogWarning("A business/validation issue occurred: {Message}", ex.Message);
                }
                else
                {
                    _logger.LogInformation("Operation was cancelled or ended early: {Message}", ex.Message);
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = message,
                    Errors = errors
                };

                var jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };

                var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
