namespace Application.DTOs.Catalog.Tag
{
    /// <summary>
    /// DTO برای ساخت برچسب جدید
    /// </summary>
    public class CreateTagDto
    {
        /// <summary>
        /// نام برچسب (اجباری، بین 2 تا 50 کاراکتر)
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}