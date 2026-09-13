namespace Application.DTOs.Common
{
    /// <summary>
    /// نتیجه صفحه‌بندی شده
    /// </summary>
    /// <typeparam name="T">نوع داده</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// لیست آیتم‌های صفحه فعلی
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// شماره صفحه فعلی
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// تعداد آیتم در هر صفحه
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// تعداد کل آیتم‌ها
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// تعداد کل صفحات
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// آیا صفحه قبلی وجود داره؟
        /// </summary>
        public bool HasPrevious => PageNumber > 1;

        /// <summary>
        /// آیا صفحه بعدی وجود داره؟
        /// </summary>
        public bool HasNext => PageNumber < TotalPages;
    }
}