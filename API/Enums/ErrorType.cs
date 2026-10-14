namespace API.Enums;

/// <summary>
/// نوع خطا برای دسته‌بندی پاسخ‌های خطای API
/// این Enum به فرانت‌اند کمک می‌کند تا نوع خطا را تشخیص دهد و رفتار مناسب نشان دهد
/// </summary>
public enum ErrorType
{
    /// <summary>خطای منطق دامنه (400 Bad Request)</summary>
    DomainError,

    /// <summary>خطای اعتبارسنجی ورودی (400 Bad Request)</summary>
    ValidationError,

    /// <summary>منبع یافت نشد (404 Not Found)</summary>
    NotFound,

    /// <summary>عدم احراز هویت (401 Unauthorized)</summary>
    Unauthorized,

    /// <summary>عدم دسترسی مجاز (403 Forbidden)</summary>
    Forbidden,

    /// <summary>تداخل در داده، مانند رکورد تکراری (409 Conflict)</summary>
    Conflict,

    /// <summary>خطای داخلی سرور (500 Internal Server Error)</summary>
    InternalError
}