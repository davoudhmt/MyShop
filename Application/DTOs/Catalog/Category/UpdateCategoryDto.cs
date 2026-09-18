namespace Application.DTOs.Catalog.Category
{
    /// <summary>
    /// DTO برای به‌روزرسانی دسته‌بندی
    /// </summary>
    public class UpdateCategoryDto
    {
        /// <summary>
        /// شناسه دسته‌بندی (اجباری)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام دسته‌بندی (اجباری)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// کد دسته‌بندی (اجباری)
        /// </summary>
        public string CategoryCode { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات دسته‌بندی (اختیاری)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// آدرس تصویر دسته‌بندی (اختیاری)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// شناسه دسته‌بندی والد برای تغییر زیردسته (اختیاری)
        /// </summary>
        public long? ParentCategoryId { get; set; }

        /// <summary>
        /// وضعیت دسته‌بندی:
        /// - "Active": فعال
        /// - "Inactive": غیرفعال
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}