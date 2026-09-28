using Application.DTOs.Catalog.Tag;
using Application.DTOs.Common;

namespace Application.Interfaces.Services.Catalog
{
    /// <summary>
    /// واسط سرویس برچسب
    /// </summary>
    public interface ITagService
    {
        /// <summary>
        /// دریافت همه برچسب‌ها
        /// </summary>
        Task<List<TagDto>> GetAllAsync();

        /// <summary>
        /// دریافت برچسب با شناسه
        /// </summary>
        /// <param name="id">شناسه برچسب</param>
        Task<TagDto?> GetByIdAsync(long id);

        /// <summary>
        /// دریافت برچسب‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        Task<PagedResult<TagDto>> GetPagedAsync(TagFilterDto filter);

        /// <summary>
        /// ساخت برچسب جدید
        /// </summary>
        /// <param name="dto">اطلاعات برچسب جدید</param>
        Task<TagDto> CreateAsync(CreateTagDto dto);

        /// <summary>
        /// به‌روزرسانی برچسب
        /// </summary>
        /// <param name="dto">اطلاعات جدید برچسب</param>
        Task<TagDto> UpdateAsync(UpdateTagDto dto);

        /// <summary>
        /// حذف منطقی برچسب
        /// </summary>
        /// <param name="id">شناسه برچسب</param>
        Task DeleteAsync(long id);
    }
}