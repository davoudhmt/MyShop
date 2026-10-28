using Application.Interfaces.Services.Identity;

namespace API.Services
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت رمز عبور با استفاده از الگوریتم استاندارد BCrypt
    /// </summary>
    public class BCryptPasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// تولید هش امن از رمز عبور خام با استفاده از BCrypt
        /// </summary>
        /// <param name="plainPassword">رمز عبور خام (PlainText)</param>
        /// <returns>رشته هش‌شده (مناسب برای ذخیره در دیتابیس)</returns>
        public string HashPassword(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("رمز عبور نمی‌تواند خالی باشد.", nameof(plainPassword));

            // BCrypt به صورت خودکار Salt تولید می‌کند و آن را در خروجی ذخیره می‌کند
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        /// <summary>
        /// مقایسه رمز عبور خام با هش ذخیره‌شده
        /// </summary>
        /// <param name="plainPassword">رمز عبور خام وارد شده توسط کاربر</param>
        /// <param name="hashedPassword">هش ذخیره‌شده در دیتابیس</param>
        /// <returns>در صورت تطابق، true برمی‌گرداند</returns>
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            // BCrypt به صورت خودکار Salt را از داخل hashedPassword استخراج و مقایسه می‌کند
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}