namespace Application.DTOs.Catalog.ProductType
{
    /// <summary>
    /// DTO برای ساخت نوع محصول جدید
    /// </summary>
    public class CreateProductTypeDto
    {
        /// <summary>
        /// نام نوع محصول (اجباری)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات نوع محصول (اختیاری)
        /// </summary>
        public string? Description { get; set; }
    }
}