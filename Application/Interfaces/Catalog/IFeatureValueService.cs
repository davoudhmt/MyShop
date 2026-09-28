using Application.DTOs.Catalog.FeatureValue;
using Application.DTOs.Common;

namespace Application.Interfaces.Catalog
{
    /// <summary>
    /// واسط سرویس مقدار ویژگی
    /// </summary>
    public interface IFeatureValueService
    {
        /// <summary>
        /// دریافت همه مقادیر ویژگی
        /// </summary>
        Task<List<FeatureValueDto>> GetAllAsync();

        /// <summary>
        /// دریافت مقدار ویژگی با شناسه
        /// </summary>
        /// <param name="id">شناسه مقدار ویژگی</param>
        Task<FeatureValueDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت مقادیر یک ویژگی خاص
        /// </summary>
        /// <param name="featureId">شناسه ویژگی</param>
        Task<List<FeatureValueDto>> GetByFeatureIdAsync(long featureId);

        /// <summary>
        /// دریافت مقادیر ویژگی با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<FeatureValueDto>> GetPagedAsync(FeatureValueFilterDto filter);

        /// <summary>
        /// ساخت مقدار ویژگی جدید
        /// </summary>
        /// <param name="dto">اطلاعات مقدار ویژگی جدید</param>
        Task<FeatureValueDto> CreateAsync(CreateFeatureValueDto dto);

        /// <summary>
        /// به‌روزرسانی مقدار ویژگی
        /// </summary>
        /// <param name="dto">اطلاعات جدید مقدار ویژگی</param>
        Task<FeatureValueDto> UpdateAsync(UpdateFeatureValueDto dto);

        /// <summary>
        /// حذف منطقی مقدار ویژگی
        /// </summary>
        /// <param name="id">شناسه مقدار ویژگی</param>
        Task DeleteAsync(long id);
    }
}