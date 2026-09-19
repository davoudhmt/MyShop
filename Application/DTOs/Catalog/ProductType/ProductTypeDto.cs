namespace Application.DTOs.Catalog.ProductType
{
    /// <summary>
    /// DTO برای نمایش نوع محصول
    /// </summary>
    public class ProductTypeDto
    {
        /// <summary>
        /// شناسه یکتای نوع محصول
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام نوع محصول
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات نوع محصول (اختیاری)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// وضعیت نوع محصول
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