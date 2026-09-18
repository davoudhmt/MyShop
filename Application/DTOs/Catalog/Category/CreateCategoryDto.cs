namespace Application.DTOs.Catalog.Category
{
    /// <summary>
    /// DTO برای ساخت دسته‌بندی جدید
    /// </summary>
    public class CreateCategoryDto
    {
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
        /// شناسه دسته‌بندی والد برای ساخت زیردسته (اختیاری)
        /// </summary>
        public long? ParentCategoryId { get; set; }
    }
}