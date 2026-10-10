using Application.DTOs.Catalog.Product;
using Application.DTOs.Common;
using Application.Interfaces.Services.Catalog;
using API.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Catalog
{
    /// <summary>
    /// کنترلر مدیریت محصولات
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// دریافت محصول با شناسه
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"محصول با شناسه {id} یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// دریافت محصول با اسلاگ (برای صفحات سئو)
        /// </summary>
        [HttpGet("slug/{slug}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _productService.GetBySlugAsync(slug);
            if (result is null)
                return NotFound(new { message = $"محصول با اسلاگ '{slug}' یافت نشد." });

            return Ok(result);
        }

        /// <summary>
        /// دریافت لیست محصولات با فیلتر و صفحه‌بندی
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetPagedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// ایجاد محصول جدید
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// به‌روزرسانی اطلاعات محصول
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "شناسه در مسیر و بدنه درخواست یکسان نیست." });

            var result = await _productService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// حذف منطقی محصول
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new { message = "محصول با موفقیت حذف شد." });
        }

        /// <summary>
        /// تنظیم یک تصویر به عنوان تصویر اصلی (کاور) محصول
        /// </summary>
        [HttpPut("{productId:long}/images/{imageId:long}/set-main")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetMainImage(long productId, long imageId)
        {
            await _productService.SetMainImageAsync(productId, imageId);
            return Ok(new { message = "تصویر اصلی با موفقیت تنظیم شد." });
        }

        /// <summary>
        /// حذف یک واریانت خاص از محصول
        /// </summary>
        /// <param name="productId">شناسه محصول</param>
        /// <param name="variantId">شناسه واریانت</param>
        [HttpDelete("{productId:long}/variants/{variantId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveVariant(long productId, long variantId)
        {
            await _productService.RemoveVariantAsync(productId, variantId);
            return Ok(new { message = "واریانت با موفقیت حذف شد." });
        }
    }
}