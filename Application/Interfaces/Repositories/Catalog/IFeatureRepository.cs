using Application.DTOs.Catalog.Feature;
using Application.DTOs.Common;
using Domain.Entities.Catalog;

namespace Application.Interfaces.Repositories.Catalog
{
    /// <summary>
    /// واسط ریپازیتوری ویژگی
    /// </summary>
    public interface IFeatureRepository
    {
        /// <summary>
        /// دریافت همه ویژگی‌ها
        /// </summary>
        Task<List<Feature>> GetAllAsync();

        /// <summary>
        /// دریافت ویژگی با شناسه
        /// </summary>
        /// <param name="id">شناسه ویژگی</param>
        Task<Feature?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت ویژگی با نام
        /// </summary>
        /// <param name="name">نام ویژگی</param>
        Task<Feature?> GetByNameAsync(string name);

        /// <summary>
        /// دریافت ویژگی‌های یک نوع محصول خاص
        /// </summary>
        /// <param name="productTypeId">شناسه نوع محصول</param>
        Task<List<Feature>> GetByProductTypeIdAsync(long productTypeId);

        /// <summary>
        /// دریافت ویژگی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<Feature>> GetPagedAsync(FeatureFilterDto filter);

        /// <summary>
        /// افزودن ویژگی جدید
        /// </summary>
        /// <param name="feature">ویژگی جدید</param>
        Task AddAsync(Feature feature);

        /// <summary>
        /// به‌روزرسانی ویژگی
        /// </summary>
        /// <param name="feature">ویژگی برای به‌روزرسانی</param>
        void Update(Feature feature);

        /// <summary>
        /// بررسی وجود ویژگی با نام
        /// </summary>
        /// <param name="name">نام ویژگی</param>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        Task SaveChangesAsync();
    }
}