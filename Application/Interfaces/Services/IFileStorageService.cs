namespace Application.Interfaces.Services
{
    /// <summary>
    /// اینترفیس مدیریت فایل‌ها (آپلود و حذف) - مشترک بین تمام ماژول‌ها
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// حذف فیزیکی فایل از سرور
        /// </summary>
        /// <param name="fileUrl">مسیر نسبی یا مطلق فایل</param>
        Task DeleteAsync(string fileUrl);

        /// <summary>
        /// حذف فیزیکی چندین فایل به صورت گروهی
        /// </summary>
        Task DeleteMultipleAsync(IEnumerable<string> fileUrls);
    }
}