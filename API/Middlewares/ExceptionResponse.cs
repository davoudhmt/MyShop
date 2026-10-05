using API.Enums;

namespace API.Middlewares
{
    /// <summary>
    /// ساختار استاندارد پاسخ خطا برای کل API
    /// </summary>
    public class ExceptionResponse
    {
        /// <summary>
        /// کد وضعیت HTTP
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// پیام خطا (قابل نمایش به کاربر)
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// نوع خطا (برای دیباگ و لاگ)
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// جزئیات خطا (فقط در محیط توسعه)
        /// </summary>
        public string? Details { get; set; }
    }
}