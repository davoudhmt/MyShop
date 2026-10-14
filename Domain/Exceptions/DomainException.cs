using System;

namespace Domain.Exceptions
{
    /// <summary>
    /// استثنای دامنه‌ای که برای نشان دادن خطاهای مرتبط با منطق دامنه استفاده می‌شود.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// مفهوم یا موجودیت دامنه‌ای که باعث ایجاد استثنا شده است (مثلاً: "Catalog.Brand")
        /// </summary>
        public string? DomainConcept { get; }

        /// <summary>
        /// نام فیلد یا خاصیتی که باعث ایجاد استثنا شده است
        /// </summary>
        public string? PropertyName { get; }

        /// <summary>
        /// سازنده استثنا فقط با پیام خطا
        /// </summary>
        public DomainException(string message)
            : this(message, null, null, null)
        {
        }

        /// <summary>
        /// سازنده استثنا با پیام خطا و مفهوم دامنه
        /// </summary>
        public DomainException(string message, string domainConcept)
            : this(message, domainConcept, null, null)
        {
        }

        /// <summary>
        /// سازنده استثنا با پیام خطا، مفهوم دامنه و نام خاصیت
        /// </summary>
        public DomainException(string message, string domainConcept, string propertyName)
            : this(message, domainConcept, propertyName, null)
        {
        }

        /// <summary>
        /// سازنده اصلی با همه پارامترها
        /// </summary>
        public DomainException(
            string message,
            string? domainConcept,
            string? propertyName,
            Exception? innerException)
            : base(message, innerException)
        {
            DomainConcept = domainConcept;
            PropertyName = propertyName;
        }
    }
}