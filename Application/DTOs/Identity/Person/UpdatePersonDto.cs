namespace Application.DTOs.Identity.Person
{
    /// <summary>
    /// DTO برای به‌روزرسانی اطلاعات شخص توسط ادمین
    /// </summary>
    public class UpdatePersonDto
    {
        /// <summary>
        /// شناسه یکتای شخص برای به‌روزرسانی
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// نام جدید شخص (اختیاری)
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی جدید شخص (اختیاری)
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// شماره موبایل جدید (اجباری و باید در سیستم یکتا باشد)
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// شماره تلفن ثابت جدید (اختیاری)
        /// </summary>
        public string? Landline { get; set; }

        /// <summary>
        /// آدرس ایمیل جدید (اختیاری)
        /// </summary>
        public string? Email { get; set; }
    }
}