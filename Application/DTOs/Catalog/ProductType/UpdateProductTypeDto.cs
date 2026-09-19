namespace Application.DTOs.Catalog.ProductType
{
    /// <summary>
    /// DTO برای به‌روزرسانی نوع محصول
    /// </summary>
    public class UpdateProductTypeDto
    {
        /// <summary>
        /// شناسه نوع محصول (اجباری)
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام نوع محصول (اجباری)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات نوع محصول (اختیاری)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// وضعیت نوع محصول:
        /// - "Active": فعال
        /// - "Inactive": غیرفعال
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}