using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Common
{
    public static class Guard
    {
        private static readonly Regex MobileRegex = new Regex(@"^09[0-9]{9}$", RegexOptions.Compiled);

        private static readonly HashSet<string> ValidImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".tiff"
        };

        /// <summary>
        /// بررسی می‌کند که آیا شناسه ورودی نامعتبر است یا خیر.
        /// شناسه در صورتی نامعتبر است که کمتر یا مساوی صفر باشد.
        /// </summary>
        /// <param name="id">شناسه ورودی</param>
        public static bool IsInvalidId(long id) => id <= 0;

        /// <summary>
        /// بررسی می‌کند که آیا مقدار Enum ورودی نامعتبر است یا خیر.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInvalidEnum<TEnum>(TEnum value) where TEnum : struct, Enum => !Enum.IsDefined(typeof(TEnum), value);

        /// <summary>
        /// بررسی می‌کند که آیا مقدار Enum ورودی نامعتبر است یا خیر.
        /// در صورت null بودن مقدار، false برمی‌گرداند.
        /// </summary>
        /// <typeparam name="TEnum">نوع Enum</typeparam>
        /// <param name="value">مقدار ورودی</param>
        public static bool IsInvalidEnum<TEnum>(TEnum? value) where TEnum : struct, Enum => value.HasValue && !Enum.IsDefined(typeof(TEnum), value.Value);

        /// <summary>
        /// بررسی می‌کند که آیا مقدار عددی ورودی منفی یا صفر است یا خیر.
        /// در صورت null بودن مقدار ورودی، false برمی‌گرداند.
        /// </summary>
        /// <typeparam name="T">نوع عددی</typeparam>
        /// <param name="value">مقدار ورودی</param>
        public static bool IsNegativeOrZero<T>(T? value) where T : struct, IComparable<T> => value.HasValue && value.Value.CompareTo(default(T)) <= 0;

        /// <summary>
        /// بررسی می‌کند که آیا مقدار عددی ورودی خارج از بازه مشخص‌شده قرار دارد یا خیر.
        /// در صورت null بودن مقدار ورودی، false برمی‌گرداند.
        /// </summary>
        /// <typeparam name="T">نوع عددی</typeparam>
        /// <param name="value">مقدار ورودی</param>
        /// <param name="min">حداقل مقدار مجاز</param>
        /// <param name="max">حداکثر مقدار مجاز</param>
        public static bool IsOutOfRange<T>(T? value, T min, T max) where T : struct, IComparable<T> => value.HasValue && (value.Value.CompareTo(min) < 0 || value.Value.CompareTo(max) > 0);

        /// <summary>
		/// بررسی می‌کند که آیا رشته ورودی نامعتبر است یا خیر.
		/// رشته در صورتی نامعتبر است که null یا تهی باشد، یا طول آن خارج از بازه مشخص‌شده قرار گیرد.
		/// این متد برای رشته‌های اجباری استفاده می‌شود.
		/// </summary>
		/// <param name="value">رشته ورودی</param>
		/// <param name="minLength">حداقل طول مجاز</param>
		/// <param name="maxLength">حداکثر طول مجاز</param>
        public static bool IsInvalidLength(string? value, int minLength, int maxLength) => string.IsNullOrWhiteSpace(value) || (value.Length < minLength || value.Length > maxLength);

        /// <summary>
		/// بررسی می‌کند که آیا طول رشته ورودی از حداکثر طول مجاز بیشتر است یا خیر.
		/// در صورت null یا تهی بودن رشته، false برمی‌گرداند (مناسب برای فیلدهای اختیاری).
		/// </summary>
		/// <param name="value">رشته ورودی</param>
		/// <param name="maxLength">حداکثر طول مجاز</param>
        public static bool IsOverMaxLength(string? value, int maxLength) => !string.IsNullOrWhiteSpace(value) && value.Length > maxLength;

        /// <summary>
        /// بررسی می‌کند که آیا URL ورودی نامعتبر است یا خیر.
        /// در صورت null یا تهی بودن، false برمی‌گرداند (مناسب برای فیلدهای اختیاری).
        /// </summary>
        /// <param name="url">URL ورودی</param>
        public static bool IsInvalidUrl(string? url) => !string.IsNullOrWhiteSpace(url) && (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _) || url.Contains(".."));

        /// <summary>
        /// بررسی می‌کند که آیا پسوند تصویر معتبر است یا خیر
        /// </summary>
        public static bool IsInvalidImageExtension(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var extension = Path.GetExtension(url);

            return string.IsNullOrWhiteSpace(extension) || !ValidImageExtensions.Contains(extension);
        }

        /// <summary>
        /// در این متد بررسی می‌شود که آیا یک مرجع به خود ایجاد نشده است یا خیر
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        public static bool IsSelfReference(long? id, long? parentId) => id.HasValue && parentId.HasValue && id.Value == parentId.Value;

        /// <summary>
        /// بررسی می‌کند که آیا شماره موبایل ورودی فرمت معتبر ایران (09xxxxxxxxx) را دارد یا خیر.
        /// </summary>
        /// <param name="mobile">شماره موبایل ورودی</param>
        public static bool IsInvalidMobile(string? mobile) => string.IsNullOrWhiteSpace(mobile) || !MobileRegex.IsMatch(mobile);
    }
}
