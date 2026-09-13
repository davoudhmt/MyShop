namespace Application.DTOs.Catalog.Brand
{
    /// <summary>
    /// DTO برای به‌روزرسانی برند
    /// </summary>
    public class UpdateBrandDto
    {
        /// <summary>
        /// شناسه برند (الزامی)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام برند (الزامی، ۲ تا ۱۰۰ کاراکتر)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات برند (اختیاری، حداکثر ۵۰۰ کاراکتر)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// آدرس تصویر برند (اختیاری، حداکثر ۵۰۰ کاراکتر)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// وضعیت برند (فعال/غیرفعال)
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}