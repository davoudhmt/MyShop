namespace Application.DTOs.Identity.Person
{
    /// <summary>
    /// DTO برای ایجاد شخص جدید توسط ادمین در پنل مدیریت
    /// </summary>
    public class CreatePersonDto
    {
        /// <summary>
        /// شماره موبایل شخص (اجباری و باید فرمت معتبر داشته باشد)
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// نام شخص (اختیاری)
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی شخص (اختیاری)
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// شماره تلفن ثابت (اختیاری)
        /// </summary>
        public string? Landline { get; set; }

        /// <summary>
        /// آدرس ایمیل (اختیاری)
        /// </summary>
        public string? Email { get; set; }
    }
}