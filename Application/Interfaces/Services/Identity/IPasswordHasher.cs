namespace Application.Interfaces.Services.Identity
{
    /// <summary>
    /// واسط سرویس مدیریت رمز عبور (هش کردن و مقایسه)
    /// این سرویس برای جداسازی الگوریتم هش از منطق بیزینسی طراحی شده است
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// تولید هش امن از رمز عبور خام
        /// </summary>
        /// <param name="plainPassword">رمز عبور خام (PlainText)</param>
        /// <returns>رشته هش‌شده (مناسب برای ذخیره در دیتابیس)</returns>
        string HashPassword(string plainPassword);

        /// <summary>
        /// مقایسه رمز عبور خام با هش ذخیره‌شده
        /// </summary>
        /// <param name="plainPassword">رمز عبور خام وارد شده توسط کاربر</param>
        /// <param name="hashedPassword">هش ذخیره‌شده در دیتابیس</param>
        /// <returns>در صورت تطابق، true برمی‌گرداند</returns>
        bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}