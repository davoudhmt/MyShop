using FluentValidation;

namespace Application.Extensions
{
    /// <summary>
    /// متدهای کمکی و توسعه‌یافته برای FluentValidation
    /// </summary>
    public static class FluentValidationExtensions
    {
        /// <summary>
        /// اعتبارسنجی آدرس تصویر (بررسی ساختار URL و پسوند فایل)
        /// </summary>
        public static IRuleBuilderOptions<T, string?> MustBeValidImageUrl<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder.Must(url =>
            {
                // اگر خالی بود، توسط NotEmpty یا When مدیریت می‌شود
                if (string.IsNullOrWhiteSpace(url)) return true;

                // ۱. بررسی معتبر بودن ساختار URL
                if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _))
                    return false;

                // ۲. بررسی پسوند تصویری (حذف query string برای بررسی دقیق‌تر)
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp" };
                var path = url.ToLowerInvariant().Split('?')[0];

                return imageExtensions.Any(ext => path.EndsWith(ext));
            }).WithMessage("آدرس یا پسوند تصویر معتبر نمی‌باشد.");
        }
    }
}