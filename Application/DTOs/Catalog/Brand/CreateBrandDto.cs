namespace Application.DTOs.Catalog.Brand
{
    /// <summary>
    /// DTO برای ساخت برند جدید
    /// </summary>
    public class CreateBrandDto
    {
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
    }
}