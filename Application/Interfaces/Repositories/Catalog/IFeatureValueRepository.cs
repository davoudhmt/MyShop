using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Common;
using Domain.Entities.Catalog;

namespace Application.Interfaces.Repositories.Catalog
{
    /// <summary>
    /// واسط ریپازیتوری مقدار ویژگی
    /// </summary>
    public interface IFeatureValueRepository
    {
        /// <summary>
        /// دریافت همه مقادیر ویژگی
        /// </summary>
        Task<List<FeatureValue>> GetAllAsync();

        /// <summary>
        /// دریافت مقدار ویژگی با شناسه
        /// </summary>
        /// <param name="id">شناسه مقدار ویژگی</param>
        Task<FeatureValue?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت مقادیر یک ویژگی خاص
        /// </summary>
        /// <param name="featureId">شناسه ویژگی</param>
        Task<List<FeatureValue>> GetByFeatureIdAsync(long featureId);

        /// <summary>
        /// دریافت مقادیر ویژگی با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<FeatureValue>> GetPagedAsync(FeatureValueFilterDto filter);

        /// <summary>
        /// افزودن مقدار ویژگی جدید
        /// </summary>
        /// <param name="featureValue">مقدار ویژگی جدید</param>
        Task AddAsync(FeatureValue featureValue);

        /// <summary>
        /// به‌روزرسانی مقدار ویژگی
        /// </summary>
        /// <param name="featureValue">مقدار ویژگی برای به‌روزرسانی</param>
        void Update(FeatureValue featureValue);

        /// <summary>
        /// بررسی وجود مقدار برای یک ویژگی خاص
        /// </summary>
        /// <param name="featureId">شناسه ویژگی</param>
        /// <param name="value">مقدار</param>
        Task<bool> ExistsAsync(long featureId, string value);

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        Task SaveChangesAsync();
    }
}