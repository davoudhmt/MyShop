namespace Application.DTOs.Catalog.Category
{
    /// <summary>
    /// DTO برای نمایش دسته‌بندی
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// شناسه یکتای دسته‌بندی
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام دسته‌بندی
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات دسته‌بندی (اختیاری)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// آدرس تصویر دسته‌بندی (اختیاری)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// کد دسته‌بندی
        /// </summary>
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// شناسه دسته‌بندی والد (اگر زیردسته باشد)
        /// </summary>
        public long? ParentCategoryId { get; set; }

        /// <summary>
        /// وضعیت دسته‌بندی
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// تاریخ آخرین به‌روزرسانی (اختیاری)
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}