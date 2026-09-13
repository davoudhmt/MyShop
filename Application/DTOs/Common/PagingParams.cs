namespace Application.DTOs.Common
{
    /// <summary>
    /// پارامترهای صفحه‌بندی
    /// </summary>
    public class PagingParams
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        /// <summary>
        /// شماره صفحه (از ۱ شروع می‌شه)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// تعداد آیتم در هر صفحه (حداکثر ۱۰۰)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}