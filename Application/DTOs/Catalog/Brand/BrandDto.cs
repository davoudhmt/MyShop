namespace Application.DTOs.Catalog.Brand
{
    /// <summary>
    /// DTO برای نمایش اطلاعات برند
    /// </summary>
    public class BrandDto
    {
        /// <summary>
        /// شناسه برند
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام برند
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات برند
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// آدرس تصویر برند
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// وضعیت برند
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// تاریخ به‌روزرسانی
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}