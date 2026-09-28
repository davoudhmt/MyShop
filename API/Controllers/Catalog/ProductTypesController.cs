using Application.DTOs.Catalog.ProductType;
using Application.DTOs.Common;
using Application.Interfaces.Services.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog  // ← یکدست با بقیه
{
    /// <summary>
    /// کنترلر مدیریت انواع محصول
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductTypesController : ControllerBase  // ← جمع
    {
        private readonly IProductTypeService _productTypeService;

        /// <summary>
        /// سازنده کنترلر
        /// </summary>
        public ProductTypesController(IProductTypeService productTypeService)  // ← اصلاح شد
        {
            _productTypeService = productTypeService;
        }

        /// <summary>
        /// دریافت همه انواع محصول
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductTypeDto>), StatusCodes.Status200OK)]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductTypeDto dto)
        {
            try
            {
                var result = await _productTypeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// به‌روزرسانی نوع محصول
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductTypeDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            try
            {
                var result = await _productTypeService.UpdateAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("یافت نشد"))
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// حذف منطقی نوع محصول
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _productTypeService.DeleteAsync(id);
                return Ok(new { message = "نوع محصول با موفقیت حذف شد." });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("یافت نشد"))
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}