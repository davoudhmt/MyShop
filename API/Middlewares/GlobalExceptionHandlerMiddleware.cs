using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای پیش‌بینی‌نشده از نوع: {ExceptionType} در مسیر {Path}",
                    ex.GetType().FullName, context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = exception switch
            {
                DomainException => CreateDomainErrorResponse(exception),
                InvalidOperationException => CreateValidationErrorResponse(exception),
                KeyNotFoundException => CreateNotFoundResponse(exception),
                ArgumentException => CreateValidationErrorResponse(exception),
                UnauthorizedAccessException => CreateUnauthorizedResponse(exception),
                _ => CreateInternalErrorResponse(exception)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)response.StatusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            var jsonResponse = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(jsonResponse);
        }

        private static ExceptionResponse CreateDomainErrorResponse(Exception ex) => new()
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = ex.Message,
            ErrorType = ErrorType.DomainError,
            Details = null
        };

        private static ExceptionResponse CreateValidationErrorResponse(Exception ex) => new()
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = ex.Message,
            ErrorType = ErrorType.ValidationError,
            Details = null
        };

        private static ExceptionResponse CreateNotFoundResponse(Exception ex) => new()
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Message = ex.Message,
            ErrorType = ErrorType.NotFound,
            Details = null
        };

        private static ExceptionResponse CreateUnauthorizedResponse(Exception ex) => new()
        {
            StatusCode = (int)HttpStatusCode.Unauthorized,
            Message = ex.Message,
            ErrorType = ErrorType.Unauthorized,
            Details = null
        };

        private ExceptionResponse CreateInternalErrorResponse(Exception ex) => new()
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = _environment.IsDevelopment() ? ex.Message : "خطای داخلی سرور. لطفاً بعداً تلاش کنید.",
            ErrorType = ErrorType.InternalError,
            Details = _environment.IsDevelopment() ? ex.StackTrace : null
        };
    }
}