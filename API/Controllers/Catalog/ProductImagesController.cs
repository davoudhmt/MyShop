using Application.DTOs.Catalog.ProductImage;
using Application.Interfaces.Services.Catalog;
using API.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog
{
    /// <summary>
    /// کنترلر مدیریت تصاویر محصول
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _imageService;

        /// <summary>
        /// سازنده کنترلر با تزریق سرویس تصاویر محصول
        /// </summary>
        /// <param name="imageService"></param>
        public ProductImagesController(IProductImageService imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// دریافت تصویر با شناسه
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProductImageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _imageService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"تصویر با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// دریافت همه تصاویر یک محصول
        /// </summary>
        [HttpGet("product/{productId:long}")]
        [ProducesResponseType(typeof(List<ProductImageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByProductId(long productId)
        {
            var result = await _imageService.GetByProductIdAsync(productId);
            return Ok(result);
        }

        /// <summary>
        /// دریافت تصویر اصلی (کاور) یک محصول
        /// </summary>
        [HttpGet("product/{productId:long}/main")]
        [ProducesResponseType(typeof(ProductImageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMainImageByProductId(long productId)
        {
            var result = await _imageService.GetMainImageByProductIdAsync(productId);
            return Ok(result);
        }

        /// <summary>
        /// دریافت تصاویر یک واریانت خاص
        /// </summary>
        [HttpGet("variant/{variantId:long}")]
        [ProducesResponseType(typeof(List<ProductImageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByVariantId(long variantId)
        {
            var result = await _imageService.GetByVariantIdAsync(variantId);
            return Ok(result);
        }

        /// <summary>
        /// افزودن تصویر جدید
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductImageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductImageDto dto)
        {
            var result = await _imageService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات تصویر
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(ProductImageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductImageDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            var result = await _imageService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// حذف منطقی تصویر
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _imageService.DeleteAsync(id);
            return Ok(new { message = "تصویر با موفقیت حذف شد." });
        }
    }
}