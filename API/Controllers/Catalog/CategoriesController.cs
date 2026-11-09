using Application.DTOs.Catalog.Category;
using Application.DTOs.Common;
using Application.Interfaces.Services.Catalog;
using API.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog
{
    /// <summary>
    /// کنترلر مدیریت دسته‌بندی‌ها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// سازنده کلاس دسته‌بندی‌ها با تزریق سرویس دسته‌بندی
        /// </summary>
        /// <param name="categoryService"></param>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// دریافت تمام دسته‌بندی‌ها
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// دریافت دسته‌بندی‌ها به صورت صفحه‌بندی شده
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] CategoryFilterDto filter)
        {
            var result = await _categoryService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت دسته‌بندی بر اساس شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null)
                return NotFound(new { message = $"دسته‌بندی با شناسه {id} یافت نشد." });

            return Ok(category);
        }

        /// <summary>
        /// دریافت دسته‌بندی‌های فرزند بر اساس شناسه دسته‌بندی والد
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:long}/children")]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChildren(long id)
        {
            var children = await _categoryService.GetChildrenAsync(id);
            return Ok(children);
        }

        /// <summary>
        /// ایجاد دسته‌بندی جدید
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            // ✅ try-catch حذف شد. مدیریت خطا بر عهده Middleware است
            var category = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// به‌روزرسانی دسته‌بندی موجود
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateCategoryDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            var result = await _categoryService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// حذف دسته‌بندی بر اساس شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok(new { message = "دسته‌بندی با موفقیت حذف شد." });
        }

        /// <summary>
        /// فعال‌سازی دسته بندی
        /// </summary>
        /// <param name="id">شناسه دسته بندی</param>
        [HttpPut("{id:long}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(long id)
        {
            await _categoryService.ActivateAsync(id);
            return Ok(new { message = "دسته‌بندی با موفقیت فعال شد." });
        }

        /// <summary>
        /// غیرفعالسازی دسته بندی
        /// </summary>
        /// <param name="id">شناسه دسته بندی</param>
        [HttpPut("{id:long}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(long id)
        {
            await _categoryService.DeactivateAsync(id);
            return Ok(new { message = "دسته‌بندی با موفقیت غیرفعال شد." });
        }
    }
}