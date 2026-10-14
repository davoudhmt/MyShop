using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    /// <summary>
    /// این شمارش وضعیت‌های مختلف یک موجودیت را نشان می‌دهد.
    /// </summary>
    public enum EntityStatus
    {
        /// <summary>
        /// این مقدار نشان‌دهنده وضعیت فعال یک موجودیت است.
        /// </summary>
        Active = 1,

        /// <summary>
        /// این مقدار نشان‌دهنده وضعیت غیرفعال یک موجودیت است.
        /// </summary>
        Inactive = 2,

        /// <summary>
        /// این مقدار نشان‌دهنده وضعیت حذف منطقی شده یک موجودیت است.
        /// </summary>
        Deleted = 3
    }
}
