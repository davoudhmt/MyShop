using Application.DTOs.Catalog.ProductType;
using Application.DTOs.Common;
using Application.Interfaces.Services.Catalog;
using API.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog
{
    /// <summary>
    /// کنترلر مدیریت انواع محصول
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductTypesController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        /// <summary>
        /// سازنده کنترلر
        /// </summary>
        public ProductTypesController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        /// <summary>
        /// دریافت همه انواع محصول
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productTypeService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// دریافت انواع محصول با صفحه‌بندی و فیلتر
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<ProductTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] ProductTypeFilterDto filter)
        {
            var result = await _productTypeService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// دریافت انواع محصول یک دسته‌بندی خاص
        /// </summary>
        [HttpGet("by-category/{categoryId:long}")]
        [ProducesResponseType(typeof(List<ProductTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByCategoryId(long categoryId)
        {
            var result = await _productTypeService.GetByCategoryIdAsync(categoryId);
            return Ok(result);
        }

        /// <summary>
        /// دریافت نوع محصول با شناسه
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProductTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _productTypeService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"نوع محصول با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// ساخت نوع محصول جدید
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductTypeDto dto)
        {
            var result = await _productTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// به‌روزرسانی نوع محصول
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(ProductTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductTypeDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            var result = await _productTypeService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// حذف منطقی نوع محصول
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _productTypeService.DeleteAsync(id);
            return Ok(new { message = "نوع محصول با موفقیت حذف شد." });
        }

        /// <summary>
        /// فعالسازی نوع محصول بر اساس شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:long}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(long id)
        {
            await _productTypeService.ActivateAsync(id);
            return Ok(new { message = "نوع محصول با موفقیت فعال شد." });
        }

        /// <summary>
        /// غیرفعالسازی نوع محصول بر اساس شناسه
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:long}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(long id)
        {
            await _productTypeService.DeactivateAsync(id);
            return Ok(new { message = "نوع محصول با موفقیت غیرفعال شد." });
        }
    }
}