using Application.DTOs.Catalog.Feature;
using Application.DTOs.Common;

namespace Application.Interfaces.Catalog
{
    /// <summary>
    /// واسط سرویس ویژگی
    /// </summary>
    public interface IFeatureService
    {
        /// <summary>
        /// دریافت همه ویژگی‌ها
        /// </summary>
        Task<List<FeatureDto>> GetAllAsync();

        /// <summary>
        /// دریافت ویژگی با شناسه
        /// </summary>
        /// <param name="id">شناسه ویژگی</param>
        Task<FeatureDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت ویژگی‌های یک نوع محصول خاص
        /// </summary>
        /// <param name="productTypeId">شناسه نوع محصول</param>
        Task<List<FeatureDto>> GetByProductTypeIdAsync(long productTypeId);

        /// <summary>
        /// دریافت ویژگی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<FeatureDto>> GetPagedAsync(FeatureFilterDto filter);

        /// <summary>
        /// ساخت ویژگی جدید
        /// </summary>
        /// <param name="dto">اطلاعات ویژگی جدید</param>
        Task<FeatureDto> CreateAsync(CreateFeatureDto dto);

        /// <summary>
        /// به‌روزرسانی ویژگی
        /// </summary>
        /// <param name="dto">اطلاعات جدید ویژگی</param>
        Task<FeatureDto> UpdateAsync(UpdateFeatureDto dto);

        /// <summary>
        /// حذف منطقی ویژگی
        /// </summary>
        /// <param name="id">شناسه ویژگی</param>
        Task DeleteAsync(long id);
    }
}