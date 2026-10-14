using API.Enums;

namespace API.Middlewares;

/// <summary>
/// ساختار استاندارد پاسخ خطا برای کل API
/// این کلاس به فرانت‌اند اجازه می‌دهد تا خطاها را به صورت یکپارچه پردازش کند
/// </summary>
public class ExceptionResponse
{
    /// <summary>
    /// کد وضعیت HTTP (مانند 400, 404, 409)
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// پیام خطا (قابل نمایش به کاربر نهایی)
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// نوع خطا برای دسته‌بندی و پردازش در فرانت‌اند
    /// </summary>
    public ErrorType ErrorType { get; set; }

    /// <summary>
    /// مفهوم دامنه که خطا در آن رخ داده است (مثلاً: "Catalog.Brand" یا "Identity.User")
    /// </summary>
    public string? DomainConcept { get; set; }

    /// <summary>
    /// نام خاصیتی که باعث ایجاد خطا شده است (مثلاً: "Name" یا "Email")
    /// </summary>
    public string? PropertyName { get; set; }

    /// <summary>
    /// جزئیات اضافی خطا (فقط در محیط توسعه برای دیباگ نمایش داده می‌شود)
    /// </summary>
    public string? Details { get; set; }
}