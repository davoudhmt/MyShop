using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Infra.Services
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت فایل‌ها برای حذف و آپلود فایل‌های فیزیکی روی سرور.
    /// این سرویس مسیرهای نسبی را به مسیرهای فیزیکی در wwwroot تبدیل کرده و عملیات فایل سیستم را با مدیریت خطای ایمن (Fail-Safe) انجام می‌دهد.
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileStorageService> _logger;

        /// <summary>
        /// سازنده سرویس مدیریت فایل و مقداردهی وابستگی‌ها از طریق DI
        /// </summary>
        /// <param name="webHostEnvironment">محیط میزبانی وب برای دسترسی به مسیر ریشه (wwwroot)</param>
        /// <param name="logger">سرویس ثبت وقایع (Logger) برای ردیابی عملیات و خطاهای فایل سیستم</param>
        public FileStorageService(IWebHostEnvironment webHostEnvironment, ILogger<FileStorageService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        /// <summary>
        /// حذف فیزیکی یک فایل از روی سرور بر اساس آدرس نسبی آن
        /// </summary>
        /// <param name="fileUrl">آدرس نسبی فایل (مثال: "uploads/products/img.jpg" یا "/uploads/products/img.jpg")</param>
        public async Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            try
            {
                // تبدیل آدرس به فرمت استاندارد (حذف اسلش‌های ابتدایی)
                var relativePath = fileUrl.TrimStart('/', '\\');

                // ترکیب با مسیر ریشه‌ی وب (wwwroot) برای به دست آوردن مسیر فیزیکی کامل
                var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    _logger.LogInformation("فایل با موفقیت حذف شد: {PhysicalPath}", physicalPath);
                }
                else
                {
                    _logger.LogWarning("فایل برای حذف یافت نشد (احتمالاً قبلاً پاک شده): {PhysicalPath}", physicalPath);
                }
            }
            catch (Exception ex)
            {
                // ⚠️ نکته‌ی بسیار مهم معماری:
                // اگر حذف فایل فیزیکی با خطا مواجه شد (مثلاً مشکل دسترسی)، 
                // کل عملیات حذف رکورد از دیتابیس را متوقف نمی‌کنیم (Fail-Safe).
                // فقط خطا را لاگ می‌کنیم تا ادمین بعداً بررسی کند.
                _logger.LogError(ex, "خطا در حذف فیزیکی فایل: {FileUrl}", fileUrl);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// حذف فیزیکی چندین فایل از روی سرور به صورت دسته‌جمعی
        /// </summary>
        /// <param name="fileUrls">مجموعه‌ای از آدرس‌های نسبی فایل‌ها برای حذف</param>
        public async Task DeleteMultipleAsync(IEnumerable<string> fileUrls)
        {
            if (fileUrls == null || !fileUrls.Any())
                return;

            foreach (var url in fileUrls)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    // استفاده از متد تکی که قبلاً نوشته شده (با مدیریت خطای داخلی ایمن)
                    await DeleteAsync(url);
                }
            }
        }
    }
}