using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exceptions
{
    public class DomainException : Exception
    {
        /// <summary>
        /// مفهوم یا موجودیت دامنه‌ای که باعث ایجاد استثنا شده است
        /// </summary>
        public DomainConcept? DomainConcept { get; }

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
        public DomainException(string message, DomainConcept domainConcept)
            : this(message, domainConcept, null, null)
        {
        }

        /// <summary>
        /// سازنده استثنا با پیام خطا، مفهوم دامنه و نام خاصیت
        /// </summary>
        public DomainException(string message, DomainConcept domainConcept, string propertyName)
            : this(message, domainConcept, propertyName, null)
        {
        }

        /// <summary>
        /// سازنده اصلی با همه پارامترها
        /// </summary>
        public DomainException(
            string message,
            DomainConcept? domainConcept,
            string? propertyName,
            Exception? innerException)
            : base(message, innerException)
        {
            DomainConcept = domainConcept;
            PropertyName = propertyName;
        }
    }
}
