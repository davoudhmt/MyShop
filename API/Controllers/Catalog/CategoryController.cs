using Application.DTOs.Catalog.Category;
using Application.Interfaces.Catalog;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// کنترلر دسته‌بندی
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// سازنده کنترلر دسته‌بندی
        /// </summary>
        /// <param name="categoryService">سرویس دسته‌بندی</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// دریافت همه دسته‌بندی‌ها
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// دریافت دسته‌بندی با شناسه
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result is null)
                return NotFound($"دسته‌بندی با شناسه {id} یافت نشد.");

            return Ok(result);
        }

        /// <summary>
        /// دریافت دسته‌بندی‌ها با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] CategoryFilterDto filter)
        {
            var result = await _categoryService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت زیردسته‌های یک دسته
        /// </summary>
        /// <param name="id">شناسه دسته والد</param>
        [HttpGet("{id}/children")]
        public async Task<IActionResult> GetChildren(long id)
        {
            var result = await _categoryService.GetChildrenAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// ساخت دسته‌بندی جدید
        /// </summary>
        /// <param name="dto">اطلاعات دسته‌بندی جدید</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            try
            {
                var result = await _categoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// به‌روزرسانی دسته‌بندی
        /// </summary>
        /// <param name="dto">اطلاعات جدید دسته‌بندی</param>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryDto dto)
        {
            try
            {
                var result = await _categoryService.UpdateAsync(dto);
                return Ok(result);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// حذف منطقی دسته‌بندی
        /// </summary>
        /// <param name="id">شناسه دسته‌بندی</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                return Ok("دسته‌بندی با موفقیت حذف شد.");
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}