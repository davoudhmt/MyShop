using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middlewares;

/// <summary>
/// Middleware سراسری برای مدیریت تمام استثناهای پیش‌بینی‌نشده در API
/// این Middleware بر اساس نوع استثنا، کد وضعیت HTTP و ساختار پاسخ مناسب را تولید می‌کند
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// سازنده Middleware با دریافت وابستگی‌های مورد نیاز
    /// </summary>
    /// <param name="next">نقطه‌ی بعدی در پایپ‌لاین درخواست</param>
    /// <param name="logger">سرویس لاگ برای ثبت خطاها</param>
    /// <param name="environment">اطلاعات محیط اجرا (توسعه یا تولید)</param>
    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// نقطه‌ی ورود Middleware که کل درخواست را در try-catch می‌گیرد
    /// </summary>
    /// <param name="context">اطلاعات درخواست HTTP جاری</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای پیش‌بینی‌نشده از نوع: {ExceptionType} در مسیر {Path}",
                ex.GetType().Name, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// نگاشت هوشمند استثناها به کدهای وضعیت HTTP و ساخت پاسخ مناسب
    /// </summary>
    /// <param name="context">کانتکست HTTP برای نوشتن پاسخ</param>
    /// <param name="exception">استثنای رخ داده</param>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // نگاشت هوشمند بر اساس نوع استثنا
        var response = exception switch
        {
            // استثناهای اختصاصی دامنه (به ترتیب اولویت)
            NotFoundException ex => CreateResponse(ex, HttpStatusCode.NotFound, ErrorType.NotFound),
            ConflictException ex => CreateResponse(ex, HttpStatusCode.Conflict, ErrorType.Conflict),
            ForbiddenException ex => CreateResponse(ex, HttpStatusCode.Forbidden, ErrorType.Forbidden),

            // DomainException پایه برای سایر خطاهای دامنه
            DomainException ex => CreateResponse(ex, HttpStatusCode.BadRequest, ErrorType.DomainError),

            // استثناهای استاندارد .NET (برای سازگاری با کدهای قدیمی)
            KeyNotFoundException ex => CreateResponse(ex, HttpStatusCode.NotFound, ErrorType.NotFound),
            UnauthorizedAccessException ex => CreateResponse(ex, HttpStatusCode.Unauthorized, ErrorType.Unauthorized),
            InvalidOperationException ex => CreateResponse(ex, HttpStatusCode.BadRequest, ErrorType.ValidationError),
            ArgumentException ex => CreateResponse(ex, HttpStatusCode.BadRequest, ErrorType.ValidationError),

            // خطاهای پیش‌بینی‌نشده
            _ => CreateResponse(exception, HttpStatusCode.InternalServerError, ErrorType.InternalError)
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

    /// <summary>
    /// ساخت ساختار پاسخ یکپارچه با اطلاعات کامل
    /// </summary>
    /// <param name="ex">استثنای رخ داده</param>
    /// <param name="statusCode">کد وضعیت HTTP مورد نظر</param>
    /// <param name="errorType">نوع خطا برای دسته‌بندی</param>
    /// <returns>ساختار پاسخ خطا آماده برای ارسال به کلاینت</returns>
    private ExceptionResponse CreateResponse(Exception ex, HttpStatusCode statusCode, ErrorType errorType)
    {
        // استخراج اطلاعات اضافی اگر استثنا از نوع DomainException باشد
        string? domainConcept = null;
        string? propertyName = null;

        if (ex is DomainException domainEx)
        {
            domainConcept = domainEx.DomainConcept;
            propertyName = domainEx.PropertyName;
        }

        return new ExceptionResponse
        {
            StatusCode = (int)statusCode,
            Message = ex.Message,
            ErrorType = errorType,
            DomainConcept = domainConcept,
            PropertyName = propertyName,
            Details = _environment.IsDevelopment() ? ex.StackTrace : null
        };
    }
}