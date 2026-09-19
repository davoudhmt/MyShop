using Application.DTOs.Catalog.ProductType;
using Application.Interfaces.Catalog;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// کنترلر نوع محصول
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        /// <summary>
        /// سازنده کنترلر نوع محصول
        /// </summary>
        /// <param name="productTypeService">سرویس نوع محصول</param>
        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        /// <summary>
        /// دریافت همه انواع محصول
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productTypeService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// دریافت نوع محصول با شناسه
        /// </summary>
        /// <param name="id">شناسه نوع محصول</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _productTypeService.GetByIdAsync(id);
            if (result is null)
                return NotFound($"نوع محصول با شناسه {id} یافت نشد.");

            return Ok(result);
        }

        /// <summary>
        /// دریافت انواع محصول یک دسته‌بندی خاص
        /// </summary>
        /// <param name="categoryId">شناسه دسته‌بندی</param>
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategoryId(long categoryId)
        {
            var result = await _productTypeService.GetByCategoryIdAsync(categoryId);
            return Ok(result);
        }

        /// <summary>
        /// دریافت انواع محصول با صفحه‌بندی و فیلتر
        /// </summary>
        /// <param name="filter">پارامترهای فیلتر</param>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] ProductTypeFilterDto filter)
        {
            var result = await _productTypeService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// ساخت نوع محصول جدید
        /// </summary>
        /// <param name="dto">اطلاعات نوع محصول جدید</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductTypeDto dto)
        {
            try
            {
                var result = await _productTypeService.CreateAsync(dto);
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
        /// به‌روزرسانی نوع محصول
        /// </summary>
        /// <param name="dto">اطلاعات جدید نوع محصول</param>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProductTypeDto dto)
        {
            try
            {
                var result = await _productTypeService.UpdateAsync(dto);
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
        /// حذف منطقی نوع محصول
        /// </summary>
        /// <param name="id">شناسه نوع محصول</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _productTypeService.DeleteAsync(id);
                return Ok("نوع محصول با موفقیت حذف شد.");
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