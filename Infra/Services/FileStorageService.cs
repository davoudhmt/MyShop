using Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infra.Services
{
    /// <summary>
    /// پیاده‌سازی سرویس مدیریت فایل‌ها برای حذف و آپلود فایل‌های فیزیکی روی سرور
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IWebHostEnvironment webHostEnvironment, ILogger<FileStorageService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            try
            {
                // فرض می‌کنیم fileUrl یک مسیر نسبی است، مثلاً: "uploads/products/img.jpg" یا "/uploads/products/img.jpg"
                // آن را به فرمت استاندارد تبدیل می‌کنیم
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
                // اگر حذف فایل فیزیکی با خطا مواجه شد (مثلاً دسترسی نداشتیم)، 
                // کل عملیات حذف رکورد از دیتابیس را متوقف نمی‌کنیم.
                // فقط خطا را لاگ می‌کنیم تا ادمین بعداً بررسی کند.
                _logger.LogError(ex, "خطا در حذف فیزیکی فایل: {FileUrl}", fileUrl);
            }

            await Task.CompletedTask;
        }

        public async Task DeleteMultipleAsync(IEnumerable<string> fileUrls)
        {
            if (fileUrls == null || !fileUrls.Any())
                return;

            foreach (var url in fileUrls)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    // استفاده از متد تکی که قبلاً نوشتیم (با مدیریت خطای داخلی)
                    await DeleteAsync(url);
                }
            }
        }
    }
}